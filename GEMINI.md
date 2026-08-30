# Règles d'Architecture du Projet (Kingdom-Like)

Ce projet Godot 4.x (C# / .NET) respecte strictement les règles d'ingénierie suivantes :

0. **Rigueur Absolue & Zéro Dette Technique (No Hacks)**
   - Interdiction formelle d'utiliser des solutions de facilité, des "hacks" ou des pansements ("quick-and-dirty") qui contournent l'architecture.
   - Si une fonctionnalité est trop complexe pour l'architecture actuelle, on refactorise l'architecture, on ne la bafoue pas. Aucune détérioration du code n'est tolérée dans le temps.

1. **Paradigme SOLID (100%) & 0% STUPID**
   - **SOLID** : Responsabilité unique des scripts (SRP). Ouvert à l'extension (Modding/DLC) mais fermé à la modification du Core (OCP).
   - **0% STUPID** : Aucun couplage fort (Tight Coupling). L'abus de Singletons (God Objects) est interdit ; privilégier l'injection de dépendances et l'Event Bus.

2. **Philosophie MVC (Model-View-Controller) pour Godot**
   - **Model** : Données C# pures (POCO comme `UnitData.cs`), lues depuis JSON. Aucune référence à Godot.
   - **View** : Les scènes Godot (`.tscn`), Nœuds, Sprites, UI.
   - **Controller** : Les scripts C# (`PlayerController.cs`). La Vue et le Modèle ne se connaissent pas.

3. **Découplage Absolu (Event Bus)**
   - Ne jamais utiliser de références directes entre des systèmes distants.
   - Utiliser `GameManager.Events.Publish()` et s'abonner via `Events.Subscribe()`.

4. **Architecture Data-Driven (Système de DLC)**
   - Zéro chiffre magique dans le code source. Statistiques lues depuis le `DataManager`.

5. **Multijoueur par Défaut (Server Authority)**
   - Toute logique critique doit être précédée d'une vérification : `if (!IsMultiplayerAuthority()) return;`.

6. **Performance C# (Garbage Collector)**
   - Interdiction d'utiliser `new` à l'intérieur de `_Process`.
   - Événements de l'EventBus définis comme des `struct` (types valeur).
