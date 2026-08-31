using Godot;
using KingdomCore.Events;
using System;

namespace KingdomCore.AI
{
    public partial class ForestSpirit : CharacterBody2D
    {
        // Notre Machine d'États (State Machine)
        public enum SpiritState
        {
            Wandering,      // Flâne autour du camp
            SeekingAmber,   // Court vers une pièce tombée
            SeekingTool,    // Court vers un râtelier d'outils disponible
            FleeingToCamp,  // Si l'animal meurt, l'esprit fuit vers le camp
            Recruited       // Attend qu'on lui donne un outil
        }

        [Export] public float Speed { get; set; } = 40.0f;
        [Export] public float WanderRadius { get; set; } = 150.0f;

        public Vector2 CampPosition { get; set; }
        private SpiritState _currentState = SpiritState.Wandering;
        
        // Gravité (Lue depuis les paramètres du projet Godot)
        private float _gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

        // Variables pour la logique de flânerie et de recherche
        private float _wanderTargetX;
        private float _idleTimer;
        private RandomNumberGenerator _rng = new RandomNumberGenerator();

        // Référence vers l'établi ciblé
        private Buildings.ToolStand _targetToolStand;

        public override void _Ready()
        {
            _rng.Randomize();
            CampPosition = GlobalPosition; 
            PickNewWanderTarget();

            // L'esprit écoute l'EventBus (Ambre et Outils)
            GameManager.Events.Subscribe<CoinDroppedEvent>(OnAmberDropped);
            GameManager.Events.Subscribe<ToolAvailableEvent>(OnToolAvailable);

            // On écoute le filet à papillon
            var pickupZone = GetNodeOrNull<Area2D>("PickupZone");
            if (pickupZone != null)
            {
                pickupZone.BodyEntered += OnPickupZoneBodyEntered;
            }
        }

        private void OnPickupZoneBodyEntered(Node2D body)
        {
            if (!Multiplayer.IsServer()) return;

            // Si l'esprit cherchait activement de l'ambre, ET que l'objet touché est une Pièce
            if (_currentState == SpiritState.SeekingAmber && body is KingdomCore.Items.Coin coin)
            {
                coin.QueueFree(); // L'esprit "mange" la pièce, elle disparaît
                _currentState = SpiritState.Recruited;
                
                // Petit effet visuel : On teinte l'esprit en bleu/doré pour montrer qu'il est recruté !
                Modulate = new Color(0.5f, 0.8f, 1.0f); 
                
                GD.Print("Esprit : Ambre absorbé ! Je suis maintenant un citoyen libre en attente d'outil !");
            }
        }

        public override void _ExitTree()
        {
            // Toujours nettoyer la mémoire quand un objet est détruit !
            GameManager.Events.Unsubscribe<CoinDroppedEvent>(OnAmberDropped);
            GameManager.Events.Unsubscribe<ToolAvailableEvent>(OnToolAvailable);
        }

        private void OnAmberDropped(CoinDroppedEvent ev)
        {
            if (_currentState == SpiritState.Recruited || _currentState == SpiritState.SeekingTool) return;

            // Si l'ambre tombe à moins de 300 pixels, l'esprit la repère !
            if (GlobalPosition.DistanceTo(ev.DropPosition) < 300.0f)
            {
                _currentState = SpiritState.SeekingAmber;
                _wanderTargetX = ev.DropPosition.X; // Sa nouvelle cible est la pièce
                GD.Print("Esprit : Ambre détecté, j'y cours !");
            }
        }

        private void OnToolAvailable(ToolAvailableEvent ev)
        {
            // Seuls les esprits recrutés sans emploi peuvent aller chercher un outil
            if (_currentState != SpiritState.Recruited) return;

            if (ev.StandSource is Buildings.ToolStand stand)
            {
                _targetToolStand = stand;
                _wanderTargetX = ev.Position.X;
                _currentState = SpiritState.SeekingTool;
                GD.Print($"Esprit : Outil ({ev.Type}) détecté à l'établi, je cours le récupérer !");
            }
        }

        public override void _PhysicsProcess(double delta)
        {
            if (!Multiplayer.IsServer()) return; 

            Vector2 velocity = Velocity;

            if (!IsOnFloor())
            {
                velocity.Y += _gravity * (float)delta;
            }

            switch (_currentState)
            {
                case SpiritState.Wandering:
                    velocity = ProcessWanderState(velocity, (float)delta);
                    break;
                case SpiritState.SeekingAmber:
                    velocity = ProcessSeekingState(velocity, (float)delta);
                    break;
                case SpiritState.SeekingTool:
                    velocity = ProcessSeekingToolState(velocity, (float)delta);
                    break;
                case SpiritState.FleeingToCamp:
                    break;
                case SpiritState.Recruited:
                    velocity.X = 0;
                    break;
            }

            Velocity = velocity;
            MoveAndSlide(); 
        }

        private Vector2 ProcessSeekingToolState(Vector2 velocity, float delta)
        {
            if (_targetToolStand == null)
            {
                _currentState = SpiritState.Recruited;
                return Vector2.Zero;
            }

            float direction = Mathf.Sign(_wanderTargetX - GlobalPosition.X);
            velocity.X = direction * Speed;

            // S'il est arrivé devant l'établi
            if (Mathf.Abs(GlobalPosition.X - _wanderTargetX) < 15.0f)
            {
                velocity.X = 0;
                if (_targetToolStand.TryClaimTool())
                {
                    PerformMetamorphosis(_targetToolStand.StandType);
                }
                else
                {
                    // L'outil a été pris par un autre esprit plus rapide
                    _currentState = SpiritState.Recruited;
                }
            }
            return velocity;
        }

        private void PerformMetamorphosis(ToolType tool)
        {
            GD.Print($"✨ MÉTAMORPHOSE ! L'esprit s'incarne grâce à l'outil : {tool}");

            PackedScene unitScene = null;
            if (tool == ToolType.Hammer)
            {
                unitScene = GD.Load<PackedScene>("res://Assets/Scenes/BeaverBuilder.tscn");
            }

            if (unitScene != null)
            {
                var unitInstance = unitScene.Instantiate<CharacterBody2D>();
                unitInstance.GlobalPosition = GlobalPosition;
                GetTree().CurrentScene.AddChild(unitInstance);
            }
            else
            {
                GD.PrintErr("Scène de l'unité introuvable, métamorphose simulée !");
            }

            // L'esprit disparaît pour laisser place à la créature physique
            QueueFree();
        }

        private Vector2 ProcessSeekingState(Vector2 velocity, float delta)
        {
            float direction = Mathf.Sign(_wanderTargetX - GlobalPosition.X);
            velocity.X = direction * (Speed * 1.5f); // Il court un peu plus vite pour l'ambre !

            // S'il est arrivé à la position théorique de l'ambre, il s'arrête et attend que la physique (PickupZone) fasse son travail.
            if (Mathf.Abs(GlobalPosition.X - _wanderTargetX) < 10.0f)
            {
                velocity.X = 0;
            }
            return velocity;
        }

        private Vector2 ProcessWanderState(Vector2 velocity, float delta)
        {
            // S'il est en pause, on décompte le temps
            if (_idleTimer > 0)
            {
                _idleTimer -= delta;
                velocity.X = 0;
                
                // Fin de la pause : on choisit une nouvelle destination
                if (_idleTimer <= 0) PickNewWanderTarget();
            }
            else
            {
                // Avancer vers la cible X
                float direction = Mathf.Sign(_wanderTargetX - GlobalPosition.X);
                velocity.X = direction * Speed;

                // Si on est presque arrivé à la cible, on s'arrête pour faire une pause
                if (Mathf.Abs(GlobalPosition.X - _wanderTargetX) < 5.0f)
                {
                    _idleTimer = _rng.RandfRange(1.0f, 4.0f); // S'arrête entre 1 et 4 secondes
                    velocity.X = 0;
                }
            }
            return velocity;
        }

        private void PickNewWanderTarget()
        {
            // Choisit un point X au hasard autour de son camp
            _wanderTargetX = CampPosition.X + _rng.RandfRange(-WanderRadius, WanderRadius);
        }
    }
}
