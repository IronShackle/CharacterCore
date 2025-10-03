// Copyright (c) Prespec.
// Licensed under the Apache License, Version 2.0. See LICENSE.md in the project root for license information.

using System;
using System.Collections.Generic;
using Prespec.CharacterCore.Contracts.Core;

namespace Prespec.CharacterCore.Contracts.Actions
{
    /// <summary>
    /// Generic action module contract (sprint, dodge, etc.). Modules plan and then execute.
    /// Plans are stored internally and referenced by a token to keep execution generic and allocation-free.
    /// </summary>
    public interface IActionModule
    {
        /// <summary>Action ids this module can handle.</summary>
        IEnumerable<ActionId> GetSupportedActions();

        /// <summary>
        /// Attempt to plan the specified action using the provided context.
        /// Returns true and a decision if the action can proceed this tick.
        /// The module must retain sufficient plan data internally, keyed by the returned token.
        /// </summary>
        bool TryPlan(ActionId id, in ActionContext context, out ActionExecutionPlan decision);

        /// <summary>
        /// Execute a previously planned action. The decision's token resolves the stored plan.
        /// Implementations should clear or recycle plan data after execution.
        /// </summary>
        void Execute(in ActionExecutionPlan decision);
    }

    /// <summary>
    /// A lightweight reference to a stored plan plus an arbitration hint.
    /// The token is opaque outside the module; the coordinator only uses it to call Execute.
    /// </summary>
    public readonly struct ActionExecutionPlan
    {
        public readonly ActionId ActionId;
        public readonly int Priority;
        public readonly Guid PlanToken;

        public ActionExecutionPlan(ActionId actionId, int priority, Guid planToken)
        {
            ActionId = actionId;
            Priority = priority;
            PlanToken = planToken;
        }
    }
}
