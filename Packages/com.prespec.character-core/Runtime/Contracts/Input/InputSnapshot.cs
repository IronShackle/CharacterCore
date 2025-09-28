// Copyright (c) Prespec.
// Licensed under the Apache License, Version 2.0. See LICENSE.md in the project root for license information.

using Prespec.CharacterCore.Contracts.Core;

namespace Prespec.CharacterCore.Contracts.Input
{
    /// <summary>
    /// Immutable inputs for one physics tick.
    /// Implementations may pool underlying action sets; consumers treat them as read-only.
    /// </summary>
    public readonly struct InputSnapshot
    {
        public readonly float MoveX;
        public readonly float MoveY;

        private readonly IActionSet _pressed;
        private readonly IActionSet _released;
        private readonly IActionSet _held;

        public InputSnapshot(float moveX, float moveY,
            IActionSet pressed,
            IActionSet released,
            IActionSet held)
        {
            MoveX = moveX;
            MoveY = moveY;
            _pressed = pressed;
            _released = released;
            _held = held;
        }

        public bool WasPressed(ActionId id)  => _pressed  != null && _pressed.Contains(id);
        public bool WasReleased(ActionId id) => _released != null && _released.Contains(id);
        public bool IsHeld(ActionId id)      => _held     != null && _held.Contains(id);
    }
}
