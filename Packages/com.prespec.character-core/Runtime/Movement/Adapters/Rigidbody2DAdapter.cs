// Copyright (c) Prespec.
// See LICENSE.md in the package root.

using UnityEngine;

namespace Prespec.CharacterCore.Runtime.Movement.Adapters
{
    /// <summary>
    /// Adapter for Rigidbody2D. Applies the MovementInstruction using Rigidbody2D APIs.
    /// No blending here; service pre-bakes all numbers.
    /// Strategy: MovePosition for base+overlay+impulse. (Single strategy for clarity.)
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class Rigidbody2DAdapter : MonoBehaviour, IMovementAdapter
    {
        private Rigidbody2D _rb;
        private LocomotionProfile _profile; // read-only hints if needed

        public void Initialize(LocomotionProfile profile)
        {
            _rb = GetComponent<Rigidbody2D>();
            if (_rb == null)
            {
                Debug.LogError($"{nameof(Rigidbody2DAdapter)} requires {nameof(Rigidbody2D)}.");
            }
            _profile = profile;
        }

        public void Apply(MovementInstruction instruction, float dt)
        {
            if (_rb == null) return;

            // Base motion for this tick (Desired is units/sec -> multiply by dt)
            Vector2 baseDelta = (instruction.LocksCount <= 0)
                ? new Vector2(instruction.Desired.x, instruction.Desired.y) * dt
                : Vector2.zero;

            // Overlay and impulse are world-space displacements for this tick
            Vector2 overlayDelta = new Vector2(instruction.OverlayDelta.x, instruction.OverlayDelta.y);
            Vector2 impulseDelta = new Vector2(instruction.Impulse.x,      instruction.Impulse.y);

            Vector2 totalDelta = baseDelta + overlayDelta + impulseDelta;

            if (totalDelta.sqrMagnitude > 0f)
            {
                _rb.MovePosition(_rb.position + totalDelta);
            }

            // Facing: optional (Rigidbody2D rotation). Many 2D top-down games rotate sprites instead.
            // If you want rotation here, uncomment and choose your convention.
            // if (instruction.Face.sqrMagnitude > 0f)
            // {
            //     float angleDeg = Mathf.Atan2(instruction.Face.y, instruction.Face.x) * Mathf.Rad2Deg;
            //     _rb.MoveRotation(angleDeg);
            // }
        }
    }
}
