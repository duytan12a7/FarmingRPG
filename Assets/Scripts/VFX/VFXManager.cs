using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXManager : SingletonMonobehaviour<VFXManager>
{
    private WaitForSeconds waitForSeconds;
    [SerializeField] private GameObject reapingPrefab = null;
    [SerializeField] private GameObject deciduousLeavesFallingPrefab = null;
    [SerializeField] private GameObject pineConesFallingPrefab = null;
    [SerializeField] private GameObject choppingTreeTrunkPrefab = null;

    protected override void Awake()
    {
        base.Awake();

        waitForSeconds = new WaitForSeconds(2f);
    }

    private void OnEnable()
    {
        EventHandler.HarvestActionEffectEvent += DisplayHarvestActionEffect;
    }

    private void OnDisable()
    {
        EventHandler.HarvestActionEffectEvent -= DisplayHarvestActionEffect;
    }

    private void DisplayHarvestActionEffect(Vector3 effectPosition, HarvestActionEffect harvestActionEffect)
    {
        switch (harvestActionEffect)
        {
            case HarvestActionEffect.reaping:
                GameObject reapingEffect = PoolManager.Instance.ReuseObject(reapingPrefab, effectPosition, Quaternion.identity);
                if (reapingEffect == null) break;

                StartCoroutine(DisableHarvestActionEffect(reapingEffect, waitForSeconds));
                break;
            case HarvestActionEffect.deciduousLeavesFalling:
                GameObject deciousLeavesFalling = PoolManager.Instance.ReuseObject(deciduousLeavesFallingPrefab, effectPosition, Quaternion.identity);
                if (deciousLeavesFalling == null) break;

                StartCoroutine(DisableHarvestActionEffect(deciousLeavesFalling, waitForSeconds));
                break;
            case HarvestActionEffect.choppingTreeTrunk:
                GameObject choppingTreeTrunk = PoolManager.Instance.ReuseObject(choppingTreeTrunkPrefab, effectPosition, Quaternion.identity);
                if (choppingTreeTrunk == null) break;

                StartCoroutine(DisableHarvestActionEffect(choppingTreeTrunk, waitForSeconds));
                break;
            case HarvestActionEffect.pineConesFalling:
                GameObject pineConesFalling = PoolManager.Instance.ReuseObject(pineConesFallingPrefab, effectPosition, Quaternion.identity);
                if (pineConesFalling == null) break;

                StartCoroutine(DisableHarvestActionEffect(pineConesFalling, waitForSeconds));
                break;
            case HarvestActionEffect.none:
                break;
            default:
                break;
        }
    }

    private IEnumerator DisableHarvestActionEffect(GameObject effectGameOject, WaitForSeconds waitForSeconds)
    {
        yield return waitForSeconds;
        effectGameOject.SetActive(false);
    }
}
