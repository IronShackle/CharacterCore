// Copyright (c) Prespec.
// Licensed under the Apache License, Version 2.0. See LICENSE.md in the project root for license information.

// Runtime/Contracts/Resources/IResourceProvider.cs
using System;
using Prespec.CharacterCore.Contracts.Core;

namespace Prespec.CharacterCore.Contracts.Resources
{
    /// <summary>Generic resource interface (e.g., Stamina). Actions query and spend via this contract.</summary>
    public interface IResourceProvider
    {
        /// <summary>The resource type identifier (e.g., "Stamina").</summary>
        ResourceId ProvidedId { get; }

        /// <summary>Current value and maximum.</summary>
        float Current { get; }
        float Max { get; }

        /// <summary>True if the resource has at least the requested amount available now.</summary>
        bool CanAfford(float amount);

        /// <summary>Attempt to spend an immediate amount. Returns false if insufficient.</summary>
        bool TrySpend(float amount);

        /// <summary>Begin a continuous drain (amount per second). Returns a token to end the drain.</summary>
        Guid BeginPerSecondDrain(float amountPerSecond);

        /// <summary>End a previously started per-second drain.</summary>
        bool EndPerSecondDrain(Guid token);
    }
}
