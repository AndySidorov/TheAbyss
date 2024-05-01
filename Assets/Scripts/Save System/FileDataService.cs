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
        
        public void Save(GameData data)
        {
            var fileLocation = GetPathToFIle(data.name);
            File.WriteAllText(fileLocation, _serializer.Serialize(data));
        }

        public GameData Load(string name)
        {
            var fileLocation = GetPathToFIle(name);
            return _serializer.Deserialize<GameData>(File.ReadAllText(fileLocation));
        }

        public void Delete(string name)
        {
            var fileLocation = GetPathToFIle(name);
            if (File.Exists(fileLocation))
            {
                File.Delete(fileLocation);
            }
        }
    }
}