using Godot;
using KingdomCore.Events;

namespace KingdomCore.Player
{
    // Node2D car le joueur se déplace dans un espace 2D (il a des coordonnées X et Y)
    public partial class PlayerController : Node2D 
    {
        // L'attribut [Export] permet de modifier la vitesse directement depuis l'interface visuelle de Godot !
        [Export] 
        public float Speed { get; set; } = 150.0f;

        private int _coinsInPouch = 5;

        // _Process est appelé par le moteur à chaque frame
        public override void _Process(double delta)
        {
            HandleMovement((float)delta);
            HandleAction();
        }

        private void HandleMovement(float delta)
        {
            // Input.GetAxis renvoie -1 (gauche), 0 (rien), ou 1 (droite).
            // "ui_left" et "ui_right" sont des raccourcis par défaut dans Godot (Flèches directionnelles)
            float direction = Input.GetAxis("ui_left", "ui_right"); 
            
            // On déplace le personnage sur l'axe X (horizontal)
            Position += new Vector2(direction * Speed * delta, 0);
        }

        private void HandleAction()
        {
            // "ui_accept" est souvent la barre Espace ou la touche Entrée par défaut
            if (Input.IsActionJustPressed("ui_accept"))
            {
                DropCoin();
            }
        }

        private void DropCoin()
        {
            if (_coinsInPouch > 0)
            {
                _coinsInPouch--;
                GD.Print($"[Player] Pièce jetée ! Il reste {_coinsInPouch} pièces.");
                
                // C'est ici que la magie du découplage opère !
                // Le joueur ne cherche pas le sol ou un PNJ. Il annonce juste qu'une pièce est tombée.
                GameManager.Events.Publish(new CoinDroppedEvent(Position.X, 1));
            }
            else
            {
                GD.Print("[Player] Plus de pièces dans la bourse !");
            }
        }
    }
}
