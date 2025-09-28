// Copyright (c) Prespec.
// Licensed under the Apache License, Version 2.0. See LICENSE.md in the project root for license information.

// Runtime/Contracts/Core/ResourceId.cs
using System;
using System.Diagnostics;

namespace Prespec.CharacterCore.Contracts.Core
{
    /// <summary>
    /// A small, case-insensitive identifier for resource kinds (e.g., "Stamina", "Health").
    /// Equality compares using OrdinalIgnoreCase; original casing is preserved for display.
    /// </summary>
    [Serializable]
    [DebuggerDisplay("{ToString(),nq}")]
    public readonly struct ResourceId : IEquatable<ResourceId>
    {
        /// <summary>Empty resource id (no value).</summary>
        public static readonly ResourceId Empty = new ResourceId(null);

        private readonly string _original;    // For UI/logs (preserve user casing)
        private readonly string _normalized;  // For equality/hash (upper-invariant)

        /// <summary>Original text as provided (trimmed), or empty for <see cref="Empty"/>.</summary>
        public string Value => _original ?? string.Empty;

        /// <summary>True if null, empty, or whitespace.</summary>
        public bool IsEmpty => string.IsNullOrWhiteSpace(_original);

        /// <summary>Create a ResourceId. Trims whitespace; stores original + normalized forms.</summary>
        public ResourceId(string value)
        {
            var v = value?.Trim();
            if (string.IsNullOrWhiteSpace(v))
            {
                _original   = null;
                _normalized = null;
            }
            else
            {
                _original   = v;
                _normalized = v.ToUpperInvariant(); // normalization for case-insensitive compare
            }
        }

        /// <summary>Convenience: returns Empty for null/whitespace; otherwise new ResourceId(value).</summary>
        public static ResourceId CreateOrEmpty(string value) => new ResourceId(value);

        public override string ToString() => _original ?? string.Empty;

        #region Equality (case-insensitive)
        public bool Equals(ResourceId other)
            => string.Equals(_normalized, other._normalized, StringComparison.Ordinal);

        public override bool Equals(object obj)
            => obj is ResourceId other && Equals(other);

        public override int GetHashCode()
            => _normalized == null ? 0 : StringComparer.Ordinal.GetHashCode(_normalized);

        public static bool operator ==(ResourceId left, ResourceId right) => left.Equals(right);
        public static bool operator !=(ResourceId left, ResourceId right) => !left.Equals(right);
        #endregion

        #region Conversions
        public static implicit operator ResourceId(string value) => new ResourceId(value);
        public static implicit operator string(ResourceId id) => id.ToString(); // prefer original for UI
        #endregion
    }
}
