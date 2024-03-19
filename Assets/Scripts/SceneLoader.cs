using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField, Scene] private string _sceneName;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var player = other.gameObject.GetComponent<PlayerInteractions>();
            PlayerPrefs.SetFloat("Flash Distance", player.FlashDistance);
            PlayerPrefs.SetFloat("Flash Cooldown", player.FlashCooldown);
            PlayerPrefs.SetInt("Number of Flashes", player.NumberOfFlashes);
            PlayerPrefs.SetInt("Number of Energy Drinks", player.NumberOfEnergyDrinks);
            PlayerPrefs.SetInt("Number of Bottles", player.NumberOfBottles);
            PlayerPrefs.SetString("Level", _sceneName);
            SceneManager.LoadScene(_sceneName, LoadSceneMode.Single);
        }
    }
}
