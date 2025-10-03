// Copyright (c) Prespec.
// Licensed under the Apache License, Version 2.0. See LICENSE.md in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;
using Prespec.CharacterCore.Contracts.Movement;

namespace Prespec.CharacterCore.Runtime.Movement.Internal
{
    /// <summary>
    /// Centralized state storage for the movement system. Acts as the single source of truth
    /// for movement state that persists between pipeline executions.
    /// </summary>
    internal class MovementStateTracker
    {
        private Vector3 _currentVelocity;
        private Vector3 _facing = Vector3.right;  // Default facing right for 2D top-down
        private Vector3 _moveIntent;
        
        private readonly Dictionary<Guid, VelocityModifier> _velocityModifiers = new();
        private readonly HashSet<object> _movementLocks = new();

        // Read-only state access
        public Vector3 CurrentVelocity => _currentVelocity;
        public Vector3 Facing => _facing;
        public Vector3 MoveIntent => _moveIntent;
        public int LockCount => _movementLocks.Count;
        public bool IsLocked => _movementLocks.Count > 0;

        // State mutations
        public void SetVelocity(Vector3 velocity) => _currentVelocity = velocity;
        public void SetFacing(Vector3 facing) => _facing = facing;
        public void SetMoveIntent(Vector3 intent) => _moveIntent = intent;

        public Guid AddVelocityModifier(VelocityModifier modifier)
        {
            var id = Guid.NewGuid();
            _velocityModifiers[id] = modifier;
            return id;
        }

        public bool RemoveVelocityModifier(Guid id) => _velocityModifiers.Remove(id);

        /// <summary>
        /// Calculate combined multiplier from all active modifiers. 
        /// Multipliers are combined multiplicatively and clamped to prevent negative values.
        /// </summary>
        public float GetCombinedVelocityMultiplier()
        {
            float combined = 1f;
            foreach (var modifier in _velocityModifiers.Values)
                combined *= Mathf.Max(0f, modifier.Multiplier);
            return combined;
        }

        /// <summary>Uses key to ensure only the system that took the lock can release it.</summary>
        public void AddLock(object key) => _movementLocks.Add(key ?? this);
        
        public void RemoveLock(object key) => _movementLocks.Remove(key ?? this);
    }
}