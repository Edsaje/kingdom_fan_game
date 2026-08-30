using Godot;
using KingdomCore.Events;
using KingdomCore.Data;
using System;

namespace KingdomCore
{
    public partial class GameManager : Node
    {
        public static EventBus Events { get; private set; }
        public static DataManager Data { get; private set; }

        // Référence au modèle (Blueprint) de la scène de la pièce
        private PackedScene _coinScene;

        public override void _Ready()
        {
            Events = new EventBus();
            Data = new DataManager();

            Data.LoadUnitData("res://Data/Configs/units_config.json");
            
            // On charge le Blueprint de la pièce en mémoire.
            // Note: Si Godot ne trouve pas le fichier lors du _Ready, il mettra null.
            _coinScene = GD.Load<PackedScene>("res://Assets/Scenes/Coin.tscn");

            // On abonne le GameManager à l'événement
            Events.Subscribe<CoinDroppedEvent>(OnCoinDropped);
            
            GD.Print("GameManager initialisé avec succès ! En attente d'événements...");
        }

        // Il faut TOUJOURS se désabonner d'un EventBus quand on est détruit pour éviter les Memory Leaks
        public override void _ExitTree()
        {
            Events.Unsubscribe<CoinDroppedEvent>(OnCoinDropped);
        }

        private void OnCoinDropped(CoinDroppedEvent ev)
        {
            if (_coinScene == null)
            {
                GD.PrintErr("Impossible de faire spawner la pièce : Coin.tscn est introuvable. L'as-tu créé dans Assets/Scenes/ ?");
                return;
            }

            // 1. Instancier (Créer un clone) du modèle de pièce
            Node2D coinInstance = _coinScene.Instantiate<Node2D>();
            
            // 2. Le placer à la position du Joueur. 
            // On le met un peu en hauteur (Y - 20) pour simuler la bourse.
            coinInstance.Position = new Vector2(ev.DropPosition.X, ev.DropPosition.Y - 20);
            
            // 3. Demander au moteur Godot d'ajouter cette pièce dans le "Niveau" actuel (la scène courante)
            GetTree().CurrentScene.AddChild(coinInstance);
        }
    }
}
