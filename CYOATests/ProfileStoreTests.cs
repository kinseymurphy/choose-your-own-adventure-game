// CYOATests/ProfileStoreTests.cs
using System.Text.Json;

[TestClass]
public class ProfileStoreTests
{
    private string _folder = "";

    [TestInitialize]
    public void Setup()
    {
        _folder = Path.Combine(Path.GetTempPath(), "cyoa-profiles-" + Guid.NewGuid().ToString("N"));
    }

    [TestCleanup]
    public void Cleanup()
    {
        if (Directory.Exists(_folder))
            Directory.Delete(_folder, true);
    }

    private string ProfilesPath => Path.Combine(_folder, "profiles.json");

    [TestMethod]
    public void Load_FromMissingFolder_ReturnsNoProfilesAndNoWarning()
    {
        ProfileStore store = new ProfileStore(_folder);

        ProfileLoadResult result = store.Load();

        Assert.AreEqual(0, result.Profiles.Count);
        Assert.IsFalse(result.WasUnreadable);
    }

    [TestMethod]
    public void Load_FromEmptyFolder_ReturnsNoProfilesAndNoWarning()
    {
        Directory.CreateDirectory(_folder);

        ProfileLoadResult result = new ProfileStore(_folder).Load();

        Assert.AreEqual(0, result.Profiles.Count);
        Assert.IsFalse(result.WasUnreadable);
    }

    [TestMethod]
    public void Save_FreshCharacter_RoundTripsEverySavedField()
    {
        ProfileStore store = new ProfileStore(_folder);
        Player bob = new Player
        {
            Name = "Bob",
            Race = "Human",
            Occupation = "Fighter",
            Strength = 14,
            Agility = 9,
            HealthPoints = 17,
            Gold = 11,
            Protection = 0,
            Weapon = new Weapon("long sword", 12, "-)=====>")
        };

        Assert.IsTrue(store.Save(bob));
        Player loaded = new ProfileStore(_folder).Load().Profiles.Single();

        Assert.AreEqual("Bob", loaded.Name);
        Assert.AreEqual("Human", loaded.Race);
        Assert.AreEqual("Fighter", loaded.Occupation);
        Assert.AreEqual(14, loaded.Strength);
        Assert.AreEqual(9, loaded.Agility);
        Assert.AreEqual(17, loaded.HealthPoints);
        Assert.AreEqual(11, loaded.Gold);
        Assert.AreEqual(0, loaded.Protection);
        Assert.IsNotNull(loaded.Weapon);
        Assert.AreEqual("long sword", loaded.Weapon!.Type);
        Assert.AreEqual(12, loaded.Weapon.MaxDamage);
        Assert.AreEqual("-)=====>", loaded.Weapon.AsciiArt);
        Assert.IsFalse(loaded.Weapon.IsMagical);
    }

    [TestMethod]
    public void Load_ReturnsProfilesInTheOrderTheyWereSaved()
    {
        ProfileStore store = new ProfileStore(_folder);
        store.Save(CreatePlayer("Zara"));
        store.Save(CreatePlayer("Adam"));

        List<Player> profiles = new ProfileStore(_folder).Load().Profiles;

        CollectionAssert.AreEqual(new[] { "Zara", "Adam" }, profiles.Select(p => p.Name).ToArray());
    }

    [TestMethod]
    public void Save_WritesVersionOne()
    {
        new ProfileStore(_folder).Save(CreatePlayer("Bob"));

        using JsonDocument doc = JsonDocument.Parse(File.ReadAllText(ProfilesPath));

        Assert.AreEqual(1, doc.RootElement.GetProperty("version").GetInt32());
    }

    [TestMethod]
    [DataRow("Bob", true)]
    [DataRow("bob", true)]
    [DataRow("BOB", true)]
    [DataRow("  Bob  ", true)]
    [DataRow("Bobby", false)]
    public void IsNameTaken_IgnoresCaseAndSurroundingSpaces(string name, bool expected)
    {
        ProfileStore store = new ProfileStore(_folder);
        store.Save(CreatePlayer("Bob"));

        Assert.AreEqual(expected, store.IsNameTaken(name));
    }

    [TestMethod]
    public void Save_ExistingName_ReplacesThatProfileInPlace()
    {
        ProfileStore store = new ProfileStore(_folder);
        store.Save(CreatePlayer("Bob"));
        store.Save(CreatePlayer("Ana"));

        Player bobAgain = CreatePlayer("Bob");
        bobAgain.Gold = 3;
        store.Save(bobAgain);

        List<Player> profiles = new ProfileStore(_folder).Load().Profiles;
        CollectionAssert.AreEqual(new[] { "Bob", "Ana" }, profiles.Select(p => p.Name).ToArray());
        Assert.AreEqual(3, profiles[0].Gold);
    }

    [TestMethod]
    public void Save_MagicalArmorAndMagicalWeapon_RoundTrip()
    {
        Player thief = CreatePlayer("Bob");
        Merchant merchant = new Merchant();
        merchant.BuyMagicalArmor(thief);
        merchant.BuyMagicalWeapon(thief);

        new ProfileStore(_folder).Save(thief);
        Player loaded = new ProfileStore(_folder).Load().Profiles.Single();

        Assert.AreEqual(3, loaded.Protection);
        Assert.AreEqual(4, loaded.Gold);
        Assert.AreEqual("magical dagger", loaded.Weapon!.Type);
        Assert.AreEqual(10, loaded.Weapon.MaxDamage);
        Assert.AreEqual("-)==>", loaded.Weapon.AsciiArt);
        Assert.IsTrue(loaded.Weapon.IsMagical);
    }

    [TestMethod]
    public void Load_DamagedFile_IsMovedToABackupAndReportsItsName()
    {
        Directory.CreateDirectory(_folder);
        File.WriteAllText(ProfilesPath, "{ this is not json");

        ProfileLoadResult result = new ProfileStore(_folder).Load();

        Assert.AreEqual(0, result.Profiles.Count);
        Assert.IsTrue(result.WasUnreadable);
        Assert.AreEqual("profiles.unreadable.json", result.BackupFileName);
        Assert.IsFalse(File.Exists(ProfilesPath));
        Assert.AreEqual("{ this is not json", File.ReadAllText(Path.Combine(_folder, "profiles.unreadable.json")));
    }

    [TestMethod]
    public void Load_DamagedFileWithBackupsPresent_UsesTheFirstFreeNumberedName()
    {
        Directory.CreateDirectory(_folder);
        File.WriteAllText(Path.Combine(_folder, "profiles.unreadable.json"), "first");
        File.WriteAllText(ProfilesPath, "second");

        Assert.AreEqual("profiles.unreadable-2.json", new ProfileStore(_folder).Load().BackupFileName);

        File.WriteAllText(ProfilesPath, "third");
        Assert.AreEqual("profiles.unreadable-3.json", new ProfileStore(_folder).Load().BackupFileName);

        Assert.AreEqual("first", File.ReadAllText(Path.Combine(_folder, "profiles.unreadable.json")));
        Assert.AreEqual("second", File.ReadAllText(Path.Combine(_folder, "profiles.unreadable-2.json")));
        Assert.AreEqual("third", File.ReadAllText(Path.Combine(_folder, "profiles.unreadable-3.json")));
    }

    [TestMethod]
    public void Load_FileWithVersionTwo_IsTreatedAsUnreadable()
    {
        Directory.CreateDirectory(_folder);
        File.WriteAllText(ProfilesPath, "{ \"version\": 2, \"profiles\": [] }");

        ProfileLoadResult result = new ProfileStore(_folder).Load();

        Assert.IsTrue(result.WasUnreadable);
        Assert.AreEqual("profiles.unreadable.json", result.BackupFileName);
    }

    [TestMethod]
    [DataRow("{ \"version\": 1 }")]
    [DataRow("{ \"version\": 1, \"profiles\": [ { \"race\": \"Human\", \"occupation\": \"Fighter\" } ] }")]
    [DataRow("{ \"version\": 1, \"profiles\": [ { \"name\": \"Bob\", \"occupation\": \"Fighter\" } ] }")]
    [DataRow("{ \"version\": 1, \"profiles\": [ null ] }")]
    [DataRow("{ \"version\": 1, \"profiles\": [ { \"name\": \"Bob\", \"race\": \"Human\", \"occupation\": \"Fighter\" } ] }")]
    [DataRow("{ \"version\": 1, \"profiles\": [ { \"name\": \"Bob\", \"race\": \"Human\", \"occupation\": \"Fighter\", \"weapon\": { \"maxDamage\": 12, \"asciiArt\": \"-\" } } ] }")]
    [DataRow("{ \"version\": 1, \"profiles\": [ { \"name\": \"Bob\", \"race\": \"Human\", \"occupation\": \"Fighter\", \"weapon\": { \"type\": \"long sword\", \"asciiArt\": \"-\" } } ] }")]
    public void Load_FileMissingRequiredData_IsTreatedAsUnreadable(string json)
    {
        Directory.CreateDirectory(_folder);
        File.WriteAllText(ProfilesPath, json);

        ProfileLoadResult result = new ProfileStore(_folder).Load();

        Assert.IsTrue(result.WasUnreadable);
        Assert.AreEqual(0, result.Profiles.Count);
    }

    [TestMethod]
    public void Save_AfterABackup_StartsAFreshReadableFile()
    {
        Directory.CreateDirectory(_folder);
        File.WriteAllText(ProfilesPath, "garbage");
        ProfileStore store = new ProfileStore(_folder);
        store.Load();

        Assert.IsTrue(store.Save(CreatePlayer("Bob")));

        ProfileLoadResult result = new ProfileStore(_folder).Load();
        Assert.IsFalse(result.WasUnreadable);
        Assert.AreEqual("Bob", result.Profiles.Single().Name);
    }

    [TestMethod]
    public void DamagedFileThatCannotBeMoved_IsReportedAndNeverOverwritten()
    {
        Directory.CreateDirectory(_folder);
        File.WriteAllText(ProfilesPath, "garbage");
        ProfileStore store = new ProfileStore(_folder);

        ProfileLoadResult result;
        bool saved;
        // Held open without delete sharing, so it can be read but not moved or replaced
        using (new FileStream(ProfilesPath, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            result = store.Load();
            saved = store.Save(CreatePlayer("Bob"));
        }

        Assert.IsTrue(result.WasUnreadable);
        Assert.IsNull(result.BackupFileName);
        Assert.IsFalse(saved);
        Assert.AreEqual("garbage", File.ReadAllText(ProfilesPath));
    }

    [TestMethod]
    public void Save_WhenTheFolderCannotBeCreated_ReportsFailureWithoutThrowing()
    {
        Directory.CreateDirectory(_folder);
        string blockingFile = Path.Combine(_folder, "not-a-folder");
        File.WriteAllText(blockingFile, "");

        ProfileStore store = new ProfileStore(Path.Combine(blockingFile, "profiles"));

        Assert.IsFalse(store.Save(CreatePlayer("Bob")));
    }

    [TestMethod]
    public void Save_WhenProfilesFileCannotBeReplaced_ReportsFailureAndLeavesItUnchanged()
    {
        ProfileStore store = new ProfileStore(_folder);
        store.Save(CreatePlayer("Bob"));
        string before = File.ReadAllText(ProfilesPath);

        bool saved;
        // Held open without delete sharing, so it can be read but not replaced
        using (new FileStream(ProfilesPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            saved = store.Save(CreatePlayer("Ana"));

        Assert.IsFalse(saved);
        Assert.AreEqual(before, File.ReadAllText(ProfilesPath));
        CollectionAssert.AreEqual(new[] { "profiles.json" }, Directory.GetFiles(_folder).Select(Path.GetFileName).ToArray());
    }

    private static Player CreatePlayer(string name)
    {
        return new Player
        {
            Name = name,
            Race = "Elf",
            Occupation = "Thief",
            Strength = 10,
            Agility = 12,
            HealthPoints = 15,
            Gold = 20,
            Weapon = new Weapon("dagger", 6, "-)==>")
        };
    }
}
