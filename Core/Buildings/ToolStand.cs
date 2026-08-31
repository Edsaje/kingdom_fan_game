using Godot;
using KingdomCore.Events;
using KingdomCore.Items;

namespace KingdomCore.Buildings
{
    public partial class ToolStand : Node2D
    {
        [Export] public ToolType StandType { get; set; } = ToolType.Hammer;
        [Export] public int ToolCost { get; set; } = 3; // Par défaut, 3 Ambre pour un marteau
        [Export] public int MaxTools { get; set; } = 4;

        private int _currentCoinsDeposited = 0;
        private int _availableToolsCount = 0;

        private Area2D _depositZone;

        public override void _Ready()
        {
            _depositZone = GetNodeOrNull<Area2D>("DepositZone");
            if (_depositZone != null)
            {
                _depositZone.BodyEntered += OnBodyEntered;
            }

            // Récupérer le coût configuré dans le DataManager si disponible (Règle 4 : Data-Driven)
            var builderData = GameManager.Data?.GetUnit("unit_builder");
            if (builderData != null && StandType == ToolType.Hammer)
            {
                ToolCost = builderData.CoinCost;
            }
        }

        private void OnBodyEntered(Node2D body)
        {
            if (!Multiplayer.IsServer()) return;

            // Si une pièce d'Ambre tombe dans l'entonnoir de l'établi
            if (body is Coin coin)
            {
                if (_availableToolsCount < MaxTools)
                {
                    _currentCoinsDeposited += coin.Value;
                    coin.QueueFree(); // Consomme la pièce
                    GD.Print($"[Établi {StandType}] Ambre reçu ({_currentCoinsDeposited}/{ToolCost})");

                    if (_currentCoinsDeposited >= ToolCost)
                    {
                        _currentCoinsDeposited -= ToolCost;
                        _availableToolsCount++;
                        GD.Print($"[Établi {StandType}] Outil fabriqué ! Total disponible : {_availableToolsCount}");
                        
                        // Notifie le royaume entier qu'un outil est prêt !
                        GameManager.Events.Publish(new ToolAvailableEvent(StandType, GlobalPosition, this));
                    }
                }
            }
        }

        // Appelé par un esprit quand il arrive à l'établi pour prendre son outil
        public bool TryClaimTool()
        {
            if (_availableToolsCount > 0)
            {
                _availableToolsCount--;
                GameManager.Events.Publish(new ToolClaimedEvent(StandType, this));
                return true;
            }
            return false;
        }

        public int GetAvailableToolsCount() => _availableToolsCount;
    }
}
