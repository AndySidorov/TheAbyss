using UnityEngine;

namespace Save_System
{
    public class DataManager: MonoBehaviour, IBind<SaveData>
    {
        public static DataManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        
            DontDestroyOnLoad(gameObject);
        }
        
        [field: SerializeField] public SerializableGuid Id { get; set; } = SerializableGuid.NewGuid();
        [SerializeField] private SaveData _data;

        public void Bind(SaveData data)
        {
            _data = data;
            _data.Id = Id;
        }
    }
}