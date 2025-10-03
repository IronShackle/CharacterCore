// Runtime/Movement/Links/Intent/MoveIntentLink.cs
using UnityEngine;
using Prespec.CharacterCore.Runtime.Movement.Pipeline;

namespace Prespec.CharacterCore.Runtime.Movement.Links.Intent
{
    public abstract class MoveIntentLink : ScriptableObject
    {
        public abstract void Process(ref Vector3 intent, in MovementContext context);
    }
}