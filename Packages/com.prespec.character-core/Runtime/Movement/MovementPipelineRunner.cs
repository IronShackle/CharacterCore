// Copyright (c) Prespec.
// Licensed under the Apache License, Version 2.0. See LICENSE.md in the project root for license information.

using System;
using UnityEngine;
using Prespec.CharacterCore.Contracts.Movement;
using Prespec.CharacterCore.Runtime.Movement.Adapters;
using Prespec.CharacterCore.Runtime.Movement.Internal;

namespace Prespec.CharacterCore.Runtime.Movement
{
    /// <summary>
    /// Main movement controller that orchestrates the movement pipeline through internal components.
    /// Acts as a facade for the movement system while delegating to specialized internal classes.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class MovementPipelineRunner : MonoBehaviour, IMovementService
    {
        [Header("Config")]
        [SerializeField] private LocomotionProfile _profile;

        [Header("Adapter")]
        [SerializeField] private MonoBehaviour _adapterBehaviour;
        private IMovementAdapter _adapter;

        // Internal system components
        private MovementStateTracker _stateTracker;
        private DisplacementManager _displacementManager;
        private MovementRequestRouter _requestRouter;
        private MovementProfileCompiler _profileCompiler;

        // Time provider for testing
        private Func<float> _fixedDtProvider = () => Time.fixedDeltaTime;

        // Read model for external access
        [SerializeField] private MovementState _state = new MovementState();

        private void Awake()
        {
            // Create and wire internal components
            _stateTracker = new MovementStateTracker();
            _displacementManager = new DisplacementManager();
            _requestRouter = new MovementRequestRouter(_stateTracker, _displacementManager);
            _profileCompiler = new MovementProfileCompiler();

            // Initialize adapter
            if (_adapterBehaviour == null || (_adapter = _adapterBehaviour as IMovementAdapter) == null)
                Debug.LogError($"{nameof(MovementPipelineRunner)} requires an IMovementAdapter.");
            else
                _adapter.Initialize(_profile);
        }

        private void FixedUpdate()
        {
            float deltaTime = _fixedDtProvider();

            // TODO: Replace with actual pipeline execution in Phase C
            // For now, maintain basic functionality using internal components
            ExecuteBasicMovement(deltaTime);
        }

        /// <summary>
        /// Temporary basic movement logic to maintain functionality while pipeline is being built.
        /// Will be replaced with proper block-based pipeline in Phase C.
        /// </summary>
        private void ExecuteBasicMovement(float deltaTime)
        {
            // Get current state
            Vector3 moveIntent = _stateTracker.MoveIntent;
            Vector3 currentVelocity = _stateTracker.CurrentVelocity;

            // Basic deadzone and movement processing
            Vector3 desiredVelocity = Vector3.zero;
            float intentMagnitude = moveIntent.magnitude;
            if (intentMagnitude >= _profile.inputDeadzone && !_stateTracker.IsLocked)
            {
                Vector3 direction = intentMagnitude > 0f ? (moveIntent / intentMagnitude) : Vector3.zero;
                desiredVelocity = direction * _profile.baseSpeed;
            }

            // Apply velocity modifiers
            float modifier = _stateTracker.GetCombinedVelocityMultiplier();
            desiredVelocity *= modifier;

            // Simple acceleration/deceleration
            float accelRate = desiredVelocity.magnitude > currentVelocity.magnitude ? 
                _profile.acceleration : _profile.deceleration;
            Vector3 newVelocity = Vector3.MoveTowards(currentVelocity, desiredVelocity, accelRate * deltaTime);

            // Get displacement overlays
            Vector3 overlayDelta = _displacementManager.TickOverlays(deltaTime);

            // Update facing if moving
            if (_profile.faceInMoveDirection && newVelocity.sqrMagnitude > 1e-6f)
                _stateTracker.SetFacing(newVelocity.normalized);

            // Update state
            _stateTracker.SetVelocity(newVelocity);

            // Create and apply movement command
            var command = new MovementInstruction(
                desired: newVelocity,
                impulse: Vector3.zero, // Impulses now handled through displacement system
                overlayDelta: overlayDelta,
                face: _stateTracker.Facing,
                locksCount: _stateTracker.LockCount
            );

            _adapter?.Apply(command, deltaTime);

            // Update read model
            _state.Velocity = newVelocity;
            _state.Face = _stateTracker.Facing;
            _state.OverlayActive = overlayDelta.sqrMagnitude > 1e-6f;
            _state.LocksCount = _stateTracker.LockCount;
        }

        #region IMovementService Implementation

        public void SetMoveIntent(in MoveIntent intent) => _requestRouter.SetMoveIntent(intent);
        public Guid RegisterVelocityModifier(in VelocityModifier modifier) => _requestRouter.RegisterVelocityModifier(modifier);
        public bool UnregisterVelocityModifier(Guid token) => _requestRouter.UnregisterVelocityModifier(token);
        public Guid RegisterDisplacementOverlay(in DisplacementOverlay overlay) => _requestRouter.RegisterDisplacementOverlay(overlay);
        public bool UnregisterDisplacementOverlay(Guid token) => _requestRouter.UnregisterDisplacementOverlay(token);
        public void AddImpulse(float dx, float dy, float dz) => _requestRouter.AddImpulse(dx, dy, dz);
        public void TakeLock(object key) => _requestRouter.TakeLock(key);
        public void ReleaseLock(object key) => _requestRouter.ReleaseLock(key);
        public void Face(float x, float y, float z) => _requestRouter.Face(x, y, z);

        #endregion

        #region Diagnostics and Testing

        public MovementState GetState() => _state;
        public void SetFixedDeltaProvider(Func<float> provider) => _fixedDtProvider = provider ?? (() => Time.fixedDeltaTime);

        #endregion
    }
}