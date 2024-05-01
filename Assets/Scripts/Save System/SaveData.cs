using System;
using UnityEngine;

namespace Save_System
{
    [Serializable]
    public class SaveData: ISaveable
    {
        [field: SerializeField] public SerializableGuid Id { get; set; }
        
        public float flashDistance;
        public float flashCooldown;
        public int numberOfFlashes;
        public int numberOfEnergyDrinks;
        public int numberOfBottles;
        
        public float playerVolume;
        public float monsterVolume;
        public float ambientVolume;
        public float mouseSensitivity;

        public string sceneName;
    }
}