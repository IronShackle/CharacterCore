// Runtime/Movement/Links/Facing/FaceMoveDirectionPolicy.cs
using UnityEngine;
using Prespec.CharacterCore.Runtime.Movement.Pipeline;

namespace Prespec.CharacterCore.Runtime.Movement.Links.Facing
{
    /// <summary>
    /// Sets facing direction to match movement direction when moving.
    /// </summary>
    public sealed class FaceMoveDirectionPolicy : FacingLink
    {
        public void Process(ref Vector3 facing, in MovementContext context, in Vector3 velocity, in Vector3 overlay)
        {
            if (!context.Profile.faceInMoveDirection)
                return;
                
            if (velocity.sqrMagnitude > 1e-6f)
            {
                facing = velocity.normalized;
            }
        }
    }
}