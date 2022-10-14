using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private Animation handAnim;
    [SerializeField] private Animation leftArrowGradient;
    [SerializeField] private Animation rightArrowGradient;
    [SerializeField] private GameObject paintArrows;
    [SerializeField] private GameObject paintSpray;
    [SerializeField] private float sprayRotationZ;
    [SerializeField] private float sprayMoveSpeed;
    [Header("Дочерний объект Tutorial")]
    [SerializeField] private Animation mainAnim;


    [Header("Таймер отсутствия нажатий на экран для повторного запуска туториала")]
    public float awaitTimer;

    public bool paintTutorial;
    public bool BattleAirTutorial;
    public bool CatapultTutorial;
    public bool startEngineTutorial;

    private Action update;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {


    }

    public void PaintFingerOn()
    {
        GameObject spray = paintSpray;
        mainAnim.gameObject.SetActive(true);
        if (MenuColorUI.IsTexturing)
        {
            spray.transform.GetChild(1).GetComponent<Image>().color = Color.white;
        }
        else
        {
            spray.transform.GetChild(1).GetComponent<Image>().color = MenuColorUI.selectColor;
        }

        if (paintTutorial)
        {
            StartCoroutine(StartTutorial());
            StartCoroutine(ClickListener());
        }

       // paintArrows.SetActive(true);
        StartCoroutine(MoveSpray());

        IEnumerator rotationListener()
        {
            while (true)
            {
                if (Input.GetMouseButton(0))
                {

                    if (Input.mousePosition.x > (Screen.width / 2 + Screen.width / 3))
                    {
                        rightArrowGradient.Play("Gradient");
                        leftArrowGradient.Stop();
                    }
                    else if (Input.mousePosition.x < (Screen.width / 2 - Screen.width / 3))
                    {
                        leftArrowGradient.Play("Gradient");
                        rightArrowGradient.Stop();
                    }
                    else
                    {
                        leftArrowGradient.Stop();
                        rightArrowGradient.Stop();
                    }
                }
                else
                {
                    leftArrowGradient.Stop();
                    rightArrowGradient.Stop();
                }
                yield return null;
            }
        }

        IEnumerator ClickListener()
        {
            yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
            handAnim.Play("Down");
            yield return new WaitForSeconds(2f);
            handAnim.gameObject.SetActive(false);
        }

        IEnumerator StartTutorial()
        {
            float timer = 0;
            yield return new WaitForSeconds(0.1f);
            while (true)
            {
                if (Input.GetMouseButton(0))
                {
                    timer = awaitTimer;
                }
                timer -= Time.deltaTime;

                if(timer <= 0)
                {
                    handAnim.gameObject.SetActive(true);
                    handAnim.Play("Up");
                    mainAnim.Play("Paint");
                    StartCoroutine(ClickListener());
                }

                yield return null;
            }
        }

        IEnumerator MoveSpray()
        {
            yield return new WaitForSeconds(0.2f);
            var animSpray = spray.GetComponent<Animation>();
            var gradient = spray.transform.GetChild(0);
            gradient.gameObject.SetActive(false);
            var startPos = spray.transform.position;
            var targetPos = startPos;
            var startRot = spray.transform.rotation;
            var targetRot = startRot;

            spray.SetActive(true);
            animSpray.Play("SprayUp");
            gradient.GetComponent<Image>().color = MenuColorUI.selectColor;
          //  spray.transform.GetChild(1).GetComponent<Image>().color = MenuColorUI.selectColor;
            yield return new WaitForSeconds(0.3f);

            while (true)
            {
                if (Input.GetMouseButton(0))
                {
                    gradient.gameObject.SetActive(true);
                    animSpray.Play("SprayClick");
                    targetPos = Input.mousePosition;
                    targetRot = Quaternion.Euler(0, 0, sprayRotationZ);

                }else if (Input.GetMouseButtonUp(0))
                {
                    gradient.gameObject.SetActive(false);
                    targetPos = startPos;
                    targetRot = startRot;
                }

                spray.transform.position = Vector3.Lerp(spray.transform.position, targetPos, Time.deltaTime * sprayMoveSpeed);
                spray.transform.rotation = Quaternion.Lerp(spray.transform.rotation, targetRot, Time.deltaTime * sprayMoveSpeed);
                yield return null;
            }
        }
    }


    public void PaintFingerOff()
    {
        mainAnim.gameObject.SetActive(false);
        StopAllCoroutines();
    }

    public void BattleAirOn()
    {
        if (BattleAirTutorial)
        {
            StartCoroutine(StartTutorial());
            StartCoroutine(ClickListener());
        }

        IEnumerator ClickListener()
        {
            yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
            mainAnim.Play("BattleAirDown");
            yield return new WaitForSeconds(2f);
            mainAnim.gameObject.SetActive(false);
        }

        IEnumerator StartTutorial()
        {
            float timer = 0;
            yield return new WaitForSeconds(0.1f);
            while (true)
            {
                if (Input.GetMouseButton(0))
                {
                    timer = awaitTimer;
                }
                timer -= Time.deltaTime;

                if (timer <= 0)
                {
                    mainAnim.gameObject.SetActive(true);
                    mainAnim.Play("BattleAirUp");
                    StartCoroutine(ClickListener());
                }

                yield return null;
            }
        }
    }

    public void BattleAirOff()
    {
        mainAnim.gameObject.SetActive(false);
        StopAllCoroutines();
    }

    public void CatapultOn()
    {
        if (CatapultTutorial)
        {
            StartCoroutine(StartTutorial());
            StartCoroutine(ClickListener());
        }

        IEnumerator ClickListener()
        {
            yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
            handAnim.Play("Down");
            yield return new WaitForSeconds(2f);
            mainAnim.gameObject.SetActive(false);
        }

        IEnumerator StartTutorial()
        {
            float timer = 0;
            yield return new WaitForSeconds(0.1f);
            while (true)
            {
                if (Input.GetMouseButton(0))
                {
                    timer = awaitTimer;
                }
                timer -= Time.deltaTime;

                if (timer <= 0)
                {
                    mainAnim.gameObject.SetActive(true);
                    mainAnim.Play("Catapult");
                    StartCoroutine(ClickListener());
                }

                yield return null;
            }
        }
    }

    public void CatapultOff()
    {
        mainAnim.gameObject.SetActive(false);
        StopAllCoroutines();

    }

    public void StartEngineOn()
    {
        if (startEngineTutorial)
        {
            StartCoroutine(StartTutorial());
            StartCoroutine(ClickListener());
        }

        IEnumerator ClickListener()
        {
            yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
            mainAnim.Play("StartEngineDown");
            yield return new WaitForSeconds(2f);
            mainAnim.gameObject.SetActive(false);
        }

        IEnumerator StartTutorial()
        {
            float timer = 0;
            yield return new WaitForSeconds(0.1f);
            while (true)
            {
                if (Input.GetMouseButton(0))
                {
                    timer = awaitTimer;
                }
                timer -= Time.deltaTime;

                if (timer <= 0)
                {
                    mainAnim.gameObject.SetActive(true);
                    mainAnim.Play("StartEngine");
                    StartCoroutine(ClickListener());
                }

                yield return null;
            }
        }
    }

    public void StartEngineOff()
    {
        mainAnim.gameObject.SetActive(false);
        StopAllCoroutines();

    }
}
