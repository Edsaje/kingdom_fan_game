# 👑 Les Règles d'Or du Projet (Manifeste)

Pour que ce jeu puisse grandir (ajouter des centaines de PNJ, du multijoueur, des DLC) sans s'effondrer sous son propre poids, nous respecterons strictement ces règles d'ingénierie.

## 1. Découplage Absolu (L'Event Bus)
- **La Règle** : Aucun système ne "parle" directement à un autre s'ils ne font pas partie du même objet.
- **L'Exemple** : Le Joueur ne modifie pas lui-même le texte de l'UI (l'interface) pour dire qu'il n'a plus d'argent. Le Joueur émet un événement `CoinCountChangedEvent`. L'UI écoute cet événement et se met à jour.
- **Pourquoi ?** Si on supprime l'UI pour faire un test, le jeu ne crashera pas.

## 2. Zéro Chiffre Magique (Data-Driven)
- **La Règle** : Les points de vie, la vitesse, le coût d'une construction ne doivent **JAMAIS** être écrits "en dur" dans le code C#.
- **L'Exemple** : La vitesse d'un paysan est lue depuis le fichier `units_config.json`.
- **Pourquoi ?** Permet d'équilibrer le jeu sans recompiler, et ouvre la porte aux Mods et DLC facilement.

## 3. Multijoueur & Autorité Réseau (Network-First)
- **La Règle** : On ne lit jamais les touches clavier sans avoir demandé la permission au serveur.
- **L'Exemple** : `if (!IsMultiplayerAuthority()) return;`
- **Pourquoi ?** Évite les tricheurs en ligne et évite de réécrire le jeu de zéro le jour où on lance les serveurs.

## 4. Performance et Garbage Collector (C#)
- **La Règle** : Ne **jamais** allouer de nouveaux objets (`new MonObjet()`) dans la méthode `_Process()` qui s'exécute 60 fois par seconde.
- **L'Exemple** : On utilise des `struct` (type valeur) pour les événements, pas des `class`. On réutilise les variables (Object Pooling) pour les pièces d'ambre plutôt que de les détruire et les recréer sans cesse.
- **Pourquoi ?** Évite les micro-gels (stutters) quand le Garbage Collector (le nettoyeur de mémoire de C#) fait son travail.

## 5. Petite Taille, Grande Fonction (Clean Code)
- **La Règle** : Si un script fait plus de 200 lignes, il fait probablement trop de choses.
- **L'Exemple** : Le `PlayerController.cs` gère les déplacements. S'il doit se battre avec une épée, on créera un `PlayerCombat.cs`.
- **Pourquoi ?** C'est le principe de la Responsabilité Unique (SRP des principes SOLID).

## 6. L'Hygiène Git (Git Flow)
- **La Règle** : On ne travaille jamais sur la branche `main`.
- **L'Exemple** : On code sur `develop`, on fait des commits clairs (`feat: ajoute le saut`, `fix: corrige le bug de l'or`), et on fait un vrai déploiement sur `main` quand une version est jouable.
