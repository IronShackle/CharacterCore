// Runtime/Movement/Links/Constraints/ConstraintLink.cs
using UnityEngine;
using Prespec.CharacterCore.Runtime.Movement.Pipeline;

namespace Prespec.CharacterCore.Runtime.Movement.Links.Constraints
{
    public abstract class ConstraintLink : ScriptableObject
    {
        public abstract void Process(ref Vector3 desiredVelocity, in MovementContext context);
    }
}