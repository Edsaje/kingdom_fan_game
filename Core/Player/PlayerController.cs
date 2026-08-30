using Godot;
using KingdomCore.Events;

namespace KingdomCore.Player
{
    public partial class PlayerController : Node2D 
    {
        [Export] 
        public float Speed { get; set; } = 150.0f;

        // Identifiant pour le Multi Local (1 = P1, 2 = P2)
        [Export]
        public int PlayerId { get; set; } = 1;

        private int _coinsInPouch = 5;
        
        // Touches par défaut (pour le P1)
        private string _inputLeft = "ui_left";
        private string _inputRight = "ui_right";
        private string _inputAction = "ui_accept";

        public override void _Ready()
        {
            // Préparation pour le Multijoueur Local : Séparation des touches
            if (PlayerId == 2)
            {
                // Tu devras créer ces Actions dans les paramètres de Godot
                _inputLeft = "p2_left";
                _inputRight = "p2_right";
                _inputAction = "p2_accept";
            }
        }

        public override void _Process(double delta)
        {
            // MULTIJOUEUR EN LIGNE : 
            // Si on n'est pas le propriétaire de ce personnage sur le réseau, 
            // on ne lit pas le clavier, on laisse le réseau bouger le perso !
            if (!IsMultiplayerAuthority()) return;

            HandleMovement((float)delta);
            HandleAction();
        }

        private void HandleMovement(float delta)
        {
            float direction = Input.GetAxis(_inputLeft, _inputRight); 
            Position += new Vector2(direction * Speed * delta, 0);
        }

        private void HandleAction()
        {
            if (Input.IsActionJustPressed(_inputAction))
            {
                // Plus tard, en ligne, ceci sera un appel RPC vers le Serveur
                DropCoin();
            }
        }

        private void DropCoin()
        {
            if (_coinsInPouch > 0)
            {
                _coinsInPouch--;
                GD.Print($"[Player {PlayerId}] Pièce jetée ! Reste {_coinsInPouch}.");
                GameManager.Events.Publish(new CoinDroppedEvent(Position.X, 1));
            }
            else
            {
                GD.Print($"[Player {PlayerId}] Plus de pièces !");
            }
        }
    }
}
