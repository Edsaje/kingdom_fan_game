# 🛠️ Premiers pas dans l'éditeur Godot

Maintenant que notre architecture de code C# est prête, il est temps de lier ce code au moteur visuel. Suis ces 3 étapes simples.

## 1. Configurer le GameManager (Singleton / Autoload)
Notre `GameManager` doit tourner en permanence en arrière-plan pour gérer les événements (EventBus) et les données.
1. Dans Godot, clique sur le menu **Projet** (en haut à gauche) > **Paramètres du projet**.
2. Va dans l'onglet **Autoload** (ou "Chargement auto").
3. Clique sur la petite icône de dossier dans "Chemin" (Path). Navigue jusqu'au dossier `Core/` et choisis `GameManager.cs`.
4. Laisse le nom "GameManager" et clique sur le bouton **Ajouter** à droite.
5. *(Bravo, ton chef d'orchestre est désormais actif dans tout le jeu !)*

## 2. Créer la Scène du Personnage
Godot utilise des "Scènes" (des ensembles de nœuds). Nous allons créer la scène de notre Héros.
1. Dans le panneau à gauche, clique sur **Scène 2D** pour créer la racine.
2. Double-clique sur le nom "Node2D" qui vient d'apparaître pour le **renommer** en `Player`.
3. Fais un **Clic droit** sur "Player" > **Attacher un script**.
4. Au lieu de le laisser créer un script vierge, clique sur la petite icône de dossier à côté de "Chemin" et **sélectionne** notre `PlayerController.cs` existant situé dans le dossier `Core/Player/`.
5. Fais `Ctrl+S` pour sauvegarder. Je te conseille de créer un dossier `Assets/Scenes/` et d'y enregistrer ce fichier sous le nom `Player.tscn`.

## 3. Lancer le Premier Test !
1. Clique sur le bouton **"Jouer la scène sélectionnée"** (l'icône en forme de clapet de cinéma avec un bouton Play en haut à droite, ou appuie sur F6).
2. Appuie sur **Espace** (la touche d'action `ui_accept` par défaut).
3. Regarde la console **Sortie (Output)** située tout en bas de ton éditeur Godot. 
4. Tu devrais y voir nos messages s'afficher : *"Pièce jetée ! Il reste 4 pièces."* ainsi que le succès de l'initialisation du `GameManager`.

*(Note : C'est normal si l'écran de jeu est tout gris pour le moment, car nous n'avons pas encore attaché d'image (Sprite) à notre Player ! C'est la prochaine étape !)*
