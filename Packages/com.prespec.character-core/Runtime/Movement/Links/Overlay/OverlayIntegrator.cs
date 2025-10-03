// Runtime/Movement/Links/Overlay/OverlayIntegrator.cs
using UnityEngine;
using Prespec.CharacterCore.Runtime.Movement.Pipeline;

namespace Prespec.CharacterCore.Runtime.Movement.Links.Overlay
{
    /// <summary>
    /// Placeholder integrator for overlay stage. 
    /// Actual overlay delta is provided by DisplacementManager and applied by pipeline runner.
    /// </summary>
    public sealed class OverlayIntegrator : OverlayLink
    {
        public void Process(ref Vector3 overlayDelta, in MovementContext context, float deltaTime)
        {
            // Overlay delta is already calculated by DisplacementManager
            // This link exists to maintain pipeline stage consistency
            // Future: could apply overlay-specific modifications here if needed
        }
    }
}