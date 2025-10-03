// Runtime/Movement/Links/Velocity/AccelDecelPolicy.cs
using UnityEngine;
using Prespec.CharacterCore.Runtime.Movement.Pipeline;

namespace Prespec.CharacterCore.Runtime.Movement.Links.Velocity
{
    /// <summary>
    /// Applies acceleration or deceleration toward desired velocity.
    /// Uses profile acceleration when speeding up, deceleration when slowing down.
    /// </summary>
    public sealed class AccelDecelPolicy : VelocityLink
    {
        public void Process(ref Vector3 velocity, in MovementContext context, float deltaTime)
        {
            Vector3 desiredVelocity = context.MoveIntent * context.Profile.baseSpeed;
            
            float currentMag = velocity.magnitude;
            float targetMag = desiredVelocity.magnitude;
            
            float rate = targetMag > currentMag 
                ? context.Profile.acceleration 
                : context.Profile.deceleration;
            
            velocity = Vector3.MoveTowards(velocity, desiredVelocity, rate * deltaTime);
        }
    }
}