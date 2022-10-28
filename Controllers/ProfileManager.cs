using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Delfinovin.Controllers
{
    public static class ProfileManager
    {
        private const string PROFILE_DIRECTORY = ".\\profiles";

        public static ControllerProfile[] CurrentProfiles { get; set; } = new ControllerProfile[4]
        {
            new ControllerProfile(), new ControllerProfile(),
            new ControllerProfile(), new ControllerProfile(),
        };

        /// <summary>
        /// Get a string list of all profiles currently in the profiles
        /// directory.
        /// </summary>
        /// <returns>List<string></returns>
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

        /// <summary>
        /// Load a ControllerProfile from file. If it does not exist,
        /// return a default ControllerProfile object.
        /// </summary>
        /// <param name="filePath">The file path to the JSON profile.</param>
        /// <returns>ControllerProfile</returns>
        public static ControllerProfile GetProfileFromFileName(string filePath)
        {
            ControllerProfile? profile = new ControllerProfile();

            if (!File.Exists(filePath))
                return profile;

            string jsonString = File.ReadAllText(filePath);
            profile = JsonSerializer.Deserialize<ControllerProfile>(jsonString);
            profile.ProfileName = GetProfileNameFromFileName(filePath);

            return profile;
        }

        /// <summary>
        /// Load a ControllerProfile from file. If it does not exist, return
        /// a default ControllerProfile object.
        /// </summary>
        /// <param name="profileName">The name of the profile.</param>
        /// <returns>ControllerProfile</returns>
        public static ControllerProfile GetProfileFromProfileName(string profileName)
        {
            string profilePath = Path.Combine(PROFILE_DIRECTORY, profileName);
            return GetProfileFromFileName(profilePath);
        }

        private static string GetProfileNameFromFileName(string fileName) => Path.GetFileNameWithoutExtension(fileName);

        /// <summary>
        /// Check to see if the profile directory exists.
        /// If not, create it and return false.
        /// </summary>
        /// <returns></returns>
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
