using MoreMountains.NiceVibrations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AirBomb : MonoBehaviour
{
    public UnityEvent<bool> FinishBomb;
    [SerializeField] private GameObject missExposionEffect;
    [SerializeField] private GameObject bigExposionEffect;
    [SerializeField] private Transform cameraPos1;
    [SerializeField] public Transform CameraBombPos;
    [SerializeField] public Transform CameraBombPosGreen;
    [SerializeField] private Transform lineX;
    [SerializeField] private Transform lineY;
    [Header("Скорость смещения лайонов")]
    [SerializeField] private float speedMoveLine;

    [SerializeField] private District[] leftTargets;
    [SerializeField] private District[] rightTargets;
    [SerializeField] private Transform aim;
    private District[] currentTargets;

    [Header("Облака")]
    [SerializeField] private Transform[] cloudLayers;
    [SerializeField] private float endCloudZ;
    [SerializeField] private float speedCloud;

    [Header("Земля")]
    [SerializeField] private Transform ground;
    [SerializeField] private Material greenMaterial;
    [SerializeField] private Material mountainMaterial;
    [SerializeField] private float groundSpeed;
    [SerializeField] private float endGroundZ;

    [Header("TESTDELETE")]
    public float waitStartTarget;
    private int missCount;

    [SerializeField] private AudioSource soundMiss;
    [SerializeField] private AudioSource soundHit;
    [SerializeField] private AudioSource soundBoom;
    [SerializeField] private AudioSource soundFly;

    private Vector3 groundStartPos;
    private MapType mapType;
    private Coroutine groundMoving;

    public void Start()
    {
        groundStartPos = ground.position;
        FinishBomb.AddListener(FailGame);
      //  EnableScript(testLevel, Camera.main.GetComponent<MainCamera>());
    }

    public void CloudMove()
    {
        StartCoroutine(Moving());

        IEnumerator Moving()
        {
            Vector3 startCloudPos = cloudLayers[0].localPosition;
            while (true)
            {
                foreach(Transform cloud in cloudLayers)
                {
                    cloud.Translate(Vector3.back * speedCloud * Time.deltaTime,transform.parent);
                    if(cloud.localPosition.z < endCloudZ)
                    {
                        cloud.localPosition = startCloudPos;
                    }
                }
                yield return null;
            }
        }
    }

    public void GroundMove()
    {
        ground.gameObject.SetActive(true);
        ground.position = groundStartPos;
        groundMoving = StartCoroutine(Moving());

        IEnumerator Moving()
        {
            bool next = true;
            while (next)
            {
                ground.Translate(Vector3.back * groundSpeed * Time.deltaTime, transform.parent);
                if(ground.position.z < endGroundZ)
                {
                    Debug.Log("КОНЕЦ ЗЕМЛИ");
                    FinishBomb?.Invoke(false);
                    next = false;
                }

                yield return null;
            }
        }
    }

    private void AimControl()
    {
        int currentLineIndex = -1;
        Vector2[] startLines;
        Vector2[] endLines;
        Coroutine lineMoving;
        Transform lineObject = lineX;


        InitLines();
        NextLine();
        lineMoving = StartCoroutine(LineMoving());
        StartCoroutine(ClickControl());


        IEnumerator AutoShotTimer()
        {
            yield return new WaitForSeconds(25);
      
            StopCoroutine(lineMoving);
            NextLine();
            soundHit.Play();
            StartCoroutine(Shot());
            lineMoving = StartCoroutine(LineMoving());
        }

        IEnumerator ClickControl()
        {
            while (true)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    StopCoroutine(lineMoving);
                    bool MayShot = NextLine();
                    soundHit.Play();
                    if (MayShot)
                    {
                        StartCoroutine(Shot());
                        MMVibrationManager.ContinuousHaptic(.9f, .07f, .4f);
                    }
                    lineMoving = StartCoroutine(LineMoving());
                }
                yield return null;
            }
        }

        IEnumerator LineMoving()
        {
            while (true)
            {
                while (Vector3.Distance(lineObject.localPosition, endLines[currentLineIndex]) > 150)
                {
                    lineObject.localPosition = Vector3.Lerp(lineObject.localPosition, endLines[currentLineIndex], speedMoveLine * Time.deltaTime);
                    yield return null;
                }
                lineObject.localPosition = startLines[currentLineIndex];

                yield return null;
            }
        }

        #region Возможно понадобитсья
        /*
        IEnumerator LineMovingLoop()
        {
            bool comeback = false;
            while (true)
            {
                Vector2 startLine;
                Vector2 endLine;

                if (line == lineX)
                {
                    if (!comeback)
                    {
                        startLine = new Vector2(rangeStart, line.localPosition.y);
                        endLine = new Vector2(rangeEnd, line.localPosition.y);
                        line.localPosition = startLine;
                        comeback = true;
                    }
                    else
                    {
                        startLine = line.localPosition;
                        endLine = new Vector2(rangeStart, line.localPosition.y);
                        comeback = false;
                    }
                }
                else
                {
                    if (!comeback)
                    {
                        startLine = new Vector2(line.localPosition.x, targets[currentTargetIndex].rangeLineY.start);
                        endLine = new Vector2(line.localPosition.x, targets[currentTargetIndex].rangeLineY.end);
                        line.localPosition = startLine;
                        comeback = true;
                    }
                    else
                    {
                        startLine = line.localPosition;
                        endLine = new Vector2(line.localPosition.x, targets[currentTargetIndex].rangeLineY.start);
                        comeback = false;
                    }
                }

                while (Vector3.Distance(line.localPosition, endLine) > 15f)
                {
                    line.localPosition = Vector3.Lerp(line.localPosition, endLine, speedMoveLine * Time.deltaTime);
                    yield return null;
                }
            }
        }
*/
        #endregion

        void InitLines()
        {
            startLines = new Vector2[currentTargets.Length * 2];
            endLines = new Vector2[currentTargets.Length * 2];

            for (int i = 0; i < currentTargets.Length * 2;)
            {
                // LINE X
                startLines[i] = new Vector2(currentTargets[i / 2].transformCenter.localPosition.x - currentTargets[i / 2].rangeLineX, lineX.localPosition.y);
                endLines[i] = new Vector2(currentTargets[i / 2].transformCenter.localPosition.x + currentTargets[i / 2].rangeLineX, lineX.localPosition.y);

                i++;

                //LINE Y
                startLines[i] = new Vector2(lineY.localPosition.x, currentTargets[i / 2].transformCenter.localPosition.y - currentTargets[i / 2].rangeLineY);
                endLines[i] = new Vector2(lineY.localPosition.x, currentTargets[i / 2].transformCenter.localPosition.y + currentTargets[i / 2].rangeLineY);

                i++;
            }
        }

        IEnumerator Shot()
        {
            aim.localPosition = new Vector2(lineX.localPosition.x, lineY.localPosition.y);
            aim.gameObject.SetActive(true);
            if (currentTargets[(currentLineIndex/2) - 1].Contain(aim.localPosition))
            {
                ///////////////////////////////////////////////////////// ПОПАДАНИЕ //////////////////////////////////////////////
                aim.GetComponent<Image>().color = Color.green;
                soundHit.Play();
                yield return new WaitForSeconds(0.5f);
                soundBoom.Play();
                soundFly.volume = 0.2f;
                Instantiate(bigExposionEffect, aim).transform.parent = ground.transform;
            }
            else
            {
                ///////////////////////////////////////////////////////// ПРОМАХ //////////////////////////////////////////////
                aim.GetComponent<Image>().color = Color.red;
                soundMiss.Play();
                yield return new WaitForSeconds(0.5f);
                soundBoom.Play();
                soundFly.volume = 0.2f;
                Instantiate(missExposionEffect, aim).transform.parent = ground.transform;
                missCount--;
                if(missCount == 0)
                {
                    FinishBomb?.Invoke(false);
                }
            }
            aim.gameObject.SetActive(false);
            currentTargets[(currentLineIndex / 2) - 1].transformCenter.gameObject.SetActive(false);
        }

        // Возвращает true если можно стрелять и две линии были пройдены
        bool NextLine()
        {
            bool CanShot = false;
            currentLineIndex++;
            if (currentLineIndex / 2 > currentTargets.Length - 1)
            {
                Debug.Log("FINISH");
                FinishBomb?.Invoke(true);
                CanShot = true;
                lineX.gameObject.SetActive(false);
                lineY.gameObject.SetActive(false);
                return CanShot;
            }



            if (currentLineIndex % 2 == 0)
            {
                lineX.gameObject.SetActive(false);
                lineY.gameObject.SetActive(false);
                lineObject = lineX;
                currentTargets[currentLineIndex / 2].transformCenter.gameObject.SetActive(true);
                CanShot = true;
            }
            else
            {
                lineObject = lineY;
            }

            lineObject.gameObject.SetActive(true);
            return CanShot;
        }
    }


    public void EnableScript(int CurrentLevel,MainCamera camera)
    {
        this.enabled = true;
        mapType = Map.GetMapType(CurrentLevel);
        missCount = 2;
        StartCoroutine(Initialiation());

        IEnumerator Initialiation()
        {
            var parent = camera.transform.parent;
            camera.transform.parent = null;
            parent.parent = camera.transform;
            camera.SetPos(cameraPos1);
            yield return new WaitForSeconds(0.5f);
            switch (mapType)
            {
                case MapType.Yellow:
                    currentTargets = rightTargets;
                    camera.SetCameraParking(CameraBombPos, 0.01f);
                    break;
                case MapType.Green:
                    ground.GetComponent<MeshRenderer>().material = greenMaterial;
                    camera.SetCameraParking(CameraBombPosGreen, 0.01f);
                    currentTargets = leftTargets;
                    break;
                case MapType.Grey:
                    ground.GetComponent<MeshRenderer>().material = mountainMaterial;
                    camera.SetCameraParking(CameraBombPos, 0.01f);
                    currentTargets = rightTargets;
                    break;
            }

            CloudMove();
            GroundMove();
            yield return new WaitForSeconds(1);
            AimControl();
        }
    }
    public void TestEnable()
    {
        CloudMove();
        GroundMove();
        AimControl();
    }

    public void Restart()
    {
        StopAllCoroutines();
        GroundMove();
        AimControl();
        CloudMove();
        missCount = 2;
        lineY.gameObject.SetActive(false);
    }

    private void FailGame(bool IsVictory)
    {
        if (!IsVictory) LevelManager.instance.RestartPanelUp();
    }


    [Serializable]
    public struct District
    {
        public string name;
        public float wideX;
        public float wideY;
        public float rangeLineX;
        public float rangeLineY;
        public Transform transformCenter;

        public bool Contain(Vector2 aim)
        {
            bool result = false;
            var center = transformCenter.localPosition;
            var X1 = center.x - wideX;
            var Y1 = center.y - wideY;
            var X2 = center.x - wideX;
            var Y2 = center.y + wideY;
            var X3 = center.x + wideX;
            var Y3 = center.y + wideY;
            var X4 = center.x + wideX;
            var Y4 = center.y - wideY;

            if(aim.x > X1 && aim.y > Y1 && aim.x < X3 && aim.y < Y3)
            {
                
                result = true;
            }
          //  Debug.Log($"if aimX({aim.x}) > X1({X1}) && aim.y({aim.y}) > Y1({Y1}) && aim.x({aim.x}) < X3({X3}) && aim.y({aim.y}) < Y3({Y3})" +
          //      $"\n {result}");

            return result;
        }
    }

    [Serializable]
    public struct Diapason
    {
        public float start;
        public float end;
    }
}
