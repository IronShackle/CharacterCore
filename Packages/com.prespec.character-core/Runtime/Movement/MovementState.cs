// Runtime/Movement/MovementState.cs
using System;
using UnityEngine;

namespace Prespec.CharacterCore.Runtime.Movement
{
    [Serializable]
    public class MovementState
    {
        public Vector3 Velocity;
        public Vector3 Face;
        public bool OverlayActive;
        public int  LocksCount;
    }
}
