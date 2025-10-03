// Runtime/Movement/Links/Velocity/VelocityLink.cs
using UnityEngine;
using Prespec.CharacterCore.Runtime.Movement.Pipeline;

namespace Prespec.CharacterCore.Runtime.Movement.Links.Velocity
{
    public abstract class VelocityLink : ScriptableObject
    {
        public abstract void Process(ref Vector3 velocity, in MovementContext context, float deltaTime);
    }
}