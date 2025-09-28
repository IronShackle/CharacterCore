// Copyright (c) Prespec.
// Licensed under the Apache License, Version 2.0. See LICENSE.md in the project root for license information.

// Runtime/Contracts/Core/ActionContext.cs
using System;
using System.Diagnostics;

namespace Prespec.CharacterCore.Contracts.Core
{
    /// <summary>
    /// Lightweight, implementation-agnostic payload describing how an action should run this time.
    /// No Unity types here; keep it serializable and portable across systems.
    /// </summary>
    [Serializable]
    [DebuggerDisplay("{ToString(),nq}")]
    public readonly struct ActionContext : IEquatable<ActionContext>
    {
        // Optional directional input (normalized or raw, caller decides).
        public float? DirX { get; }
        public float? DirY { get; }

        // Optional scalar (e.g., charge amount, strength multiplier).
        public float? Scalar { get; }

        // Optional slot index (e.g., hotbar slot).
        public int? Slot { get; }

        public bool HasDirection => DirX.HasValue && DirY.HasValue;
        public bool HasScalar    => Scalar.HasValue;
        public bool HasSlot      => Slot.HasValue;

        public float? DirectionMagnitude
            => HasDirection ? MathF.Sqrt((DirX!.Value * DirX.Value) + (DirY!.Value * DirY.Value)) : (float?)null;

        public ActionContext(float? dirX, float? dirY, float? scalar, int? slot)
        {
            DirX   = dirX;
            DirY   = dirY;
            Scalar = scalar;
            Slot   = slot;
        }

        /// <summary>Create a context with only direction (and optional scalar).</summary>
        public static ActionContext FromDirection(float x, float y, float? scalar = null)
            => new ActionContext(x, y, scalar, null);

        /// <summary>Create a context with only slot.</summary>
        public static ActionContext FromSlot(int slot)
            => new ActionContext(null, null, null, slot);

        /// <summary>Try to read direction; returns false if no direction present.</summary>
        public bool TryGetDirection(out float x, out float y)
        {
            if (HasDirection)
            {
                x = DirX!.Value;
                y = DirY!.Value;
                return true;
            }
            x = y = 0f;
            return false;
        }

        public override string ToString()
        {
            if (HasDirection && HasScalar)
                return $"Dir=({DirX:0.###},{DirY:0.###}) Scalar={Scalar:0.###}";
            if (HasDirection)
                return $"Dir=({DirX:0.###},{DirY:0.###})";
            if (HasSlot)
                return $"Slot={Slot}";
            if (HasScalar)
                return $"Scalar={Scalar:0.###}";
            return "(empty)";
        }

        #region Equality
        public bool Equals(ActionContext other)
            => DirX.Equals(other.DirX)
            && DirY.Equals(other.DirY)
            && Scalar.Equals(other.Scalar)
            && Slot.Equals(other.Slot);

        public override bool Equals(object obj) => obj is ActionContext other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = (hash * 31) + DirX.GetHashCode();
                hash = (hash * 31) + DirY.GetHashCode();
                hash = (hash * 31) + Scalar.GetHashCode();
                hash = (hash * 31) + Slot.GetHashCode();
                return hash;
            }
        }

        public static bool operator ==(ActionContext a, ActionContext b) => a.Equals(b);
        public static bool operator !=(ActionContext a, ActionContext b) => !a.Equals(b);
        #endregion
    }
}
