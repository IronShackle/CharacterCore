// Copyright (c) Prespec.
// Licensed under the Apache License, Version 2.0. See LICENSE.md in the project root for license information.

// Runtime/Contracts/Core/ActionId.cs
using System;
using System.Diagnostics;

namespace Prespec.CharacterCore.Contracts.Core
{
    /// <summary>
    /// A small, case-sensitive identifier for gameplay actions (e.g., "Sprint", "Dodge").
    /// Stored as text; equality uses Ordinal comparison (case-sensitive).
    /// </summary>
    [Serializable]
    [DebuggerDisplay("{ToString(),nq}")]
    public readonly struct ActionId : IEquatable<ActionId>
    {
        /// <summary>Empty action id (no value).</summary>
        public static readonly ActionId Empty = new ActionId(null);

        private readonly string _value;

        /// <summary>
        /// Raw value as provided (trimmed). May be null/empty for <see cref="Empty"/>.
        /// </summary>
        public string Value => _value ?? string.Empty;

        /// <summary>True if null, empty, or whitespace.</summary>
        public bool IsEmpty => string.IsNullOrWhiteSpace(_value);

        /// <summary>Create a new ActionId. Trims whitespace; whitespace-only becomes Empty.</summary>
        public ActionId(string value)
        {
            var v = value?.Trim();
            _value = string.IsNullOrWhiteSpace(v) ? null : v;
        }

        /// <summary>Convenience: returns Empty for null/whitespace; otherwise new ActionId(value).</summary>
        public static ActionId CreateOrEmpty(string value) => new ActionId(value);

        public override string ToString() => _value ?? string.Empty;

        #region Equality
        public bool Equals(ActionId other) => string.Equals(_value, other._value, StringComparison.Ordinal);
        public override bool Equals(object obj) => obj is ActionId other && Equals(other);
        public override int GetHashCode() => _value == null ? 0 : StringComparer.Ordinal.GetHashCode(_value);
        public static bool operator ==(ActionId left, ActionId right) => left.Equals(right);
        public static bool operator !=(ActionId left, ActionId right) => !left.Equals(right);
        #endregion

        #region Conversions
        public static implicit operator ActionId(string value) => new ActionId(value);
        public static implicit operator string(ActionId id) => id.ToString();
        #endregion
    }
}
