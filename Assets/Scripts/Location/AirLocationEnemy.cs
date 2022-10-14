using MoreMountains.NiceVibrations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirLocationEnemy : MonoBehaviour
{
    [SerializeField] private float speedMove;
    [Range(0.3f, 10)]
    [SerializeField] private float newTargetTimer = 0.3f;
    [SerializeField] private GameObject BoomEffect;
    [SerializeField] private AudioSource BoomSource;




    private Vector3 targetPosition;

    void Start()
    {
        targetPosition = AirParkBattleAir.RandomPointInBounds();
        StartCoroutine("NewTargetTimer");

    }

    
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * speedMove);
    }
    private void FixedUpdate()
    {
        GetComponent<Rigidbody>().AddExplosionForce(1000f, transform.position, 1000);
    }

    IEnumerator NewTargetTimer()
	{
		while (true)
		{
			yield return new WaitForSeconds(newTargetTimer);
            targetPosition = AirParkBattleAir.RandomPointInBounds();
        }
	}
    public void Liquidation()
    {
        StopCoroutine("NewTargetTimer");
        MMVibrationManager.ContinuousHaptic(.9f, .07f, .4f);
        transform.GetChild(0).gameObject.SetActive(false);
        // transform.parent = skyBox;
        GetComponent<Collider>().isTrigger = false;
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Rigidbody>().useGravity = true;
        BoomEffect.SetActive(true);
        BoomEffect.transform.parent = null;
        BoomSource.Play();

        AirParkBattleAir.OffAnimationAllEnemies();

        AirParkBattleAir.enemies.Remove(this.gameObject);
        Destroy(this.gameObject, 2f);

        if(AirParkBattleAir.enemies.Count <= 0)
        {
            AirParkBattleAir.Victory?.Invoke();
        }
    }

    public void AimFailed()
    {
        transform.GetChild(0).GetComponent<Animation>().Play("Failed");
    }
    public void EnableAim()
    {
        transform.GetChild(0).GetComponent<Animation>().Play("StartAim");
    }
    
}
