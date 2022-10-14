using cakeslice;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MetaMap : MonoBehaviour
{
    [SerializeField] private Location[] locations;
    [SerializeField] private Transform airModel;
    [SerializeField] private float speedMove;
    public Transform cameraStartPos;
    public Transform cameraAirPos;
    public Material grayMaterial;
    public string mapName;

    private Coroutine cameraCoroutine;

    /// <summary>
    /// LocationNum четко по массиву локаций, в LevelManager нужно определить какую локацию выбрать
    /// </summary>
    public void EnableScript(int locationNum)
    {
        Vector3 nextPos = locations[locationNum].point.position;
        GameObject prevOutline;
        Vector3 startPos;

        if (locationNum == 0)
        {
            startPos = locations[locations.Length - 1].point.position;
            prevOutline = locations[locations.Length - 1].line;
        }
        else
        {
            startPos = locations[locationNum - 1].point.position;
            prevOutline = locations[locationNum - 1].line;
            ChangeColorOldLocations(locationNum);
        }

        airModel.position = startPos;
        var halfDistance = Vector3.Distance(airModel.position, nextPos) / 2;
        float restDist = Vector3.Distance(airModel.position, nextPos);

        StartCoroutine(MoveAirModel());
        StartCoroutine(LocationController());
        cameraCoroutine = StartCoroutine(CameraController());
        IEnumerator LocationController()
        {

            prevOutline.GetComponent<NewOutline>().eraseRenderer = false;
            yield return new WaitUntil(() => restDist < halfDistance);
            prevOutline.GetComponent<NewOutline>().eraseRenderer = true;
            locations[locationNum].line.GetComponent<NewOutline>().eraseRenderer = false;
        }


        IEnumerator MoveAirModel()
        {
            bool IsPlay = true;

            while (true)
            {
                airModel.position = Vector3.Lerp(airModel.position, nextPos, speedMove * Time.deltaTime);
                airModel.LookAt(nextPos);
                restDist = Vector3.Distance(airModel.position, nextPos);

                // Плавный поворот к цели 
                /*
                Vector3 dir = nextPos - airModel.position;
                Quaternion rot = Quaternion.LookRotation(dir);
                airModel.rotation = Quaternion.Lerp(airModel.rotation, rot, (speedMove * Time.deltaTime) / 2);
                */
                yield return null;
            }
        }

        IEnumerator CameraController()
        {
            
            var mainCamera = Camera.main.GetComponent<MainCamera>();
            // yield return new WaitForSeconds(1.5f);

              mainCamera.SetCameraParking(cameraAirPos,0.01f);
          //  mainCamera.SetPos(cameraAirPos);
            mainCamera.LookAtOn(airModel);

            yield return new WaitForSeconds(3);
            mainCamera.LookAtOff();
            mainCamera.SetCameraParking(cameraStartPos, 0.01f);
        }
    }

    public void clickTest(int lvl)
    {
        StopAllCoroutines();
        EnableScript(lvl);
    }
    private void ChangeColorOldLocations(int locationNum)
    {
        for(int i = 1; i < locationNum + 1; i++)
        {
            Material[] vaterials = locations[i - 1].line.GetComponent<MeshRenderer>().materials;
            vaterials[1] = grayMaterial;
            locations[i - 1].line.GetComponent<MeshRenderer>().materials = vaterials;
        }
    }

    public void StopCamera()
    {
        StopCoroutine(cameraCoroutine);
    }



    [Serializable]
    internal class Location
    {
        public Transform point;
        public GameObject line;
    }


}
