using Save_System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField, Scene] private string _sceneName; // Следующая сцена 

    private void OnTriggerEnter(Collider other) // Загрузить следующий уровень и сохранить прогресс
    {
        if (other.CompareTag("Player"))
        {
            var player = other.gameObject.GetComponent<PlayerInteractions>();

            var saveLoadSystem = SaveLoadSystem.Instance;
            saveLoadSystem.data.flashDistance = player.FlashDistance;
            saveLoadSystem.data.flashCooldown = player.FlashCooldown;
            saveLoadSystem.data.numberOfFlashes = player.NumberOfFlashes;
            saveLoadSystem.data.numberOfEnergyDrinks = player.NumberOfEnergyDrinks;
            saveLoadSystem.data.numberOfBottles = player.NumberOfBottles;
            saveLoadSystem.data.sceneName = _sceneName;
            saveLoadSystem.SaveGame();
            
            SceneManager.LoadScene(_sceneName, LoadSceneMode.Single);
        }
    }
}
