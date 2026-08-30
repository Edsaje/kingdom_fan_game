using Godot;

namespace KingdomCore.Items
{
    // RigidBody2D permet à l'objet d'avoir une vraie physique (gravité, rebonds, collisions)
    public partial class Coin : RigidBody2D
    {
        public int Value { get; set; } = 1;

        public override void _Ready()
        {
            // Au moment d'apparaître, on donne une petite impulsion physique à la pièce
            // pour qu'elle "saute" hors de la bourse du joueur avec un angle aléatoire, comme dans Kingdom.
            float randomX = (float)GD.RandRange(-50.0, 50.0);
            
            // LinearVelocity est le vecteur de vitesse. -200 sur Y la fait sauter vers le haut (l'axe Y est inversé).
            LinearVelocity = new Vector2(randomX, -200);
        }
    }
}
