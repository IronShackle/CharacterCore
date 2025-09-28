using Prespec.CharacterCore.Runtime.Movement;

namespace Prespec.CharacterCore.Runtime.Movement.Adapters
{
    /// <summary>
    /// Translator per motor type. Implemented by Transform2DAdapter, Rigidbody2DAdapter, etc.
    /// Receives the already-blended command; MUST NOT do blending/accel/overlay scaling.
    /// </summary>
    public interface IMovementAdapter2D
    {
        /// <summary>Adapters may read profile hints (read-only).</summary>
        void Initialize(LocomotionProfile profile);

        /// <summary>Apply one physics tick.</summary>
        void Apply(MovementCommand command, float dt);
    }
}
