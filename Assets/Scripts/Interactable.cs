using UnityEngine;

public class Interactable : MonoBehaviour
{
    public string itemName;
    
    public void OnInteraction()
    {
        Destroy(gameObject);
    }
}
