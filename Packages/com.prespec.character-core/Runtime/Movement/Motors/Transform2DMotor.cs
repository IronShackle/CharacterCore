// Runtime/Movement/Motors/Transform2DMotor.cs
// Copyright (c) Prespec.
// See LICENSE.md in the project root.

using UnityEngine;

namespace Prespec.CharacterCore.Runtime.Movement
{
    /// <summary>
    /// The only class in the Transform path that writes to Transform.
    /// Integrates base velocity using the provided dt and applies per-tick overlay displacements.
    /// Never reads Time.*. No blending or accel/decel here—that’s the service’s job.
    /// </summary>
    public sealed class Transform2DMotor : MonoBehaviour
    {
        // Base velocity resolved by the service (units/sec) for this tick.
        private float _vx, _vy;

        // Accumulated one-tick displacement (impulses, overlays) in world units.
        private float _ox, _oy;

        /// <summary>Set the base velocity (units/sec) for this tick.</summary>
        public void SetBaseVelocity(float vx, float vy, float dt)
        {
            // dt is provided to make the call uniform with 3D motors; we don't need it here.
            _vx = vx;
            _vy = vy;
        }

        /// <summary>Add an instant world-space displacement to apply this tick.</summary>
        public void AddInstantDisplacement(float dx, float dy)
        {
            _ox += dx;
            _oy += dy;
        }

        /// <summary>Optional: implement sprite rotation later. No-op for V0.1.</summary>
        public void SetFacing(float fx, float fy)
        {
            // Intentionally left empty in V0.1; facing handled by render/anim if desired.
        }

        /// <summary>Apply this tick's movement to the Transform, then clear one-tick offsets.</summary>
        public void Apply(float dt)
        {
            if (dt <= 0f) dt = 0f;

            // Base motion from velocity
            var p = transform.position;
            p.x += _vx * dt;
            p.y += _vy * dt;

            // Overlay / impulse displacement (already in world units)
            p.x += _ox;
            p.y += _oy;

            transform.position = p;

            // Clear one-tick displacement
            _ox = 0f;
            _oy = 0f;
        }
    }
}
