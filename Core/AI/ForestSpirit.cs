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
            FleeingToCamp,  // Si l'animal meurt, l'esprit fuit vers le camp
            Recruited       // Attend qu'on lui donne un outil
        }

        [Export] public float Speed { get; set; } = 40.0f;
        [Export] public float WanderRadius { get; set; } = 150.0f;

        public Vector2 CampPosition { get; set; }
        private SpiritState _currentState = SpiritState.Wandering;
        
        // Gravité (Lue depuis les paramètres du projet Godot)
        private float _gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

        // Variables pour la logique de flânerie
        private float _wanderTargetX;
        private float _idleTimer;
        private RandomNumberGenerator _rng = new RandomNumberGenerator();

        public override void _Ready()
        {
            _rng.Randomize();
            CampPosition = GlobalPosition; // Par défaut, son camp est là où il spawn
            PickNewWanderTarget();
        }

        public override void _PhysicsProcess(double delta)
        {
            // RÈGLE 5 : Seul le Serveur a le droit de bouger les PNJ !
            if (!Multiplayer.IsServer()) return; 

            Vector2 velocity = Velocity;

            // Appliquer la gravité pour qu'il touche le sol
            if (!IsOnFloor())
            {
                velocity.Y += _gravity * (float)delta;
            }

            // Exécution de l'état actuel
            switch (_currentState)
            {
                case SpiritState.Wandering:
                    velocity = ProcessWanderState(velocity, (float)delta);
                    break;
                case SpiritState.SeekingAmber:
                    // TODO: L'esprit court vers l'ambre (Étape suivante)
                    break;
                case SpiritState.FleeingToCamp:
                    // TODO: L'esprit rentre au camp en pleurant
                    break;
            }

            Velocity = velocity;
            MoveAndSlide(); // Fonction Godot magique qui gère les collisions
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
