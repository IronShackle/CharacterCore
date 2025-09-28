// Copyright (c) Prespec.
// Licensed under the Apache License, Version 2.0. See LICENSE.md in the project root for license information.

using System;

namespace Prespec.CharacterCore.Runtime.Coordinator
{
    /// <summary>
    /// Optional global signals raised by the coordinator. Plain C# events keep GC minimal.
    /// </summary>
    public sealed class CoordinatorSignals
    {
        /// <summary>Raised at the start of FixedUpdate after the input snapshot is latched.</summary>
        public event Action TickStarted;

        /// <summary>Raised at the end of FixedUpdate after movement and modules have advanced.</summary>
        public event Action TickEnded;

        internal void RaiseTickStarted() => TickStarted?.Invoke();
        internal void RaiseTickEnded() => TickEnded?.Invoke();
    }
}
