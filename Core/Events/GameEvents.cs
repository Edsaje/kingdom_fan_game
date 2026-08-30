using Godot;

namespace KingdomCore.Events
{
    public interface IGameEvent { }

    public struct CoinDroppedEvent : IGameEvent
    {
        public Vector2 DropPosition { get; }
        public int CoinValue { get; }

        public CoinDroppedEvent(Vector2 dropPosition, int coinValue)
        {
            DropPosition = dropPosition;
            CoinValue = coinValue;
        }
    }
}
