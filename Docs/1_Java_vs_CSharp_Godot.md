# 🚀 De Java à C# / Godot : Guide de Survie

Puisque tu viens du monde Java, j'ai une excellente nouvelle : **C# est le cousin de Java**. Tu vas te sentir à la maison très vite. Microsoft s'est d'ailleurs beaucoup inspiré de Java pour créer le C#.

## 1. Les Différences Java vs C#

| Concept Java | Équivalent C# | Remarque |
| --- | --- | --- |
| `package com.monjeu;` | `namespace MonJeu {}` | En C#, on utilise des blocs de code `{}` pour encapsuler. |
| `import java.util.List;` | `using System.Collections.Generic;` | Tout se passe en haut du fichier. |
| `public void setNom(String n)` | `public string Nom { get; set; }` | C# utilise des **Propriétés**. Plus besoin d'écrire des getters/setters à rallonge ! |
| `@Override` | `public override void...` | C# demande explicitement le mot-clé `override` dans la signature de la méthode. |
| `implements Interface` | `: Interface` | En C#, l'héritage de classe (`extends`) et d'interface utilisent le même symbole `:`. |

## 2. Les Concepts Clés de Godot

Oublie la fameuse méthode `public static void main(String[] args)`. Un jeu vidéo ne fonctionne pas comme un programme classique de gestion. 
Godot fonctionne avec une arborescence de **Nœuds (Nodes)**. Un Nœud est un petit bloc logique (ex: un Sprite pour l'image, une Hitbox pour les collisions, un Script pour le code). On assemble des Nœuds comme des Legos pour former des **Scènes** (ex: La scène "Joueur", la scène "Pièce", la scène "Niveau_1").

- `Node` : La classe de base de tout élément dans le jeu (comme `Object` en Java).
- `Node2D` : Un Node spécialisé qui a une position (X, Y) dans l'espace.
- `_Ready()` : Une méthode appelée automatiquement par Godot quand le nœud apparaît à l'écran (un peu comme un constructeur).
- `_Process(double delta)` : Appelée à **CHAQUE IMAGE** (ex: 60 fois par seconde). C'est là qu'on gère les déplacements ou les entrées clavier.

## 3. Ce que nous venons de coder (L'Étape 3)

### Le GameManager
C'est notre "chef d'orchestre". Dans Godot, nous le configurerons comme un **Autoload** (un Singleton natif). Il survivra au chargement des différents niveaux et sera accessible de partout. C'est lui qui initialise notre `EventBus` et charge nos `units_config.json`.

### Le PlayerController
C'est le code de notre Roi/Reine.
1. À chaque frame (`_Process`), il lit les touches (Flèches gauche/droite) avec la fonction native `Input.GetAxis()`.
2. Il modifie sa `Position` pour se déplacer.
3. Si on appuie sur la touche d'action (`ui_accept`, souvent Espace), il retire une pièce de son inventaire et crie dans notre EventBus : `CoinDroppedEvent`.

### Et maintenant ?
Dans notre prochaine session, nous ouvrirons l'éditeur Godot pour créer la scène visuelle du Joueur et y attacher ce script `PlayerController`.
