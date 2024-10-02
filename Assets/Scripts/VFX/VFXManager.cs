using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXManager : SingletonMonobehaviour<VFXManager>
{
    private WaitForSeconds waitForSeconds;
    [SerializeField] private GameObject reapingPrefab = null;

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
