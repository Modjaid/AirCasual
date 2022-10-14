using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BladeEngine : AirEngine
{
    [Header("Трансформы где будет инстантироваться еффекты")]
    [SerializeField] private Transform[] smokePlaces;
    [SerializeField] private Transform[] sparklePlaces;
    [SerializeField] private Transform blade;
    [Header("Сами префабы эффектов")]
    [SerializeField] private GameObject smokeEffectPrefab;
    [SerializeField] private GameObject sparkleEffectPrefab;


    private StartEngine startEngine;

    private int maxFireCount;

    void Start()
    {
        startEngine = transform.parent.parent.parent.GetComponent<StartEngine>();
        startEngine.RedPhaseEvent.AddListener(StrongMode);
        startEngine.EndMechanicEvent.AddListener(delegate
        {
            
        });
    }

    private void Update()
    {
            blade.transform.Rotate(new Vector3(0, 360 / 100 * (startEngine.CurrentPhase / 15), 0));
    }
    protected override void EffectOn(Transform[] places, GameObject prefabEffect, int maxEffects)
    {
        if (places[0].childCount > maxEffects)
        {
            return;
        }
        foreach (Transform place in places)
        {
            Instantiate(prefabEffect, place);
        }
    }

    protected override void MiddleMode()
    {

    }

    protected override void StrongMode()
    {
        EffectOn(smokePlaces, smokeEffectPrefab, 3);
        EffectOn(sparklePlaces, sparkleEffectPrefab, 3);
    }

    protected override void WeakMode()
    {

    }


}
