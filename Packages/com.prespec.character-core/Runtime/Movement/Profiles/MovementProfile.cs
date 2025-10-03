// Runtime/Movement/Profiles/MovementProfile.cs
using System.Collections.Generic;
using UnityEngine;
using Prespec.CharacterCore.Runtime.Movement.Links.Intent;
using Prespec.CharacterCore.Runtime.Movement.Links.Velocity;
using Prespec.CharacterCore.Runtime.Movement.Links.Overlay;
using Prespec.CharacterCore.Runtime.Movement.Links.Constraints;
using Prespec.CharacterCore.Runtime.Movement.Links.Facing;

namespace Prespec.CharacterCore.Runtime.Movement.Profiles
{
    /// <summary>
    /// Defines the complete movement pipeline through ordered link chains.
    /// Each stage's chain is executed sequentially during movement processing.
    /// </summary>
    [CreateAssetMenu(fileName = "MovementProfile", menuName = "Prespec/Character Core/Movement Profile")]
    public sealed class MovementProfile : ScriptableObject
    {
        [Header("Base Configuration")]
        [Tooltip("Locomotion settings like speed, acceleration, deadzone")]
        public LocomotionProfile locomotionProfile;

        [Header("Intent Chain")]
        [Tooltip("Filters and processes raw input")]
        public List<IMoveIntentLink> intentChain = new List<IMoveIntentLink>();

        [Header("Velocity Chain")]
        [Tooltip("Calculates desired velocity from processed intent")]
        public List<IVelocityLink> velocityChain = new List<IVelocityLink>();

        [Header("Overlay Chain")]
        [Tooltip("Integrates displacement overlays")]
        public List<IOverlayLink> overlayChain = new List<IOverlayLink>();

        [Header("Constraints Chain")]
        [Tooltip("Enforces movement restrictions")]
        public List<IConstraintLink> constraintsChain = new List<IConstraintLink>();

        [Header("Facing Chain")]
        [Tooltip("Determines character orientation")]
        public List<IFacingLink> facingChain = new List<IFacingLink>();
    }
}