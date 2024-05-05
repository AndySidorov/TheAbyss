using System.IO;
using UnityEngine;

namespace Save_System
{
    public class FileDataService: IDataService
    {
        private ISerializer _serializer;
        private string _dataPath;
        private string _fileExtension;

        public FileDataService(ISerializer serializer)
        {
            _dataPath = Application.persistentDataPath;
            _fileExtension = "json";
            _serializer = serializer;
        }

        private string GetPathToFIle(string fileName)
        {
            return Path.Combine(_dataPath, string.Concat(fileName, ".", _fileExtension));
        }

        public bool SaveExists()
        {
            var fileLocation = GetPathToFIle("Save");
            return File.Exists(fileLocation);
        }
        
        public void Save(SaveData data)
        {
            var fileLocation = GetPathToFIle("Save");
            File.WriteAllText(fileLocation, _serializer.Serialize(data));
        }

        public SaveData Load()
        {
            var fileLocation = GetPathToFIle("Save");
            return _serializer.Deserialize<SaveData>(File.ReadAllText(fileLocation));
        }
    }
}