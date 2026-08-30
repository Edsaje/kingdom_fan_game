using Godot;
using KingdomCore.Events;
using KingdomCore.Data;

namespace KingdomCore
{
    // GameManager hérite de Node. Il sera placé dans la scène principale du jeu.
    public partial class GameManager : Node
    {
        // Propriétés statiques pour y accéder facilement depuis n'importe où (Singleton pattern)
        public static EventBus Events { get; private set; }
        public static DataManager Data { get; private set; }

        public override void _Ready()
        {
            // Initialisation de nos systèmes fondamentaux
            Events = new EventBus();
            Data = new DataManager();

            // "res://" est le chemin absolu virtuel vers le dossier de ton projet Godot
            Data.LoadUnitData("res://Data/Configs/units_config.json");
            
            GD.Print("GameManager initialisé avec succès ! JSON chargé.");
        }
    }
}
