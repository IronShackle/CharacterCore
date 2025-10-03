// Copyright (c) Prespec.
// See LICENSE.md in the package root.

using UnityEngine;

namespace Prespec.CharacterCore.Runtime.Movement.Adapters
{
    /// <summary>
    /// Adapter for Transform-based (non-physics) 2D application.
    /// Translates the motor-agnostic MovementInstruction into Transform2DMotor calls.
    /// - Ignores Z (2D); do not perform any blending here.
    /// </summary>
    [RequireComponent(typeof(Transform2DMotor))]
    public sealed class Transform2DAdapter : MonoBehaviour, IMovementAdapter
    {
        private Transform2DMotor _motor;

        public void Initialize(LocomotionProfile profile)
        {
            // Profile can provide read-only hints if needed later; no blending here.
            _motor = GetComponent<Transform2DMotor>();
            if (_motor == null)
            {
                Debug.LogError($"{nameof(Transform2DAdapter)} requires {nameof(Transform2DMotor)}.");
            }
        }

        public void Apply(MovementInstruction instruction, float dt)
        {
            if (_motor == null) return;

            // Base velocity this tick (already accel/modifiers in the service)
            if (instruction.LocksCount <= 0)
            {
                _motor.SetBaseVelocity(instruction.Desired.x, instruction.Desired.y, dt);
            }
            else
            {
                _motor.SetBaseVelocity(0f, 0f, dt);
            }

            // One-tick impulse displacement (world units)
            if (instruction.Impulse.sqrMagnitude > 0f)
            {
                _motor.AddInstantDisplacement(instruction.Impulse.x, instruction.Impulse.y);
            }

            // Overlay displacement (world units, this tick)
            if (instruction.OverlayDelta.sqrMagnitude > 0f)
            {
                _motor.AddInstantDisplacement(instruction.OverlayDelta.x, instruction.OverlayDelta.y);
            }

            // Facing (optional)
            if (instruction.Face.sqrMagnitude > 0f)
            {
                _motor.SetFacing(instruction.Face.x, instruction.Face.y);
            }
        }
    }
}
