using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class KeySpawner : MonoBehaviour
{
    [SerializeField] private List<Transform> _positions;
    [SerializeField] private Interactable _key;

    private void Awake()
    {
        Instantiate(_key, _positions[Random.Range(0, _positions.Count)].position, Quaternion.Euler(0, Random.Range(0.0f, 360.0f), 0));
    }
}
