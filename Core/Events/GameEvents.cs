namespace KingdomCore.Events
{
    // Exemple d'événement lorsqu'une pièce est jetée et touche le sol
    public struct CoinDroppedEvent : IGameEvent
    {
        public float PositionX { get; }
        public int CoinValue { get; }

        public CoinDroppedEvent(float positionX, int coinValue)
        {
            PositionX = positionX;
            CoinValue = coinValue;
        }
    }
}
