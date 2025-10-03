// Runtime/Movement/Links/Facing/FacingLink.cs
using UnityEngine;
using Prespec.CharacterCore.Runtime.Movement.Pipeline;

namespace Prespec.CharacterCore.Runtime.Movement.Links.Facing
{
    public abstract class FacingLink : ScriptableObject
    {
        public abstract void Process(ref Vector3 facing, in MovementContext context, in Vector3 velocity, in Vector3 overlay);
    }
}