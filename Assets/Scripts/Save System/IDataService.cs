namespace Save_System
{
    public interface IDataService
    {
        bool SaveExists();
        void Save(SaveData data);
        SaveData Load();
    }
}