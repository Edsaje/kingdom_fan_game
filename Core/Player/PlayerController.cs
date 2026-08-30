using Godot;
using KingdomCore.Events;

namespace KingdomCore.Player
{
    public partial class PlayerController : Node2D 
    {
        // RÈGLE 4 : Plus de chiffre magique hardcodé. Initialisé par le JSON.
        [Export] 
        public float Speed { get; private set; }

        [Export]
        public int PlayerId { get; set; } = 1;

        private int _coinsInPouch = 5;
        
        private string _inputLeft = "ui_left";
        private string _inputRight = "ui_right";
        private string _inputAction = "ui_accept";

        private Sprite2D _sprite;
        private AnimationPlayer _animPlayer;

        public override void _Ready()
        {
            _sprite = GetNode<Sprite2D>("Sprite2D");
            
            // On tente de récupérer l'AnimationPlayer s'il existe
            _animPlayer = GetNodeOrNull<AnimationPlayer>("AnimationPlayer");

            var kingData = GameManager.Data.GetUnit("unit_king");
            if (kingData != null)
            {
                Speed = kingData.MovementSpeed;
            }
            else
            {
                GD.PrintErr("Données du Roi introuvables dans le JSON !");
                Speed = 150.0f;
            }

            if (PlayerId == 2)
            {
                _inputLeft = "p2_left";
                _inputRight = "p2_right";
                _inputAction = "p2_accept";
            }
        }

        public override void _Process(double delta)
        {
            if (!IsMultiplayerAuthority()) return;

            HandleMovement((float)delta);
            HandleAction();
        }

        private void HandleMovement(float delta)
        {
            float direction = Input.GetAxis(_inputLeft, _inputRight); 
            Position += new Vector2(direction * Speed * delta, 0);

            if (_sprite != null)
            {
                if (direction < 0) _sprite.FlipH = true;
                else if (direction > 0) _sprite.FlipH = false;
            }

            // Gestion de l'animation
            if (_animPlayer != null)
            {
                if (direction != 0)
                {
                    _animPlayer.Play("walk"); // Joue l'animation (remplace par le nom de ton animation si différent)
                }
                else
                {
                    // Arrête l'animation et remet à la frame de base si on ne bouge pas
                    _animPlayer.Play("RESET"); 
                }
            }
        }

        private void HandleAction()
        {
            if (Input.IsActionJustPressed(_inputAction))
            {
                // RÈGLE 5 : Architecture Serveur Autoritaire.
                // On n'exécute plus l'action directement. On Demande au Serveur.
                Rpc(nameof(DropCoinRpc));
            }
        }

        // CallLocal = true permet au joueur hôte de s'envoyer le message à lui-même
        [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true)]
        private void DropCoinRpc()
        {
            // RÈGLE 5 : Seul le Serveur (autorité suprême) a le droit de dépenser l'argent.
            if (!Multiplayer.IsServer()) return;

            if (_coinsInPouch > 0)
            {
                _coinsInPouch--;
                GD.Print($"[Serveur] Éclat d'ambre jeté par Joueur {PlayerId} ! Reste {_coinsInPouch}.");
                GameManager.Events.Publish(new CoinDroppedEvent(Position, 1));
            }
            else
            {
                GD.Print($"[Serveur] Le joueur {PlayerId} n'a plus de monnaie !");
            }
        }
    }
}
