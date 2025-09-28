// Contracts/Movement/MoveIntent.cs
namespace Prespec.CharacterCore.Contracts.Movement
{
    public readonly struct MoveIntent
    {
        public readonly float X, Y, Z;
        public MoveIntent(float x, float y, float z = 0f) { X = x; Y = y; Z = z; }
        public static readonly MoveIntent Zero = new MoveIntent(0, 0, 0);
    }
}
