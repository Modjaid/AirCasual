using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReactiveEngine : AirEngine
{
    [Header("Трансформы где будет инстантироваться еффекты")]
    [SerializeField] private Transform[] smokePlaces;
    [SerializeField] private Transform[] firePlaces;
    [SerializeField] private Transform[] sparklePlaces;
    [Header("Сами префабы эффектов")]
    [SerializeField] private GameObject smokeEffectPrefab;
    [SerializeField] private GameObject fireEffectPrefab;
    [SerializeField] private GameObject sparkleEffectPrefab;


    [Header("Место камеры для позиционирования")]
    public Transform cameraPlace;

    private StartEngine startEngine;

    private int maxFireCount;

    void Start()
    {
        fireEffectPrefab.GetComponent<ParticleSystem>().loop = false;
        startEngine = transform.parent.parent.GetComponent<StartEngine>();
        startEngine.NeutralPhaseEvent.AddListener(WeakMode);
        startEngine.GreenPhaseEvent.AddListener(MiddleMode);
        startEngine.RedPhaseEvent.AddListener(StrongMode);
        startEngine.EndMechanicEvent.AddListener(delegate
        {
            fireEffectPrefab.GetComponent<ParticleSystem>().loop = true;
            EffectOn(firePlaces, fireEffectPrefab, 20);
        });

        StartCoroutine(InstantiateAlwaysFire(0.1f));
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
        maxFireCount = 20;
    }

    protected override void StrongMode()
    {
        maxFireCount = 20;
        EffectOn(sparklePlaces, sparkleEffectPrefab, 2);
        EffectOn(smokePlaces, smokeEffectPrefab, 3);
    }

    protected override void WeakMode()
    {
        EffectOn(firePlaces, fireEffectPrefab, 1);
        maxFireCount = 0;
    }


    public IEnumerator InstantiateAlwaysFire(float delay)
    {
        while (true)
        {
            if (maxFireCount <= firePlaces[0].childCount)
            {
                yield return new WaitForSeconds(delay);
                continue;
            }
            foreach (Transform firePlace in firePlaces)
            {
                GameObject newFire = Instantiate(fireEffectPrefab, firePlace);
                var particle = newFire.GetComponent<ParticleSystem>();
                particle.Stop();
                var main = newFire.GetComponent<ParticleSystem>().main;
                main.duration = 2f;
                particle.Play();
            }
            yield return new WaitForSeconds(delay);
        }

    }
}
