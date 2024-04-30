using System.Collections;
using UnityEngine;

public class CoroutineStarter : MonoBehaviour
{
    public static CoroutineStarter Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        DontDestroyOnLoad(gameObject);
    }
    
    public void StartRoutine(IEnumerator enumerator)
    {
        Instance.StartCoroutine(enumerator);
    } 
    
}
