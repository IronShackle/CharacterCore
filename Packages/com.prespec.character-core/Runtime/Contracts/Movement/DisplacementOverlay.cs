// Contracts/Movement/DisplacementOverlay.cs
namespace Prespec.CharacterCore.Contracts.Movement
{
    /// <summary>
    /// World-space displacement applied over time (e.g., a dodge burst).
    /// Direction is normalized by the runtime; Distance is total world units; Duration in seconds.
    /// </summary>
    public readonly struct DisplacementOverlay
    {
        public readonly float DirectionX, DirectionY, DirectionZ;
        public readonly float Distance;
        public readonly float Duration;

        public DisplacementOverlay(float dirX, float dirY, float distance, float duration)
        {
            DirectionX = dirX; DirectionY = dirY; DirectionZ = 0f;
            Distance = distance; Duration = duration;
        }

        public DisplacementOverlay(float dirX, float dirY, float dirZ, float distance, float duration)
        {
            DirectionX = dirX; DirectionY = dirY; DirectionZ = dirZ;
            Distance = distance; Duration = duration;
        }
    }
}
