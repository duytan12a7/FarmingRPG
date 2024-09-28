using System.Collections.Generic;

[System.Serializable]
public class SceneSave
{
    public List<SceneItem> sceneItems;
    public Dictionary<string, GridPropertyDetails> gridPropertyDetailsDictionary;
}
