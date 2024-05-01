using System.Collections;
using System.Collections.Generic;

namespace Save_System
{
    public interface IDataService
    {
        void Save(GameData data);
        GameData Load(string name);
        void Delete(string name);
    }
}