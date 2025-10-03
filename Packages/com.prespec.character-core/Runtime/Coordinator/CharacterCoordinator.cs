// Copyright (c) Prespec.
// Licensed under the Apache License, Version 2.0. See LICENSE.md in the project root for license information.

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Prespec.CharacterCore.Contracts.Actions;
using Prespec.CharacterCore.Contracts.Input;
using Prespec.CharacterCore.Contracts.Movement;
using Prespec.CharacterCore.Contracts.Resources;

namespace Prespec.CharacterCore.Runtime.Coordinator
{
    /// <summary>
    /// Central orchestrator for a character. Gathers input in Update, advances modules and movement in FixedUpdate.
    /// Step 2 shell: composition discovery + input snapshot latching (no gates/plan/execute yet).
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class CharacterCoordinator : MonoBehaviour
    {
        [Header("Hosts (optional; must implement the interfaces)")]
        [SerializeField] private Component _inputReaderHost;    // IInputReader
        [SerializeField] private Component _movementHost;       // IMovementService
        [SerializeField] private bool _autoDiscover = true;     // Gather IActionModule + IResourceProvider on enable

        private IInputReader _inputReader;
        private IMovementService _movement;

        private readonly ResourceRegistry _directory = new ResourceRegistry();
        private readonly CoordinatorSignals _signals = new CoordinatorSignals();
        private readonly List<IActionModule> _actions = new List<IActionModule>(8);

        private InputSnapshot _latchedSnapshot;

        /// <summary>Resource registrations for this character.</summary>
        public ResourceRegistry Directory => _directory;

        /// <summary>Current action modules discovered on this character.</summary>
        public IReadOnlyList<IActionModule> Actions => _actions;

        /// <summary>Global coordinator signals (subscribe in code if needed).</summary>
        public CoordinatorSignals Signals => _signals;

        private void OnValidate()
        {
            // Keep serialized host fields honest (must implement required interfaces).
            if (_inputReaderHost != null && !(_inputReaderHost is IInputReader)) _inputReaderHost = null;
            if (_movementHost != null && !(_movementHost is IMovementService)) _movementHost = null;
        }

        private void Awake()
        {
            // Resolve hosts if assigned.
            _inputReader = _inputReaderHost as IInputReader;
            _movement    = _movementHost   as IMovementService;
        }

        private void OnEnable()
        {
            if (_autoDiscover)
                RefreshComposition();
        }

        private void OnDisable()
        {
            // Clear cached state; providers will re-register on next enable.
            _actions.Clear();
            _directory.Clear();
        }

        /// <summary>
        /// Re-scan this GameObject (and children) for IActionModule and IResourceProvider implementors,
        /// and (re)bind hosts if present. Allocation-light via UnityEngine.Pool.ListPool.
        /// </summary>
        public void RefreshComposition()
        {
            _actions.Clear();
            _directory.Clear();

            // Hosts: allow serialized assignment or discovery on this object.
            _inputReader = (_inputReaderHost != null) ? _inputReaderHost as IInputReader : GetComponent<IInputReader>();
            _movement    = (_movementHost    != null) ? _movementHost    as IMovementService  : GetComponent<IMovementService>();

            // Actions: collect from this object and children (common prefab pattern).
            GetComponentsInChildren(true, _actions);

            // Resources: rent a pooled list for scan, then register by ProvidedId.
            var list = ListPool<Component>.Get();
            try
            {
                GetComponentsInChildren(true, list);
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i] is IResourceProvider rp && !rp.ProvidedId.IsEmpty)
                        _directory.Register(rp);
                }
            }
            finally
            {
                ListPool<Component>.Release(list);
            }
        }

        private void Update()
        {
            // Produce the input snapshot in Update; consume in FixedUpdate for deterministic physics.
            if (_inputReader != null)
                _latchedSnapshot = _inputReader.CreateSnapshot();
        }

        private void FixedUpdate()
        {
            _signals.RaiseTickStarted();

            // Step 3+ will live here:
            // 1) Read _latchedSnapshot and gather action intents.
            // 2) Run gates and build plans on IActionModule(s).
            // 3) Arbitrate and Execute accepted plans.
            // 4) Apply movement intent/modifiers/overlays via _movement.
            // 5) Advance resource policies; update read models; emit signals.

            _signals.RaiseTickEnded();
        }

        /// <summary>Programmatic host injection for tests or custom setups.</summary>
        public void SetHosts(IInputReader inputReader, IMovementService movement)
        {
            _inputReader = inputReader;
            _movement = movement;
        }
    }
}
