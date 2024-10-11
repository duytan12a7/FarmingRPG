public interface ISaveable
{
    string ISaveableUniqueID { get; set; }

    GameObjectSave GameObjectSave { get; set; }

    void IRegisterSaveable();

    void IDeregisterSaveable();

    void IStoreSceneData(string sceneName);

    void IRestoreSceneData(string sceneName);

    GameObjectSave ISaveData();
    void ILoadData(GameSave gameSave);
}