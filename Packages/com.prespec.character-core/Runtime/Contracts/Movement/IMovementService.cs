// Contracts/Movement/IMovementService.cs
using System;

namespace Prespec.CharacterCore.Contracts.Movement
{
    public interface IMovementService
    {
        void SetMoveIntent(in MoveIntent intent);

        Guid RegisterVelocityModifier(in VelocityModifier modifier);
        bool UnregisterVelocityModifier(Guid token);

        Guid RegisterDisplacementOverlay(in DisplacementOverlay overlay);
        bool UnregisterDisplacementOverlay(Guid token);

        void AddImpulse(float dx, float dy, float dz);

        void TakeLock(object key);
        void ReleaseLock(object key);

        void Face(float x, float y, float z);
    }
}
