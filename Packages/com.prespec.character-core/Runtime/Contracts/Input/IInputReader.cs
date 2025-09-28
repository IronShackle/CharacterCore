// Copyright (c) Prespec.
// Licensed under the Apache License, Version 2.0. See LICENSE.md in the project root for license information.

using Prespec.CharacterCore.Contracts.Core;

namespace Prespec.CharacterCore.Contracts.Input
{
    /// <summary>
    /// Adapter interface for the current player (or AI) inputs.
    /// Produces a per-physics-tick snapshot consumed by the Coordinator.
    /// </summary>
    public interface IInputReader
    {
        /// <summary>Create an immutable snapshot of inputs for the next physics tick.</summary>
        InputSnapshot CreateSnapshot();

        /// <summary>Current move axis data in Update (for preview/debug). Physics uses the snapshot.</summary>
        (float x, float y) PeekMoveAxes();
    }
}
