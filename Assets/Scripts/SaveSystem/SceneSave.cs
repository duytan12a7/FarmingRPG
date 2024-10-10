using System.Collections.Generic;

[System.Serializable]
public class SceneSave
{
    public Dictionary<string, bool> boolDictionary;
    public List<SceneItem> sceneItems;
    public Dictionary<string, GridPropertyDetails> gridPropertyDetailsDictionary;

    public SceneSave() { }

    public SceneSave(List<SceneItem> sceneItems)
    {
        this.sceneItems = sceneItems;
    }

    public SceneSave(Dictionary<string, GridPropertyDetails> gridPropertyDetails)
    {
        gridPropertyDetailsDictionary = gridPropertyDetails;
    }
}
