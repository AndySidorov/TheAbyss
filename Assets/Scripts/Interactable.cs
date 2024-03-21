using UnityEngine;

public class Interactable : MonoBehaviour
{
    public string itemName;
    
    // Удалить объект после взаимодействия с ним
    public void OnInteraction()
    {
        Destroy(gameObject);
    }
}
