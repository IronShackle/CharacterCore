// Copyright (c) Prespec.
// Licensed under the Apache License, Version 2.0. See LICENSE.md in the project root for license information.

using System.Collections.Generic;
using Prespec.CharacterCore.Contracts.Core;
using Prespec.CharacterCore.Contracts.Resources;

namespace Prespec.CharacterCore.Runtime.Coordinator
{
    /// <summary>
    /// Case-insensitive registry of resource providers available to this character.
    /// Providers register/unregister at runtime; lookups are O(1).
    /// </summary>
    public sealed class CoordinatorDirectory
    {
        private readonly Dictionary<ResourceId, IResourceProvider> _providers =
            new Dictionary<ResourceId, IResourceProvider>();

        /// <summary>Register or replace the provider for its ResourceId.</summary>
        public void Register(IResourceProvider provider)
        {
            if (provider == null || provider.ProvidedId.IsEmpty) return;
            _providers[provider.ProvidedId] = provider;
        }

        /// <summary>Remove any provider registered for the given id.</summary>
        public bool Unregister(ResourceId id) => _providers.Remove(id);

        /// <summary>Try to get a provider by id.</summary>
        public bool TryGet(ResourceId id, out IResourceProvider provider) => _providers.TryGetValue(id, out provider);

        /// <summary>Remove all providers.</summary>
        public void Clear() => _providers.Clear();

        /// <summary>Number of registered providers.</summary>
        public int Count => _providers.Count;

        /// <summary>Enumerate current registrations (id → provider).</summary>
        public IEnumerable<KeyValuePair<ResourceId, IResourceProvider>> Enumerate() => _providers;
    }
}
