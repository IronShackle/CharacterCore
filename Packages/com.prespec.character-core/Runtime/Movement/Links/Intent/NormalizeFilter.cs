// Runtime/Movement/Links/Intent/NormalizeFilter.cs
using UnityEngine;
using Prespec.CharacterCore.Runtime.Movement.Pipeline;

namespace Prespec.CharacterCore.Runtime.Movement.Links.Intent
{
    /// <summary>
    /// Normalizes input vector to unit length.
    /// Ensures consistent directional input regardless of input magnitude.
    /// </summary>
    public sealed class NormalizeFilter : MoveIntentLink
    {
        public void Process(ref Vector3 intent, in MovementContext context)
        {
            float magnitude = intent.magnitude;
            
            if (magnitude > 1e-6f)
            {
                intent = intent / magnitude;
            }
        }
    }
}