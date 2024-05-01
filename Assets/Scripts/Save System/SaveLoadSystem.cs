using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Save_System
{   
    [Serializable] public class GameData
    {
        public string name;
        public string currentLevelName;
        public SaveData saveData;
    }

    public interface ISaveable
    {
        SerializableGuid Id { get; set; }
    }
    
    public interface IBind<TData> where TData: ISaveable
    {
        SerializableGuid Id { get; set; }
        void Bind(TData data);
    }
    
    public class SaveLoadSystem: MonoBehaviour
    {
        [SerializeField] public GameData gameData;
        private IDataService _dataService;
        
        public static SaveLoadSystem Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        
            DontDestroyOnLoad(gameObject);

            _dataService = new FileDataService(new JsonSerializer());
        }
        
        void Start() => NewGame();

        void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
        void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

        
        void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
            Bind<DataManager, SaveData>(gameData.saveData);
        }

        void Bind<T, TData>(TData data) where T : MonoBehaviour, IBind<TData> where TData : ISaveable, new() {
            var entity = FindObjectsByType<T>(FindObjectsSortMode.None).FirstOrDefault();
            if (entity != null) {
                if (data == null) {
                    data = new TData { Id = entity.Id };
                }
                entity.Bind(data);
            }
        }
        
        public void NewGame()
        {
            gameData = new GameData
            {
                name = "New Game",
                currentLevelName = "Level 1"
            };
            SceneManager.LoadScene(gameData.currentLevelName);
        }
        
        public void SaveGame() => _dataService.Save(gameData);
        
        public void LoadGame(string gameName)
        {
            gameData = _dataService.Load(gameName);
            if (String.IsNullOrWhiteSpace(gameData.currentLevelName))
            {
                gameData.currentLevelName = "Level 1";
            }
            SceneManager.LoadScene(gameData.currentLevelName);
        }
        
        public void ReloadGame() => LoadGame(gameData.name);

        public void DeleteGame(string gameName) => _dataService.Delete(gameName);
    }
}