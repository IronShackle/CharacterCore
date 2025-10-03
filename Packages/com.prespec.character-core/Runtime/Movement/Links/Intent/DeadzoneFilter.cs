// Runtime/Movement/Links/Intent/DeadzoneFilter.cs
using UnityEngine;
using Prespec.CharacterCore.Runtime.Movement.Pipeline;

namespace Prespec.CharacterCore.Runtime.Movement.Links.Intent
{
    /// <summary>
    /// Filters out input below the configured deadzone threshold.
    /// Prevents stick drift and unintended micro-movements.
    /// </summary>
    public sealed class DeadzoneFilter : MoveIntentLink
    {
        public void Process(ref Vector3 intent, in MovementContext context)
        {
            float magnitude = intent.magnitude;
            
            if (magnitude < context.Profile.inputDeadzone)
            {
                intent = Vector3.zero;
            }
        }
    }
}