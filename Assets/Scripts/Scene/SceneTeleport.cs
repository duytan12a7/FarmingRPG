using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTeleport : MonoBehaviour
{
    [SerializeField] private SceneName sceneNameGoto = SceneName.Scene1_Farm;
    [SerializeField] private Vector3 scenePositionGoto = new();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController playerCtrl = collision.GetComponent<PlayerController>();

        if (playerCtrl == null) return;

        float xPosition = Mathf.Approximately(scenePositionGoto.x, 0f) ? playerCtrl.transform.position.x : scenePositionGoto.x;
        float yPosition = Mathf.Approximately(scenePositionGoto.y, 0f) ? playerCtrl.transform.position.y : scenePositionGoto.y;

        float zPosition = 0f;

        SceneControllerManager.Instance.FadeAndLoadScene(sceneNameGoto.ToString(), new Vector3(xPosition, yPosition, zPosition));
    }
}
