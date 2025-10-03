// Runtime/Movement/Pipeline/MovementContext.cs
using UnityEngine;

namespace Prespec.CharacterCore.Runtime.Movement.Pipeline
{
    /// <summary>
    /// Read-only context data passed to links during pipeline execution.
    /// Provides access to current state without exposing internal mutability.
    /// </summary>
    public readonly struct MovementContext
    {
        // Current state
        public readonly Vector3 CurrentVelocity;
        public readonly Vector3 CurrentFacing;
        public readonly Vector3 MoveIntent;
        
        // Movement modifiers
        public readonly float VelocityMultiplier;
        
        // State flags
        public readonly bool IsLocked;
        public readonly int LockCount;
        
        // Profile reference for links that need configuration values
        public readonly LocomotionProfile Profile;

        public MovementContext(
            Vector3 currentVelocity,
            Vector3 currentFacing,
            Vector3 moveIntent,
            float velocityMultiplier,
            bool isLocked,
            int lockCount,
            LocomotionProfile profile)
        {
            CurrentVelocity = currentVelocity;
            CurrentFacing = currentFacing;
            MoveIntent = moveIntent;
            VelocityMultiplier = velocityMultiplier;
            IsLocked = isLocked;
            LockCount = lockCount;
            Profile = profile;
        }
    }
}