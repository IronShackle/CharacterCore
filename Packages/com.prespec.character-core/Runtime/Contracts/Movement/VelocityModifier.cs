// Copyright (c) Prespec.
// Licensed under the Apache License, Version 2.0. See LICENSE.md in the project root for license information.

namespace Prespec.CharacterCore.Contracts.Movement
{
    /// <summary>
    /// Scales/offsets the resolved velocity while active (e.g., sprint multiplier).
    /// Exact composition (additive/multiplicative) is adapter/policy-defined.
    /// </summary>
    public readonly struct VelocityModifier
    {
        /// <summary>Multiplicative factor (1 = no change).</summary>
        public readonly float Multiplier;

        /// <summary>Optional additive offset applied after scaling.</summary>
        public readonly float Add;

        public VelocityModifier(float multiplier, float add = 0f)
        {
            Multiplier = multiplier;
            Add = add;
        }
    }
}
