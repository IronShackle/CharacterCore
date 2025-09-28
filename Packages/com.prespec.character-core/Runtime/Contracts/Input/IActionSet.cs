// Copyright (c) Prespec.
// Licensed under the Apache License, Version 2.0. See LICENSE.md in the project root for license information.

using Prespec.CharacterCore.Contracts.Core;

namespace Prespec.CharacterCore.Contracts.Input
{
    /// <summary>
    /// Minimal, allocation-friendly membership surface for action lookups.
    /// Implementations can wrap pooled HashSets, arrays, or bitsets.
    /// </summary>
    public interface IActionSet
    {
        /// <summary>Returns true if the set contains <paramref name="id"/>.</summary>
        bool Contains(ActionId id);

        /// <summary>
        /// Optional: number of elements for diagnostics. Implementations may return 0 if unknown cheaply.
        /// Consumers must not rely on Count for logic.
        /// </summary>
        int Count { get; }
    }
}
