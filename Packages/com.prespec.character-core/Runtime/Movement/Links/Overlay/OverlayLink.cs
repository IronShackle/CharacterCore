// Runtime/Movement/Links/Overlay/OverlayLink.cs
using UnityEngine;
using Prespec.CharacterCore.Runtime.Movement.Pipeline;

namespace Prespec.CharacterCore.Runtime.Movement.Links.Overlay
{
    public abstract class OverlayLink : ScriptableObject
    {
        public abstract void Process(ref Vector3 overlayDelta, in MovementContext context, float deltaTime);
    }
}