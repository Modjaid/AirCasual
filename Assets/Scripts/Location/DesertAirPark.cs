using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DesertAirPark : AirPark
{
    public UnityEvent<bool> FinishTrack;

    public Transform helicopterPlatform;
    public Transform planePlatform;

    [SerializeField] private GameObject[] maps;
    [SerializeField] private Material stdMaterial;
    [SerializeField] private DragHandler dragHandler;

    public BezierTest planePath;

    public Enemy[] enemies;

    public GameObject textDeathCount;
    public GameObject animCoinPrefab;
    [SerializeField] private int countNum;
    private int CountNum;
    private bool IsPlane;
    private int timerTrack;

    public static Transform selectedPlatform;
    public DesertAutoWeapon[] rocketSpawns;

    public void Start()
    {
        foreach(Enemy enemy in enemies)
        {
            enemy.OnDeath += OnDeathCounter;
        }
        CountNum = countNum;
        textDeathCount.GetComponent<Text>().text = CountNum.ToString();
        FinishTrack.AddListener(FailGame);
    }

    private void OnDeathCounter(Enemy enemy)
    {
        CountNum--;
        textDeathCount.GetComponent<Text>().text = CountNum.ToString();
        textDeathCount.GetComponent<Animation>().Play("TextUp");
        var coin = Instantiate(animCoinPrefab,enemy.transform).transform.parent = null;
        if(CountNum == 0)
        {
            StopAllCoroutines();
            Debug.Log("AirPlatform OFF");
            selectedPlatform.GetComponent<AirPlatform>().enabled = false;
            FinishTrack?.Invoke(true);
            selectedPlatform.GetComponent<AirPlatform>().OffWeaponShot();
        }
    }
    private void SetTimerTrack(int seconds)
    {
        StartCoroutine(TimerTrack());

        IEnumerator TimerTrack()
        {
            yield return new WaitForSeconds(seconds);
            FinishTrack?.Invoke(false);
        }
    }

    public void StartTrack(int currentLvl, GameObject air, bool IsPlane, int timerTrack)
    {
        MapType mapType = Map.GetMapType(currentLvl);
        this.IsPlane = IsPlane;
        this.timerTrack = timerTrack;
        Transform airPlace;

        maps[(int)mapType].gameObject.SetActive(true);

        if (IsPlane)
        {
            airPlace = planePlatform;
            SetTimerTrack(timerTrack);
        }
        else
        {
            airPlace = helicopterPlatform;
            SetTimerTrack(timerTrack + 2);
        }
        selectedPlatform = airPlace;

        air.transform.position = airPlace.position;
        air.transform.rotation = airPlace.rotation;
        air.transform.parent = airPlace.transform;
    }


    private void FailGame(bool IsVictory)
    {
        if (!IsVictory)
        {
            CountNum = 20;
            textDeathCount.GetComponent<Text>().gameObject.SetActive(false);
            LevelManager.instance.RestartPanelUp();
            selectedPlatform.GetComponent<AirPlatform>().OffWeaponShot();
            selectedPlatform.GetComponent<AirPlatform>().enabled = false;
        }
    }

    public void Restart()
    {
        StopAllCoroutines();
        SetTimerTrack(timerTrack);
        if (IsPlane)
        {
            planePath.t = 0;
        }
        else
        {
            helicopterPlatform.GetComponent<Animation>().Stop();
            helicopterPlatform.GetComponent<Animation>().Play("Track1");
        }

        CountNum = countNum;
        textDeathCount.GetComponent<Text>().gameObject.SetActive(true);
        textDeathCount.GetComponent<Text>().text = CountNum.ToString();
        foreach (Enemy enemy in enemies)
        {
            enemy.health = 6;
            enemy.gameObject.SetActive(true);
        }
        selectedPlatform.GetComponent<AirPlatform>().enabled = true;
        //   foreach(DesertAutoWeapon rocketSpawn in rocketSpawns)
        //   {
        //       rocketSpawn.Restart();
        //   }

        //     selectedPlatform.GetComponent<AirPlatform>().healthData.RoundPoints = selectedPlatform.GetComponent<AirPlatform>().healthPlatform;
        //     selectedPlatform.GetComponent<AirPlatform>().healthSlideBar.fillAmount = 1;
    }

}
