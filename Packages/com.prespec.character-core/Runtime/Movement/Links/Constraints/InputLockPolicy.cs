// Runtime/Movement/Links/Constraints/InputLockPolicy.cs
using UnityEngine;
using Prespec.CharacterCore.Runtime.Movement.Pipeline;

namespace Prespec.CharacterCore.Runtime.Movement.Links.Constraints
{
    /// <summary>
    /// Zeros velocity when movement is locked by active actions.
    /// </summary>
    public sealed class InputLockPolicy : ConstraintLink
    {
        public void Process(ref Vector3 desiredVelocity, in MovementContext context)
        {
            if (context.IsLocked)
            {
                desiredVelocity = Vector3.zero;
            }
        }
    }
}