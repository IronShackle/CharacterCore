// Runtime/Movement/Commands/MovementCommand.cs
using UnityEngine;

namespace Prespec.CharacterCore.Runtime.Movement
{
    /// <summary>Motor-agnostic command for one physics tick.</summary>
    public readonly struct MovementCommand
    {
        public readonly Vector3 Desired;     // base velocity after accel/modifiers (units/sec)
        public readonly Vector3 Impulse;     // one-tick displacement (world units)
        public readonly Vector3 OverlayDelta;// this-tick overlay displacement (world units)
        public readonly Vector3 Face;        // desired facing direction (unit or zero)
        public readonly int     LocksCount;
        public bool HasOverlay => OverlayDelta.sqrMagnitude > 0f || Impulse.sqrMagnitude > 0f;

        public MovementCommand(Vector3 desired, Vector3 impulse, Vector3 overlayDelta, Vector3 face, int locksCount)
        {
            Desired = desired; Impulse = impulse; OverlayDelta = overlayDelta; Face = face; LocksCount = locksCount;
        }
    }
}
