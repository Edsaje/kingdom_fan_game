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
        
        // Bloque le ramassage par le Roi pendant 1 seconde (mais pas par l'esprit !)
        public bool CanBePickedUpByKing { get; private set; } = false;

        public override void _Ready()
        {
            float randomX = (float)GD.RandRange(-HorizontalSpread, HorizontalSpread);
            LinearVelocity = new Vector2(randomX, JumpForce);
            
            // On crée un petit chrono (Timer) d'une seconde directement en code
            GetTree().CreateTimer(1.0f).Timeout += () => 
            {
                // Le code dans ce bloc s'exécute après 1 seconde
                CanBePickedUpByKing = true; 
            };
        }
    }
}
