using System.Text.Json;

public class ProfileLoadResult
{
    public List<Player> Profiles { get; set; } = new List<Player>();
    public bool WasUnreadable { get; set; }

    // The name the unreadable file was moved to, or null if it couldn't be moved
    public string? BackupFileName { get; set; }
}

// The only class that reads or writes the profile file (ADR 0003)
public class ProfileStore
{
    private const int FileVersion = 1;
    private const string FileName = "profiles.json";

    private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        WriteIndented = true
    };

    private readonly string _folder;

    public ProfileStore(string folder)
    {
        _folder = folder;
    }

    private string ProfilesPath => Path.Combine(_folder, FileName);

    public ProfileLoadResult Load()
    {
        var result = new ProfileLoadResult();

        if (TryReadProfiles(out List<Player> profiles))
        {
            result.Profiles = profiles;
            return result;
        }

        result.WasUnreadable = true;
        result.BackupFileName = MoveAside();
        return result;
    }

    public bool IsNameTaken(string name)
    {
        TryReadProfiles(out List<Player> profiles);
        return profiles.Any(profile => SameName(profile.Name, name));
    }

    // Profile names are unique ignoring case and surrounding spaces
    private static bool SameName(string a, string b)
    {
        return string.Equals(a.Trim(), b.Trim(), StringComparison.OrdinalIgnoreCase);
    }

    public bool Save(Player player)
    {
        // Never overwrite a file that couldn't be read or moved aside
        if (!TryReadProfiles(out List<Player> profiles))
            return false;

        int existing = profiles.FindIndex(profile => SameName(profile.Name, player.Name));
        if (existing >= 0)
            profiles[existing] = player;
        else
            profiles.Add(player);

        // Write everything to a temporary file first so a failed write never touches profiles.json
        string tempPath = ProfilesPath + ".tmp";
        try
        {
            Directory.CreateDirectory(_folder);
            var file = new ProfileFile { Version = FileVersion, Profiles = profiles };
            File.WriteAllText(tempPath, JsonSerializer.Serialize(file, JsonOptions));
            File.Move(tempPath, ProfilesPath, true);
            return true;
        }
        // Any failure is reported, never thrown, so the Session can carry on
        catch (Exception)
        {
            DeleteQuietly(tempPath);
            return false;
        }
    }

    private static void DeleteQuietly(string path)
    {
        try
        {
            File.Delete(path);
        }
        catch (Exception ex) when (IsFileError(ex))
        {
            // Leaving a stray temporary file is harmless; profiles.json is untouched
        }
    }

    // A missing file means no Profiles. Returns false only when the file exists but can't be used.
    private bool TryReadProfiles(out List<Player> profiles)
    {
        profiles = new List<Player>();

        if (!File.Exists(ProfilesPath))
            return true;

        try
        {
            ProfileFile? file = JsonSerializer.Deserialize<ProfileFile>(File.ReadAllText(ProfilesPath), JsonOptions);
            if (file?.Version != FileVersion || file.Profiles == null || !file.Profiles.All(HasRequiredData))
                return false;

            profiles = file.Profiles;
            return true;
        }
        catch (Exception ex) when (ex is JsonException || IsFileError(ex))
        {
            return false;
        }
    }

    private static bool HasRequiredData(Player? profile)
    {
        return profile != null &&
               !string.IsNullOrWhiteSpace(profile.Name) &&
               !string.IsNullOrWhiteSpace(profile.Race) &&
               !string.IsNullOrWhiteSpace(profile.Occupation) &&
               profile.Weapon != null &&
               !string.IsNullOrWhiteSpace(profile.Weapon.Type) &&
               profile.Weapon.AsciiArt != null &&
               profile.Weapon.MaxDamage > 0;
    }

    private static bool IsFileError(Exception ex)
    {
        return ex is IOException || ex is UnauthorizedAccessException;
    }

    // Moves an unreadable profiles.json to a backup name, or returns null if it can't be moved
    private string? MoveAside()
    {
        string backupName = "profiles.unreadable.json";
        for (int number = 2; File.Exists(Path.Combine(_folder, backupName)); number++)
            backupName = $"profiles.unreadable-{number}.json";

        try
        {
            // No overwrite, so an existing backup is never replaced
            File.Move(ProfilesPath, Path.Combine(_folder, backupName), false);
            return backupName;
        }
        catch (Exception ex) when (IsFileError(ex))
        {
            return null;
        }
    }

    private class ProfileFile
    {
        public int Version { get; set; }
        public List<Player>? Profiles { get; set; }
    }
}
