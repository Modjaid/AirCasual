using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Map : MonoBehaviour
{
    public UnityEvent FinishMap;

    public MetaMap YellowMap;
    public MetaMap GreenMap;
    public MetaMap GreyMap;

    [SerializeField] private Transform map_cameraStartPos;
    [SerializeField] private Transform base_cameraStartPos;
    [SerializeField] private Transform mapCentr;
    [SerializeField] private Transform baseCentr;
    [SerializeField] private Transform mapCameraUp;
    [SerializeField] private Transform baseCameraUp;

    #region UI
    [SerializeField] private TextMeshProUGUI lvlInfoText;
    [SerializeField] private Animation nextButtonAnim;
    [SerializeField] private Animation ToBaseButton;
    #endregion

    private MetaMap currentMap;

    public void StartMap(int currentLvl)
    {
        MapType type = GetMapType(currentLvl);
        lvlInfoText.text = "";
        int locationNum = GetLocationNum(currentLvl);
        switch (type)
        {
            case MapType.Yellow:
                currentMap = YellowMap;
                YellowMap.gameObject.SetActive(true);
                YellowMap.EnableScript(locationNum);
            //    lvlInfoText.text = YellowMap.mapName;
                break;
            case MapType.Green:
                currentMap = GreenMap;
                GreenMap.gameObject.SetActive(true);
                GreenMap.EnableScript(locationNum);
           //     lvlInfoText.text = GreenMap.mapName;
                break;
            case MapType.Grey:
                currentMap = GreyMap;
                GreyMap.gameObject.SetActive(true);
                GreyMap.EnableScript(locationNum);
           //     lvlInfoText.text = GreyMap.mapName;
                break;

        }
        lvlInfoText.text += " MISSION " + currentLvl.ToString();
    }


    #region UI
    public void HideUI()
    {
        lvlInfoText.gameObject.SetActive(false);
        ToBaseButton.Play("Down");
        nextButtonAnim.Play("Down");
    }
    public void GoToMap()
    {
        ToBaseButton.Play("Up");
        nextButtonAnim.Play("Up");
        lvlInfoText.gameObject.SetActive(true);
    }
    public void ClickNextButton()
    {
        FinishMap?.Invoke();
    }
    public void ResetMapTasks()
    {
        currentMap.StopCamera();
        Camera.main.GetComponent<MainCamera>().LookAtOff();
    }
    public void CameraToBaseTransition()
    {
        StartCoroutine(
            CameraTransition(mapCentr, mapCameraUp, baseCameraUp, base_cameraStartPos, baseCentr)
            );
    }
    public void CameraToMapTransition()
    {
        StartCoroutine(
            CameraTransition(baseCentr, baseCameraUp, mapCameraUp, map_cameraStartPos, mapCentr)
            );
    }
    #endregion


    public static MapType GetMapType(int CurrentLvl)
    {
        int delta = CurrentLvl - ((CurrentLvl / 24) * 24);

        if (delta < 7)
        {
            return MapType.Yellow;
        }
        else if (delta < 14)
        {
            return MapType.Green;
        }
        else return MapType.Grey;
    }
    public static int GetLocationNum(int CurrentLvl)
    {
        int delta = CurrentLvl - ((CurrentLvl / 24) * 24);

        if (delta < 7)
        {
            return delta;
        }
        else if (delta < 14)
        {
            return delta - 7;
        }
        else return delta - 14;

    }
    private IEnumerator CameraTransition(Transform centrStartPos, Transform upStartPos, Transform upEndPos, Transform downEndPos,Transform centrEndPos)
    {
        MainCamera camera = Camera.main.GetComponent<MainCamera>();
        camera.SetCameraParking(upStartPos, 0.01f);
        camera.LookAtOn(centrStartPos);

        yield return new WaitForSeconds(1f);
        camera.LookAtOn(centrEndPos);
        camera.SetPos(upEndPos);
        camera.SetCameraParking(downEndPos,0.01f);

        yield return new WaitForSeconds(1f);
        camera.LookAtOff();
    }

}

public enum MapType
{
    Yellow,
    Green,
    Grey
}
