using System;
using UnityEngine;

namespace Save_System
{   
    public class SaveLoadSystem: MonoBehaviour
    {
        [SerializeField] public SaveData data;
        private IDataService _dataService;

        [SerializeField] private PlayerData _playerData;
        
        public static SaveLoadSystem Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        
            DontDestroyOnLoad(gameObject);

            _dataService = new FileDataService(new JsonSerializer());

            if (!_dataService.SaveExists())
            {
                CreateSave();
            }
            
            data = _dataService.Load();
        }

        private void CreateSave()
        {
            data = new SaveData
            {
                flashDistance = _playerData.FlashDistance,
                flashCooldown = _playerData.FlashCooldown,
                numberOfFlashes = _playerData.NumberOfFlashes,
                numberOfEnergyDrinks = _playerData.NumberOfEnergyDrinks,
                numberOfBottles = _playerData.NumberOfBottles,
                playerVolume = 0,
                monsterVolume = 0,
                ambientVolume = 0,
                mouseSensitivity = _playerData.MouseSensitivity,
                sceneName = "Level 1"
            };
            _dataService.Save(data);
        }
        
        public void SaveGame() => _dataService.Save(data);
    }
}