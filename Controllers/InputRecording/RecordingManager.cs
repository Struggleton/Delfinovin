using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace Delfinovin.Controllers
{
    /// <summary>
    /// A class that provides functionality to create, delete and read recordings.
    /// </summary>
    public static class RecordingManager
    {
        private const string RECORDING_DIRECTORY = ".\\recordings";

        /// <summary>
        /// Get a list of recording names saved.
        /// </summary>
        /// <returns>List<string> - A list of recording names from the recording directory</returns>
        public static List<string> GetRecordingNameList()
        {
            List<string> recordings = new List<string>();
            CheckOrCreateDirectory();

            string[] fileNames = Directory.GetFiles(RECORDING_DIRECTORY, "*.json");
            foreach (string fileName in fileNames)
            {
                recordings.Add(GetRecordingNameFromFileName(fileName));
            }

            return recordings;
        }

        /// <summary>
        /// Check to see if the profile directory exists.
        /// If not, create it and return false.
        /// </summary>
        /// <returns>Boolean - whether or not the directory exists.</returns>
        private static bool CheckOrCreateDirectory()
        {
            if (!Directory.Exists(RECORDING_DIRECTORY))
            {
                Directory.CreateDirectory(RECORDING_DIRECTORY);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Load a Recording from file. If it does not exist,
        /// return a default Recording object.
        /// </summary>
        /// <param name="filePath">The file path to the JSON recording.</param>
        /// <returns>Recording - either the loaded recording or a default recording.</returns>
        public static Recording GetRecordingFromFileName(string filePath)
        {
            Recording recording = new Recording();

            if (!File.Exists(filePath))
                return recording;

            string jsonString = File.ReadAllText(filePath);
            recording = JsonConvert.DeserializeObject<Recording>(jsonString);
            recording.RecordingName = GetRecordingNameFromFileName(filePath);

            return recording;
        }

        /// <summary>
        /// Load a Recording from a recording name. If it does not exist, return
        /// a default Recording object.
        /// </summary>
        /// <param name="recordingName">The name of the profile.</param>
        /// <returns>ControllerProfile</returns>
        public static Recording GetRecordingFromRecordingName(string recordingName)
        {
            string recordingPath = Path.Combine(RECORDING_DIRECTORY, recordingName + ".json");
            return GetRecordingFromFileName(recordingPath);
        }

        private static string GetRecordingNameFromFileName(string fileName) => Path.GetFileNameWithoutExtension(fileName);

        /// <summary>
        /// Try to find a recording by its recording name.
        /// If it exists, return true and delete it.
        /// </summary>
        /// <param name="recordingName">The name of the recording to be deleted.</param>
        /// <returns>Boolean - whether or not the recording was deleted properly.</returns>
        public static bool DeleteRecordingByRecordName(string recordingName)
        {
            string recordingPath = Path.Combine(RECORDING_DIRECTORY, recordingName + ".json");
            if (!File.Exists(recordingPath))
                return false;

            File.Delete(recordingPath);
            return true;
        }

        /// <summary>
        /// Save a recording to a file.
        /// </summary>
        /// <param name="recording">The recording being saved to a file.</param>
        /// <param name="recordingName">The name for the recording.</param>
        public static void SaveRecording(Recording recording, string recordingName)
        {
            string jsonString = JsonConvert.SerializeObject(recording, Formatting.Indented);
            CheckOrCreateDirectory();
            File.WriteAllText(Path.Combine(RECORDING_DIRECTORY, recordingName + ".json"), jsonString);
        }
    }
}
