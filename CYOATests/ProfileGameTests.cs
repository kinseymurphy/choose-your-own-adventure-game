// CYOATests/ProfileGameTests.cs
// Drives the whole game through scripted console input, with Profiles kept in a temporary folder.
[TestClass]
[DoNotParallelize]
public class ProfileGameTests
{
    private const int TimeoutMs = 10000;

    // Creates an English Human Fighter named Bob and proceeds to the path choice
    private const string CreateBob = "human\nBob\nfighter\nroll\nroll\nroll\nroll\n2\n";

    private TextReader? _originalIn;
    private TextWriter? _originalOut;
    private string _folder = "";

    [TestInitialize]
    public void Setup()
    {
        _originalIn = Console.In;
        _originalOut = Console.Out;
        _folder = Path.Combine(Path.GetTempPath(), "cyoa-game-" + Guid.NewGuid().ToString("N"));
    }

    [TestCleanup]
    public void Cleanup()
    {
        if (_originalIn != null)
            Console.SetIn(_originalIn);

        if (_originalOut != null)
            Console.SetOut(_originalOut);

        if (Directory.Exists(_folder))
            Directory.Delete(_folder, true);
    }

    private static Messages GetMessages(string language)
    {
        Messages messages = new Messages();
        messages.SetCurrentLanguage(language);
        messages.ReadDictionary();
        return messages;
    }

    // Plays a whole Session with the given input and returns everything printed
    private string Play(string input, ProfileStore? store = null)
    {
        Console.SetIn(new StringReader(input));

        using StringWriter writer = new StringWriter();
        Console.SetOut(writer);

        new Game(store ?? new ProfileStore(_folder)).StartGame();

        return writer.ToString();
    }

    [TestMethod]
    [Timeout(TimeoutMs)]
    public void NoProfiles_ChoosingPlay_CreatesCharacterAndSavesIt()
    {
        Messages messages = GetMessages("English");

        string output = Play("1\n1\n" + CreateBob + "e\n");

        StringAssert.Contains(output, messages.GetMessage("race_prompt"));
        StringAssert.Contains(output, messages.GetMessage("path_prompt_full"));

        Player saved = new ProfileStore(_folder).Load().Profiles.Single();
        Assert.AreEqual("Bob", saved.Name);
        Assert.AreEqual("Human", saved.Race);
        Assert.AreEqual("Fighter", saved.Occupation);
    }

    [TestMethod]
    [Timeout(TimeoutMs)]
    [DataRow("1", "English", "1. Bob (Human Fighter)", "2. Ana (Elf Thief)", "3. New profile")]
    [DataRow("2", "Spanish", "1. Bob (Humano Guerrero)", "2. Ana (Elfo Ladron)", "3. Nuevo perfil")]
    public void SavedProfiles_MenuListsThemInCreationOrderWithNewProfileLast(
        string languageChoice, string language, string first, string second, string newProfile)
    {
        ProfileStore store = new ProfileStore(_folder);
        store.Save(SavedPlayer("Bob", "Human", "Fighter"));
        store.Save(SavedPlayer("Ana", "Elf", "Thief"));
        Messages messages = GetMessages(language);

        string output = Play(languageChoice + "\n1\n1\ne\n");

        StringAssert.Contains(output, messages.GetMessage("profile_menu_header"));
        int firstAt = output.IndexOf(first);
        int secondAt = output.IndexOf(second);
        int newAt = output.IndexOf(newProfile);
        Assert.IsTrue(firstAt >= 0 && firstAt < secondAt && secondAt < newAt, "Profile menu out of order:\n" + output);
    }

    [TestMethod]
    [Timeout(TimeoutMs)]
    public void ProfileMenu_InvalidChoice_ShowsInvalidAndTheMenuAgain()
    {
        new ProfileStore(_folder).Save(SavedPlayer("Bob"));
        Messages messages = GetMessages("English");

        string output = Play("1\n1\n7\n1\ne\n");

        string afterInvalid = output.Substring(output.IndexOf(messages.GetMessage("invalid")));
        StringAssert.Contains(afterInvalid, messages.GetMessage("profile_menu_header"));
        StringAssert.Contains(afterInvalid, "1. Bob (Human Fighter)");
    }

    [TestMethod]
    [Timeout(TimeoutMs)]
    public void PickingAProfile_WelcomesBackShowsStatsAndGoesToThePathChoice()
    {
        ProfileStore store = new ProfileStore(_folder);
        store.Save(SavedPlayer("Bob"));
        string savedFile = File.ReadAllText(Path.Combine(_folder, "profiles.json"));
        Messages messages = GetMessages("English");

        string output = Play("1\n1\n1\ne\n");

        StringAssert.Contains(output, "Welcome back, Bob!");
        StringAssert.Contains(output, messages.GetMessage("stats_header"));
        StringAssert.Contains(output, messages.GetMessage("stats_health") + 16);
        StringAssert.Contains(output, messages.GetMessage("stats_gold") + 20);
        StringAssert.Contains(output, messages.GetMessage("path_prompt_full"));
        Assert.IsFalse(output.Contains(messages.GetMessage("next_action")), "The next-action menu should be skipped.");
        Assert.IsFalse(output.Contains(messages.GetMessage("race_prompt")), "Character creation should be skipped.");
        Assert.AreEqual(savedFile, File.ReadAllText(Path.Combine(_folder, "profiles.json")));
    }

    [TestMethod]
    [Timeout(TimeoutMs)]
    public void PickingNewProfile_CreatesAndSavesASecondProfileAfterTheFirst()
    {
        new ProfileStore(_folder).Save(SavedPlayer("Ana", "Elf", "Thief"));

        Play("1\n1\n2\n" + CreateBob + "e\n");

        List<Player> profiles = new ProfileStore(_folder).Load().Profiles;
        CollectionAssert.AreEqual(new[] { "Ana", "Bob" }, profiles.Select(p => p.Name).ToArray());
    }

    [TestMethod]
    [Timeout(TimeoutMs)]
    public void NewProfile_WithATakenNameInDifferentCase_AsksForAnotherNameKeepingTheRace()
    {
        ProfileStore store = new ProfileStore(_folder);
        store.Save(SavedPlayer("Bob"));
        string originalBob = File.ReadAllText(Path.Combine(_folder, "profiles.json"));
        Messages messages = GetMessages("English");

        string output = Play("1\n1\n2\nelf\nbob\nCara\nthief\nroll\nroll\nroll\nroll\n2\ne\n");

        StringAssert.Contains(output, messages.GetMessage("profile_name_taken"));
        string racePrompt = messages.GetMessage("race_prompt");
        Assert.AreEqual(output.IndexOf(racePrompt), output.LastIndexOf(racePrompt), "The Race was asked for twice.");

        List<Player> profiles = new ProfileStore(_folder).Load().Profiles;
        CollectionAssert.AreEqual(new[] { "Bob", "Cara" }, profiles.Select(p => p.Name).ToArray());
        Assert.AreEqual("Elf", profiles[1].Race);
        Assert.AreEqual("Human", profiles[0].Race);
        Assert.AreEqual(16, profiles[0].HealthPoints);
    }

    [TestMethod]
    [Timeout(TimeoutMs)]
    public void LeavingTheMerchant_SavesPurchasesButNotADifferentHealthPoints()
    {
        new ProfileStore(_folder).Save(SavedPlayer("Bob"));

        // Pick Bob, go South, buy Magical Armor, leave, then exit
        Play("1\n1\n1\ns\n1\n3\ne\n");

        Player saved = new ProfileStore(_folder).Load().Profiles.Single();
        Assert.AreEqual(12, saved.Gold);
        Assert.AreEqual(3, saved.Protection);
        Assert.AreEqual(16, saved.HealthPoints);
    }

    [TestMethod]
    [Timeout(TimeoutMs)]
    public void AfterRestart_MerchantSaysMagicalArmorAndMagicalWeaponAreAlreadyOwned()
    {
        Player bob = SavedPlayer("Bob");
        bob.Protection = 3;
        bob.Weapon = new Weapon("magical long sword", 16, "-)=====>") { IsMagical = true };
        new ProfileStore(_folder).Save(bob);
        Messages messages = GetMessages("English");

        string output = Play("1\n1\n1\ns\n1\n2\n3\ne\n");

        string owned = messages.GetMessage("merchant_already_owned");
        int first = output.IndexOf(owned);
        Assert.IsTrue(first >= 0 && output.IndexOf(owned, first + 1) > first, "Both items should be refused:\n" + output);
        Assert.AreEqual(20, new ProfileStore(_folder).Load().Profiles.Single().Gold);
    }

    [TestMethod]
    [Timeout(TimeoutMs)]
    [DataRow("n")]
    [DataRow("w")]
    public void RetreatingBeforeTheFight_LeavesTheProfileUnchanged(string path)
    {
        new ProfileStore(_folder).Save(SavedPlayer("Bob"));
        string savedFile = File.ReadAllText(Path.Combine(_folder, "profiles.json"));

        Play("1\n1\n1\n" + path + "\nr\n");

        Assert.AreEqual(savedFile, File.ReadAllText(Path.Combine(_folder, "profiles.json")));
    }

    // Strength 0 always misses and the high Health Points survive the first round, so the retreat is always offered
    [TestMethod]
    [Timeout(TimeoutMs)]
    [DataRow("n")]
    [DataRow("w")]
    public void RetreatingDuringTheFight_LeavesTheProfileUnchanged(string path)
    {
        Player bob = SavedPlayer("Bob");
        bob.Strength = 0;
        bob.HealthPoints = 1000;
        new ProfileStore(_folder).Save(bob);
        string savedFile = File.ReadAllText(Path.Combine(_folder, "profiles.json"));
        Messages messages = GetMessages("English");

        string output = Play("1\n1\n1\n" + path + "\na\nr\n");

        StringAssert.Contains(output, messages.GetMessage("retreat_combat"));
        Assert.AreEqual(savedFile, File.ReadAllText(Path.Combine(_folder, "profiles.json")));
    }

    // Strength 0 always misses and Agility 0 never blocks, so Grubnik always wins.
    // Combat keeps fighting when the scripted input runs out.
    [TestMethod]
    [Timeout(TimeoutMs)]
    public void DefeatByGrubnik_DoesNotOverwriteTheSavedProfile()
    {
        Player bob = SavedPlayer("Bob");
        bob.Strength = 0;
        bob.Agility = 0;
        bob.HealthPoints = 5;
        new ProfileStore(_folder).Save(bob);
        Messages messages = GetMessages("English");

        string output = Play("1\n1\n1\nw\na\n");

        StringAssert.Contains(output, messages.GetMessage("goblin_defeat"));
        Player saved = new ProfileStore(_folder).Load().Profiles.Single();
        Assert.AreEqual(5, saved.HealthPoints);
        Assert.AreEqual(20, saved.Gold);
        Assert.AreEqual(0, saved.Protection);
        Assert.AreEqual("long sword", saved.Weapon!.Type);
    }

    // Strength 20 always hits and the high Health Points outlast Grubnik's d12 stats
    [TestMethod]
    [Timeout(TimeoutMs)]
    public void VictoryOverGrubnik_DoesNotOverwriteTheSavedHealthPoints()
    {
        Player bob = SavedPlayer("Bob");
        bob.Strength = 20;
        bob.Agility = 0;
        bob.HealthPoints = 1000;
        new ProfileStore(_folder).Save(bob);
        Messages messages = GetMessages("English");

        string output = Play("1\n1\n1\nw\na\n");

        StringAssert.Contains(output, messages.GetMessage("goblin_victory"));
        Assert.AreEqual(1000, new ProfileStore(_folder).Load().Profiles.Single().HealthPoints);
    }

    [TestMethod]
    [Timeout(TimeoutMs)]
    public void DamagedProfileFile_WarnsNamingTheBackupThenGoesToCharacterCreation()
    {
        Directory.CreateDirectory(_folder);
        File.WriteAllText(Path.Combine(_folder, "profiles.json"), "not json");
        Messages messages = GetMessages("English");

        string output = Play("1\n1\n" + CreateBob + "e\n");

        string warning = string.Format(messages.GetMessage("profile_file_unreadable"), "profiles.unreadable.json");
        StringAssert.Contains(output, warning);
        Assert.IsTrue(output.IndexOf(warning) < output.IndexOf(messages.GetMessage("race_prompt")));
        Assert.IsFalse(output.Contains(messages.GetMessage("profile_menu_header")));
    }

    [TestMethod]
    [Timeout(TimeoutMs)]
    public void DamagedProfileFileThatCannotBeMoved_WarnsAndGoesToCharacterCreation()
    {
        Directory.CreateDirectory(_folder);
        string profilesPath = Path.Combine(_folder, "profiles.json");
        File.WriteAllText(profilesPath, "not json");
        Messages messages = GetMessages("English");

        string output;
        using (new FileStream(profilesPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            output = Play("1\n1\n" + CreateBob + "e\n");

        StringAssert.Contains(output, messages.GetMessage("profile_file_unreadable_not_moved"));
        StringAssert.Contains(output, messages.GetMessage("race_prompt"));
        Assert.AreEqual("not json", File.ReadAllText(profilesPath));
    }

    [TestMethod]
    [Timeout(TimeoutMs)]
    public void FailedSaveAfterCreation_WarnsAndContinuesToThePathChoice()
    {
        Directory.CreateDirectory(_folder);
        string blockingFile = Path.Combine(_folder, "not-a-folder");
        File.WriteAllText(blockingFile, "");
        Messages messages = GetMessages("English");

        string output = Play("1\n1\n" + CreateBob + "e\n", new ProfileStore(Path.Combine(blockingFile, "profiles")));

        string warning = messages.GetMessage("profile_save_failed");
        StringAssert.Contains(output, warning);
        Assert.IsTrue(output.IndexOf(warning) < output.IndexOf(messages.GetMessage("path_prompt_full")));
        StringAssert.Contains(output, messages.GetMessage("adventure_ends"));
    }

    [TestMethod]
    [Timeout(TimeoutMs)]
    public void FailedSaveAfterTheMerchant_WarnsAndLeavesTheSavedProfileUnchanged()
    {
        new ProfileStore(_folder).Save(SavedPlayer("Bob"));
        string profilesPath = Path.Combine(_folder, "profiles.json");
        string before = File.ReadAllText(profilesPath);
        Messages messages = GetMessages("English");

        string output;
        // Held open without delete sharing, so it can be read but not replaced
        using (new FileStream(profilesPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            output = Play("1\n1\n1\ns\n1\n3\ne\n");

        string farewell = messages.GetMessage("merchant_farewell");
        Assert.IsTrue(output.IndexOf(messages.GetMessage("profile_save_failed")) > output.IndexOf(farewell));
        StringAssert.Contains(output, messages.GetMessage("adventure_ends"));
        Assert.AreEqual(before, File.ReadAllText(profilesPath));
    }

    private static Player SavedPlayer(string name, string race = "Human", string occupation = "Fighter")
    {
        return new Player
        {
            Name = name,
            Race = race,
            Occupation = occupation,
            Strength = 13,
            Agility = 8,
            HealthPoints = 16,
            Gold = 20,
            Protection = 0,
            Weapon = new Weapon("long sword", 12, "-)=====>")
        };
    }
}
