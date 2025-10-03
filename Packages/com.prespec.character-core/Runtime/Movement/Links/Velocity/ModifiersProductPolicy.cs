// Runtime/Movement/Links/Velocity/ModifiersProductPolicy.cs
using UnityEngine;
using Prespec.CharacterCore.Runtime.Movement.Pipeline;

namespace Prespec.CharacterCore.Runtime.Movement.Links.Velocity
{
    /// <summary>
    /// Applies velocity multiplier from active modifiers (sprint, slow effects).
    /// </summary>
    public sealed class ModifiersProductPolicy : VelocityLink
    {
        public void Process(ref Vector3 velocity, in MovementContext context, float deltaTime)
        {
            velocity *= context.VelocityMultiplier;
        }
    }
}