// Copyright (c) Prespec.
// Licensed under the Apache License, Version 2.0. See LICENSE.md in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;
using Prespec.CharacterCore.Contracts.Movement;

namespace Prespec.CharacterCore.Runtime.Movement.Internal
{
    /// <summary>
    /// Manages displacement overlays and impulses over time. Handles registration, tick updates,
    /// and cleanup of temporary movement effects that override normal input.
    /// </summary>
    internal class DisplacementManager
    {
        private struct OverlayEntry
        {
            public Guid Id;
            public Vector3 PerSecondVelocity;
            public float RemainingDuration;
        }

        private readonly List<OverlayEntry> _activeOverlays = new();
        private readonly List<Guid> _overlaysToRemove = new();

        public Guid RegisterOverlay(in DisplacementOverlay overlay)
        {
            var id = Guid.NewGuid();
            var perSecondVelocity = CalculatePerSecondVelocity(
                overlay.DirectionX, overlay.DirectionY, overlay.DirectionZ, 
                overlay.Distance, overlay.Duration);
            
            _activeOverlays.Add(new OverlayEntry
            {
                Id = id,
                PerSecondVelocity = perSecondVelocity,
                RemainingDuration = Mathf.Max(0f, overlay.Duration)
            });
            
            return id;
        }

        public bool UnregisterOverlay(Guid token)
        {
            int index = _activeOverlays.FindIndex(entry => entry.Id == token);
            if (index < 0) return false;
            
            _activeOverlays.RemoveAt(index);
            return true;
        }

        /// <summary>
        /// Update all active overlays and return the combined displacement for this frame.
        /// Automatically removes completed overlays.
        /// </summary>
        public Vector3 TickOverlays(float deltaTime)
        {
            if (_activeOverlays.Count == 0) return Vector3.zero;

            Vector3 totalDisplacement = Vector3.zero;
            _overlaysToRemove.Clear();

            for (int i = 0; i < _activeOverlays.Count; i++)
            {
                var overlay = _activeOverlays[i];
                
                // Handle 0-duration overlays (impulses) - apply full displacement immediately
                if (overlay.RemainingDuration <= 0f)
                {
                    totalDisplacement += overlay.PerSecondVelocity;
                    _overlaysToRemove.Add(overlay.Id);
                    continue;
                }

                // Handle timed overlays - apply portion based on remaining time
                float timeStep = Mathf.Min(overlay.RemainingDuration, deltaTime);
                totalDisplacement += overlay.PerSecondVelocity * timeStep;
                
                overlay.RemainingDuration -= timeStep;
                _activeOverlays[i] = overlay;
                
                if (overlay.RemainingDuration <= 0f)
                    _overlaysToRemove.Add(overlay.Id);
            }

            // Remove completed overlays
            foreach (var id in _overlaysToRemove)
                UnregisterOverlay(id);

            return totalDisplacement;
        }

        /// <summary>Convert direction + distance + duration into a per-second velocity vector.</summary>
        private static Vector3 CalculatePerSecondVelocity(float dirX, float dirY, float dirZ, float distance, float duration)
        {
            var direction = new Vector3(dirX, dirY, dirZ);
            var normalizedDirection = direction.sqrMagnitude > 1e-6f ? direction.normalized : Vector3.zero;
            
            // For 0-duration (impulses), velocity represents total displacement
            if (duration <= 0f)
                return normalizedDirection * distance;
            
            // For timed overlays, velocity is distance/time
            return normalizedDirection * (distance / duration);
        }
    }
}