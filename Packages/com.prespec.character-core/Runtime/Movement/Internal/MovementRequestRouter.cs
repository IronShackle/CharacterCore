// Copyright (c) Prespec.
// Licensed under the Apache License, Version 2.0. See LICENSE.md in the project root for license information.

using System;
using UnityEngine;
using Prespec.CharacterCore.Contracts.Movement;

namespace Prespec.CharacterCore.Runtime.Movement.Internal
{
    /// <summary>
    /// Routes external movement API calls to the appropriate internal systems.
    /// Implements the IMovementService interface and delegates to StateTracker and DisplacementManager.
    /// </summary>
    internal class MovementRequestRouter
    {
        private readonly MovementStateTracker _stateTracker;
        private readonly DisplacementManager _displacementManager;

        public MovementRequestRouter(MovementStateTracker stateTracker, DisplacementManager displacementManager)
        {
            _stateTracker = stateTracker;
            _displacementManager = displacementManager;
        }

        public void SetMoveIntent(in MoveIntent intent)
        {
            _stateTracker.SetMoveIntent(new Vector3(intent.X, intent.Y, intent.Z));
        }

        public Guid RegisterVelocityModifier(in VelocityModifier modifier)
        {
            return _stateTracker.AddVelocityModifier(modifier);
        }

        public bool UnregisterVelocityModifier(Guid token)
        {
            return _stateTracker.RemoveVelocityModifier(token);
        }

        public Guid RegisterDisplacementOverlay(in DisplacementOverlay overlay)
        {
            return _displacementManager.RegisterOverlay(overlay);
        }

        public bool UnregisterDisplacementOverlay(Guid token)
        {
            return _displacementManager.UnregisterOverlay(token);
        }

        /// <summary>Applies instant displacement by creating a 0-duration overlay.</summary>
        public void AddImpulse(float dx, float dy, float dz)
        {
            var impulseOverlay = new DisplacementOverlay(dx, dy, dz, 
                distance: new Vector3(dx, dy, dz).magnitude, 
                duration: 0f);
            _displacementManager.RegisterOverlay(impulseOverlay);
        }

        public void TakeLock(object key)
        {
            _stateTracker.AddLock(key);
        }

        public void ReleaseLock(object key)
        {
            _stateTracker.RemoveLock(key);
        }

        public void Face(float x, float y, float z)
        {
            var direction = new Vector3(x, y, z);
            if (direction.sqrMagnitude > 1e-6f)
                _stateTracker.SetFacing(direction.normalized);
        }
    }
}