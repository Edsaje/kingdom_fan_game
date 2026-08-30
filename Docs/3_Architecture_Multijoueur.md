# 🌐 Architecture Multijoueur (Local & Ligne)

Ajouter du multijoueur (comme dans *Kingdom: Two Crowns*) change la donne. Rétro-ingénierer un jeu solo en multi est un cauchemar. Le faire dès le début est un jeu d'enfant avec Godot 4.

## 1. Le Concept d'Autorité (Authority)
En multijoueur réseau, on ne peut pas faire confiance aux clients (sinon ils trichent et se donnent 1000 pièces). Chaque objet a un "Propriétaire" (Authority). 
- Le Joueur 1 a l'autorité sur le Personnage 1.
- Le Joueur 2 a l'autorité sur le Personnage 2.
- **Le Serveur (Host)** a l'autorité sur TOUT le reste (les pièces au sol, les paysans, l'heure de la journée).

Règle d'or : **Seule l'autorité a le droit de bouger l'objet ou de valider une action.**

## 2. Le Local vs Ligne
Dans Godot, on gère les deux presque de la même façon pour se simplifier la vie :
- En **Local** : Le Joueur 1 lit les touches ZQSD, le Joueur 2 lit les touches de la Manette. Le PC fait office de Serveur. (Les deux joueurs ont l'autorité locale).
- En **Ligne** : Le Joueur 2 n'est physiquement pas là. Le Joueur 1 (Serveur) voit le Personnage 2 bouger tout seul, car la position est synchronisée via le réseau.

## 3. Ce qu'on vient de modifier dans le code
Le script `PlayerController.cs` a été adapté. Avant de lire les flèches du clavier, il pose désormais une question : `"Est-ce que je suis l'autorité de ce personnage ?"`.
- Si la réponse est **NON** (ex: on est le Joueur 1 qui regarde l'écran, et c'est le code du Joueur 2), le script s'arrête. On ne lit pas le clavier pour lui, on laisse Godot synchroniser sa position via le réseau.
- De plus, on a rajouté une variable `PlayerId` pour différencier les contrôles (Joueur 1 = Flèches, Joueur 2 = ZQSD par exemple).

## 4. Les RPC (Remote Procedure Call) - Pour plus tard
C'est magique : c'est comme appeler une méthode en Java, mais elle s'exécute sur un autre ordinateur !
Quand on fera le réseau en ligne et qu'un joueur voudra jeter une pièce, il ne la créera pas lui-même. Il enverra un RPC au Serveur : `[Rpc] DemanderLacherPiece()`. Le serveur vérifiera que le joueur a de l'argent, et si oui, le serveur fera apparaître la pièce pour tout le monde. C'est l'architecture "Server-Authoritative".
