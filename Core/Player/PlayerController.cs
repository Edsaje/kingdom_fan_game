using Godot;
using KingdomCore.Events;

namespace KingdomCore.Player
{
    public partial class PlayerController : CharacterBody2D 
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
        
        // Gravité du moteur
        private float _gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

        public override void _Ready()
        {
            _sprite = GetNode<Sprite2D>("Sprite2D");
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

            // On récupère le "filet à papillon" et on écoute quand quelque chose rentre dedans
            var pickupZone = GetNodeOrNull<Area2D>("PickupZone");
            if (pickupZone != null)
            {
                pickupZone.BodyEntered += OnPickupZoneBodyEntered;
            }
        }

        private void OnPickupZoneBodyEntered(Node2D body)
        {
            if (!Multiplayer.IsServer()) return; // Seul le serveur gère les ramassages

            // Si l'objet qui est entré est bien une Pièce (Coin)
            if (body is KingdomCore.Items.Coin coin)
            {
                // On vérifie le cooldown de 1 seconde
                if (coin.CanBePickedUpByKing)
                {
                    _coinsInPouch++;
                    GD.Print($"[Serveur] Le Hibou a ramassé un Ambre ! Total en poche : {_coinsInPouch}");
                    
                    // On supprime physiquement la pièce du jeu
                    coin.QueueFree(); 
                }
            }
        }

        // On passe de _Process à _PhysicsProcess car on gère de la gravité !
        public override void _PhysicsProcess(double delta)
        {
            if (!IsMultiplayerAuthority()) return;

            HandleMovement((float)delta);
            HandleAction();
        }

        private void HandleMovement(float delta)
        {
            Vector2 velocity = Velocity;

            // Application de la gravité
            if (!IsOnFloor())
            {
                velocity.Y += _gravity * delta;
            }

            float direction = Input.GetAxis(_inputLeft, _inputRight); 
            
            // On modifie la vélocité horizontale au lieu de se téléporter (Position += ...)
            velocity.X = direction * Speed;

            if (_sprite != null)
            {
                if (direction < 0) _sprite.FlipH = true;
                else if (direction > 0) _sprite.FlipH = false;
            }

            if (_animPlayer != null)
            {
                if (direction != 0)
                {
                    _animPlayer.Play("walk");
                }
                else
                {
                    _animPlayer.Play("RESET"); 
                }
            }

            // On applique la vélocité finale au moteur physique
            Velocity = velocity;
            MoveAndSlide();
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
