using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation.Examples;

public class FollowBot : MonoBehaviour
{
    public Weapon weapon;
    Outline _outline;

    [SerializeField] private GameObject BoomEffect;
    [SerializeField] private AudioSource BoomSource;

    private Transform canvas;

    private Vector3 testPos;
    private Quaternion testRot;

    void Start()
    {
        weapon.SetTargetPos(WallAirPark.RandomPointInBounds());
        _outline = GetComponent<Outline>();
        canvas = transform.GetChild(0);
        testPos = transform.position;
        testRot = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        //bool isDone = weapon.Shot();
       // if (isDone)
       // {
       //     weapon.SetTargetPos(WallAirPark.RandomPointInBounds());
       // }
        canvas.eulerAngles = new Vector3(0,90,0);
    }


    public void TurnOnOutLine()
    {
        _outline.enabled = true;
    }


    public void Liquidation()
    {
        transform.GetChild(0).gameObject.GetComponent<Animation>().Play("AllOff");
        GetComponent<Collider>().isTrigger = false;
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Rigidbody>().useGravity = true;
        Instantiate(BoomEffect, this.transform).transform.parent = null;
        BoomSource.Play();
        //  EarthBattle.OffAnimationAllEnemies();
        EarthBattle.instance.DecreaseEnemy();
        StartCoroutine(delay());
        IEnumerator delay()
        {
            yield return new WaitForSeconds(1);
            RestartPath();
            this.transform.parent.gameObject.SetActive(false);
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
    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.name == "FinishBox")
        {
            RestartPath();
            LevelManager.instance.RestartPanelUp();
            StartCoroutine(delay());
        }
        IEnumerator delay()
        {
           // EarthBattle.instance.DestroyHelicopter();
            yield return new WaitForSeconds(1);
            EarthBattle.instance.DisableBots();
        }
    }

    public void RestartPath()
    {
        GetComponent<Collider>().isTrigger = true;
        GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Rigidbody>().useGravity = false;
        transform.parent.transform.GetComponent<PathFollower>().distanceTravelled = 0;
        transform.GetChild(0).gameObject.SetActive(true);
    }
}
