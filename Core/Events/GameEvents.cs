using Godot;

namespace KingdomCore.Events
{
    public enum ToolType
    {
        Hammer, // Bâtisseur (Castor)
        Bow,    // Archer / Chasseur (Hérisson)
        Scythe  // Fermier (Abeille)
    }

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

    public struct ToolAvailableEvent : IGameEvent
    {
        public ToolType Type { get; }
        public Vector2 Position { get; }
        public Node2D StandSource { get; }

        public ToolAvailableEvent(ToolType type, Vector2 position, Node2D standSource)
        {
            Type = type;
            Position = position;
            StandSource = standSource;
        }
    }

    public struct ToolClaimedEvent : IGameEvent
    {
        public ToolType Type { get; }
        public Node2D StandSource { get; }

        public ToolClaimedEvent(ToolType type, Node2D standSource)
        {
            Type = type;
            StandSource = standSource;
        }
    }
}
