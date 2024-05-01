using System;

namespace Save_System
{
    [Serializable] public class SaveData
    {
        public float flashDistance;
        public float flashCooldown;
        public int numberOfFlashes;
        public int numberOfEnergyDrinks;
        public int numberOfBottles;
        public string sceneName;
        
        public float playerVolume;
        public float monsterVolume;
        public float ambientVolume;
        public float mouseSensitivity;
    }
}