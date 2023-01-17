using Delfinovin.Properties;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace Delfinovin.Controllers
{
    /// <summary>
    /// A class that provides global access to profiles across the application.
    /// It provides functionality to create, delete and read profiles
    /// </summary>
    public static class ProfileManager
    {
        private const string PROFILE_DIRECTORY = ".\\profiles";

        public static ControllerProfile[] CurrentProfiles { get; set; } = new ControllerProfile[4];
        static ProfileManager()
        {
            for (int i = 0; i < CurrentProfiles.Length; i++)
            {
                // Populate the current profiles with profiles from the default profile list.
                LoadDefaultProfile(i);
            }
        }

        public static bool LoadDefaultProfile(int port)
        {
            bool applied = false;
            if (CheckIfProfileExists(Settings.Default.DefaultProfiles[port]))
                applied = true;

            CurrentProfiles[port] = GetProfileFromProfileName(Settings.Default.DefaultProfiles[port]);

            return applied;
        }

        /// <summary>
        /// Get a string list of all profiles currently in the profiles
        /// directory.
        /// </summary>
        /// <returns>List<string> - A list of profile names from the profile directory.</returns>
        public static List<string> GetProfileNameList()
        {
            List<string> profiles = new List<string>();
            CheckOrCreateDirectory();

            string[] fileNames = Directory.GetFiles(PROFILE_DIRECTORY, "*.json");
            foreach (string fileName in fileNames)
            {
                profiles.Add(GetProfileNameFromFileName(fileName));
            }

            return profiles;
        }

        private static bool CheckIfProfileExists(string profileName)
        {
            string profilePath = Path.Combine(PROFILE_DIRECTORY, profileName + ".json");
            return File.Exists(profilePath);
        }

        /// <summary>
        /// Load a ControllerProfile from file. If it does not exist,
        /// return a default ControllerProfile object.
        /// </summary>
        /// <param name="filePath">The file path to the JSON profile.</param>
        /// <returns>ControllerProfile - either the loaded profile or a default profile.</returns>
        public static ControllerProfile GetProfileFromFileName(string filePath)
        {
            ControllerProfile profile = new ControllerProfile();

            if (!File.Exists(filePath))
                return profile;

            string jsonString = File.ReadAllText(filePath);

            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                ObjectCreationHandling = ObjectCreationHandling.Replace 
            };
            
            profile = JsonConvert.DeserializeObject<ControllerProfile>(jsonString, settings);
            profile.ProfileName = GetProfileNameFromFileName(filePath);

            return profile;
        }

        /// <summary>
        /// Load a ControllerProfile from a profile name. If it does not exist, return
        /// a default ControllerProfile object.
        /// </summary>
        /// <param name="profileName">The name of the profile.</param>
        /// <returns>ControllerProfile</returns>
        public static ControllerProfile GetProfileFromProfileName(string profileName)
        {
            string profilePath = Path.Combine(PROFILE_DIRECTORY, profileName + ".json");
            return GetProfileFromFileName(profilePath);
        }

        /// <summary>
        /// Save a profile to a file.
        /// </summary>
        /// <param name="profile">The profile being saved to a file.</param>
        /// <param name="profileName">The name for the profile.</param>
        public static void SaveProfile(ControllerProfile profile, string profileName)
        {
            // Serialize the profile into a string
            string jsonString = JsonConvert.SerializeObject(profile, Formatting.Indented);
            CheckOrCreateDirectory();

            // Write the file to a JSON text file.
            File.WriteAllText(Path.Combine(PROFILE_DIRECTORY, profileName + ".json"), jsonString);
        }

        /// <summary>
        /// Try to find a profile by its profile name.
        /// If it exists, return true and delete it.
        /// </summary>
        /// <param name="profileName">The name of the profile to be deleted.</param>
        /// <returns>Boolean - whether or not the profile was deleted properly.</returns>
        public static bool DeleteProfileByProfileName(string profileName)
        {
            string profilePath = Path.Combine(PROFILE_DIRECTORY, profileName + ".json");
            if (!File.Exists(profilePath))
                return false;

            File.Delete(profilePath); 
            return true;
        }

        private static string GetProfileNameFromFileName(string fileName) => Path.GetFileNameWithoutExtension(fileName);

        /// <summary>
        /// Check to see if the profile directory exists.
        /// If not, create it and return false.
        /// </summary>
        /// <returns>Boolean - whether or not the directory exists.</returns>
        private static bool CheckOrCreateDirectory()
        {
            if (!Directory.Exists(PROFILE_DIRECTORY))
            {
                Directory.CreateDirectory(PROFILE_DIRECTORY);
                return false;
            }

            return true;
        }
    }
}
