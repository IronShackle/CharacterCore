// Runtime/Movement/MovementServiceComponent.cs
using System;
using System.Collections.Generic;
using UnityEngine;
using Prespec.CharacterCore.Contracts.Movement;
using Prespec.CharacterCore.Runtime.Movement.Adapters;
using Prespec.CharacterCore.Runtime.Movement;

namespace Prespec.CharacterCore.Runtime.Movement
{
    /// <summary>
    /// CENTRAL BLENDER (dimension-agnostic).
    /// - Intent -> deadzone/accel/decel (LocomotionProfile)
    /// - Velocity modifiers (multiplicative)
    /// - Displacement overlays (world-space, not scaled by modifiers)
    /// - Locks suppress base intent (overlays still apply)
    /// Emits MovementCommand each FixedUpdate; never touches Transform/Rigidbody.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class MovementServiceComponent : MonoBehaviour, IMovement2D
    {
        [Header("Config")]
        [SerializeField] private LocomotionProfile _profile;

        [Tooltip("If true, while an overlay is active, face its direction. Default: false.")]
        [SerializeField] private bool _faceInOverlayDirection = false;

        [Header("Adapter (translator)")]
        [SerializeField] private MonoBehaviour _adapterBehaviour; // IMovementAdapter2D
        private IMovementAdapter2D _adapter;

        // Time seam for tests
        private Func<float> _fixedDtProvider = () => Time.fixedDeltaTime;

        // Blended vectors
        private Vector3 _moveIntent;   // from input
        private Vector3 _velocity;     // after accel/decel & modifiers
        private Vector3 _face = Vector3.right;

        // One-tick impulse displacement
        private Vector3 _impulse;

        // Internals (keyed by Guid)
        private struct ModifierEntry { public Guid Id; public float Multiplier; }
        private struct OverlayEntry  { public Guid Id; public Vector3 PerSecond; public float Remaining; }

        private readonly List<ModifierEntry> _modifiers = new(4);
        private readonly List<OverlayEntry>  _overlays  = new(4);
        private readonly List<Guid>          _toRemove  = new(4);

        // Keyed locks
        private readonly HashSet<object> _lockKeys = new();
        private int LocksCount => _lockKeys.Count;

        // Read model
        [SerializeField] private MovementState _state = new MovementState();

        private void Awake()
        {
            if (_profile == null)
            {
                Debug.LogWarning($"{nameof(MovementServiceComponent)} has no LocomotionProfile; using safe defaults.");
                _profile = ScriptableObject.CreateInstance<LocomotionProfile>();
                _profile.baseSpeed = 0f;
                _profile.acceleration = 9999f;
                _profile.deceleration = 9999f;
                _profile.inputDeadzone = 0f;
                _profile.faceInMoveDirection = true;
            }

            if (_adapterBehaviour == null || (_adapter = _adapterBehaviour as IMovementAdapter2D) == null)
                Debug.LogError($"{nameof(MovementServiceComponent)} requires an IMovementAdapter2D.");
            else
                _adapter.Initialize(_profile); // read-only hints only
        }

        private void FixedUpdate()
        {
            float dt = _fixedDtProvider();

            // 1) Intent -> desired velocity (deadzone + locks)
            Vector3 desiredVel = Vector3.zero;
            float mag = _moveIntent.magnitude;
            if (mag >= _profile.inputDeadzone && LocksCount == 0)
            {
                Vector3 dir = (mag > 0f) ? (_moveIntent / mag) : Vector3.zero;
                desiredVel = dir * Mathf.Max(0f, _profile.baseSpeed);
            }

            // 2) Accel/decel
            _velocity = MoveTowards3(_velocity, desiredVel, AccelFor(_velocity, desiredVel) * dt);

            // 3) Modifiers (multiplicative)
            float mul = 1f;
            for (int i = 0; i < _modifiers.Count; i++)
                mul *= Mathf.Max(0f, _modifiers[i].Multiplier);
            Vector3 velAfterMods = _velocity * mul;

            // 4) Overlays: sum this-tick delta; remove finished
            Vector3 overlayDelta = Vector3.zero;
            _toRemove.Clear();
            for (int i = 0; i < _overlays.Count; i++)
            {
                var ov = _overlays[i];
                float step = Mathf.Min(ov.Remaining, dt);
                overlayDelta += ov.PerSecond * step;
                ov.Remaining -= step;
                _overlays[i] = ov;
                if (ov.Remaining <= 0f) _toRemove.Add(ov.Id);
            }
            for (int r = 0; r < _toRemove.Count; r++)
            {
                int idx = _overlays.FindIndex(e => e.Id == _toRemove[r]);
                if (idx >= 0) _overlays.RemoveAt(idx);
            }

            // 5) Facing policy
            if (_profile.faceInMoveDirection && velAfterMods.sqrMagnitude > 1e-6f)
                _face = velAfterMods.normalized;

            if (_faceInOverlayDirection && overlayDelta.sqrMagnitude > 1e-6f)
                _face = overlayDelta.normalized;

            // 6) Compose & apply command
            var cmd = new MovementCommand(
                desired:     velAfterMods,
                impulse:     _impulse,
                overlayDelta: overlayDelta,
                face:        _face,
                locksCount:  LocksCount
            );

            _adapter?.Apply(cmd, dt);

            // 7) Clear one-tick impulse
            _impulse = Vector3.zero;

            // 8) Update read model
            _state.Velocity      = velAfterMods;
            _state.Face          = _face;
            _state.OverlayActive = cmd.HasOverlay;
            _state.LocksCount    = LocksCount;
        }

        // ---- IMovement2D (dimension-agnostic) ----

        public void SetMoveIntent(in MoveIntent intent)
        {
            _moveIntent = new Vector3(intent.X, intent.Y, intent.Z);
        }

        public Guid RegisterVelocityModifier(in VelocityModifier modifier)
        {
            var id = Guid.NewGuid();
            _modifiers.Add(new ModifierEntry { Id = id, Multiplier = Mathf.Max(0f, modifier.Multiplier) });
            return id;
        }

        public bool UnregisterVelocityModifier(Guid token)
        {
            int idx = _modifiers.FindIndex(m => m.Id == token);
            if (idx < 0) return false;
            _modifiers.RemoveAt(idx);
            return true;
        }

        public Guid RegisterDisplacementOverlay(in DisplacementOverlay overlay)
        {
            var id = Guid.NewGuid();
            var perSec = OverlayPerSecond(overlay.DirectionX, overlay.DirectionY, overlay.DirectionZ, overlay.Distance, overlay.Duration);
            _overlays.Add(new OverlayEntry { Id = id, PerSecond = perSec, Remaining = Mathf.Max(1e-6f, overlay.Duration) });
            return id;
        }

        public bool UnregisterDisplacementOverlay(Guid token)
        {
            int idx = _overlays.FindIndex(o => o.Id == token);
            if (idx < 0) return false;
            _overlays.RemoveAt(idx);
            return true;
        }

        public void AddImpulse(float dx, float dy, float dz)
        {
            _impulse.x += dx; _impulse.y += dy; _impulse.z += dz;
        }

        public void TakeLock(object key)    { _lockKeys.Add(key ?? this); }
        public void ReleaseLock(object key) { _lockKeys.Remove(key ?? this); }

        public void Face(float x, float y, float z)
        {
            var v = new Vector3(x, y, z);
            if (v.sqrMagnitude > 1e-6f) _face = v.normalized;
        }

        // ---- Optional diagnostics/test seam ----

        public MovementState GetState() => _state;
        public void SetFixedDeltaProvider(Func<float> provider) => _fixedDtProvider = provider ?? (() => Time.fixedDeltaTime);

        // ---- Helpers ----

        private float AccelFor(in Vector3 current, in Vector3 target)
        {
            float cur = current.magnitude;
            float tar = target.magnitude;
            return (tar > cur ? _profile.acceleration : _profile.deceleration);
        }

        private static Vector3 MoveTowards3(in Vector3 current, in Vector3 target, float maxDelta)
        {
            Vector3 delta = target - current;
            float dist = delta.magnitude;
            if (dist <= maxDelta || dist == 0f) return target;
            return current + delta / dist * maxDelta;
        }

        private static Vector3 OverlayPerSecond(float dirX, float dirY, float dirZ, float distance, float duration)
        {
            var dir = new Vector3(dirX, dirY, dirZ);
            var n = dir.sqrMagnitude > 1e-6f ? dir.normalized : Vector3.zero;
            float d = Mathf.Max(1e-6f, duration);
            return n * (Mathf.Max(0f, distance) / d);
        }
    }
}
