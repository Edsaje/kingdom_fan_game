using Godot;
using KingdomCore.Events;
using System;

namespace KingdomCore.Units
{
    public partial class BeaverBuilder : CharacterBody2D
    {
        public enum BuilderState
        {
            Idle,
            GoingToBuild,
            Building,
            Fleeing
        }

        [Export] public float Speed { get; private set; } = 50.0f;
        [Export] public int Health { get; private set; } = 1;

        private BuilderState _currentState = BuilderState.Idle;
        private float _gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

        private Sprite2D _sprite;
        private AnimationPlayer _animPlayer;

        public override void _Ready()
        {
            _sprite = GetNodeOrNull<Sprite2D>("Sprite2D");
            _animPlayer = GetNodeOrNull<AnimationPlayer>("AnimationPlayer");

            // RÈGLE 4 : Data-Driven (Lecture depuis units_config.json)
            var data = GameManager.Data?.GetUnit("unit_builder");
            if (data != null)
            {
                Speed = data.MovementSpeed;
                Health = data.Health;
            }

            GD.Print("🦫 Un nouveau Castor Bâtisseur est né et prêt à travailler !");
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
                case BuilderState.Idle:
                    velocity.X = 0; // Attend les ordres de construction
                    break;
                case BuilderState.GoingToBuild:
                    // TODO: Marcher vers le chantier
                    break;
                case BuilderState.Building:
                    velocity.X = 0;
                    // TODO: Animer la queue du castor qui tape sur la barricade
                    break;
            }

            Velocity = velocity;
            MoveAndSlide();
        }
    }
}
