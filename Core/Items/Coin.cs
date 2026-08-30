using Godot;

namespace KingdomCore.Items
{
    public partial class Coin : RigidBody2D
    {
        // RÈGLE 4 : Zéro Chiffre Magique ! Les variables [Export] permettent aux 
        // Game Designers de régler ces valeurs dans l'éditeur sans toucher au code.
        [Export]
        public float JumpForce { get; set; } = -200.0f;
        
        [Export]
        public float HorizontalSpread { get; set; } = 50.0f;

        public int Value { get; set; } = 1;

        public override void _Ready()
        {
            float randomX = (float)GD.RandRange(-HorizontalSpread, HorizontalSpread);
            LinearVelocity = new Vector2(randomX, JumpForce);
        }
    }
}
