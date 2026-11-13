// Profiles live in the per-user local application data folder (ADR 0003)
string profileFolder = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
    "ChooseYourAdventure");

Game game = new Game(new ProfileStore(profileFolder));
game.StartGame();
