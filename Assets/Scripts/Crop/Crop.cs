using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crop : MonoBehaviour
{
    private int _harvestActionCount = 0;

    [SerializeField]
    [Tooltip("This should be populated from the child transform gameobject showing harvest effect spawn point")]
    private Transform _harvestActionEffectTransform = null;

    [SerializeField]
    [Tooltip("This should be populated from the child gameobject")]
    private SpriteRenderer _cropHarvestedSpriteRenderer = null;
    [HideInInspector] public Vector2Int cropGridPosition;

    public static GameObject Create(
        GameObject prefab,
        Vector3 position,
        Sprite sprite,
        Transform parentTransform,
        int gridPosX,
        int gridPosY)
    {
        var cropInstance = Instantiate(prefab, position, Quaternion.identity);
        cropInstance.GetComponentInChildren<SpriteRenderer>().sprite = sprite;
        cropInstance.transform.SetParent(parentTransform);
        cropInstance.GetComponent<Crop>().cropGridPosition = new Vector2Int(gridPosX, gridPosY);
        return cropInstance;
    }
}
