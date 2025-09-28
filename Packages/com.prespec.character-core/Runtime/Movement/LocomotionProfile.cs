// Copyright (c) Prespec.
// Licensed under the Apache License, Version 2.0. See LICENSE.md in the project root for license information.

using UnityEngine;

namespace Prespec.CharacterCore.Runtime.Movement
{
    /// <summary>
    /// Tunable locomotion settings (data-only). The adapter enforces these values.
    /// </summary>
    [CreateAssetMenu(fileName = "LocomotionProfile", menuName = "Prespec/Character Core/Locomotion Profile", order = 0)]
    public sealed class LocomotionProfile : ScriptableObject
    {
        [Header("Speeds")]
        [Tooltip("Base movement speed in units/second.")]
        public float baseSpeed = 4.0f;

        [Tooltip("Acceleration toward target speed in units/second^2.")]
        public float acceleration = 40.0f;

        [Tooltip("Deceleration when input reduces speed in units/second^2.")]
        public float deceleration = 50.0f;

        [Header("Input Shaping")]
        [Tooltip("Axial deadzone; input magnitude below this is treated as zero.")]
        [Range(0f, 0.5f)] public float inputDeadzone = 0.1f;

        [Tooltip("If true, snap facing to last non-zero move direction.")]
        public bool faceInMoveDirection = true;

        [Header("Reserved (V0.1 scope)")]
        [Tooltip("Reserved for collision options (not active in V0.1).")]
        public bool collisionsReserved;
    }
}
