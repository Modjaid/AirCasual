using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class StartBladeEngine : MonoBehaviour
{
    [Header("C какого числа начинается текущая фаза (Всего фазовых оборотов:100)")]
    [Range(0, 100f)]
    [SerializeField] private float greenPhase;
    [Range(0, 100f)]
    [SerializeField] private float redPhase;


    public UnityEvent NeutralPhaseEvent;
    public UnityEvent GreenPhaseEvent;
    public UnityEvent RedPhaseEvent;


    /// ///////////////////
    [Header("!!! FOR TEST !!!")]
    public Text TestText;
    public GameObject EnginePrefab;
    /// ///////////////////

    private float currentPhase = 0;
    public float CurrentPhase
    {
        get { return currentPhase; }
        set
        {
            currentPhase = value;

            if(currentPhase < 0)
            {
                currentPhase = 0;

            }else if(currentPhase < greenPhase)
            {
                NeutralPhaseEvent.Invoke();
            }else if(currentPhase < redPhase)
            {
                GreenPhaseEvent.Invoke();
            }else if(currentPhase < 100)
            {
                RedPhaseEvent.Invoke();
            }
            else
            {
                currentPhase = 100;
                RedPhaseEvent.Invoke();
            }

        }
    }
    private bool IsGreenPhase
    {
        get
        {
            if(currentPhase >= greenPhase && currentPhase < redPhase)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    private GameObject[] engines;// Двигатели которые найдены в EngineSlots
    private Transform[] blades;  // Лопасти в engines дочерних объектах по имени Screw

    private Vector2 startSwipePos;
    private Vector2 endSwipePos;

    private Quaternion startLeverRotation;
    private Quaternion startArrowRotation;

    private Transform tachometr;
    private Transform lever;
    private Transform arrow;

    private Action update;


    void OnEnable()
    {
      //  NeutralPhaseEvent.AddListener(() => Debug.Log("NEUTRAL"));
      //  GreenPhaseEvent.AddListener(() => Debug.Log("GREEN"));
      //  RedPhaseEvent.AddListener(() => Debug.Log("RED"));
        update = EngineController;
    }


    void Update()
    {
        update();
    }

    public void EnableScript(GameObject[] engines)
    {

        InitTachometr();
        blades = AutoGetBlades(engines);
        update = EngineController;
        this.enabled = true;
    }

    /// <summary>
    /// Отслеживает полный запуск от начала и до конца
    /// </summary>
    private void EngineController()
    {

        if (Input.GetMouseButtonDown(0))
        {
            startSwipePos = Input.mousePosition;
        }else if (Input.GetMouseButtonUp(0) && IsGreenPhase)
        {
            tachometr.parent.GetComponent<Animation>().Play("Down");
            Destroy(tachometr.parent.gameObject, 1f);
            update = FinishController;
        }
        else if (Input.GetMouseButton(0))
        {
            endSwipePos = Input.mousePosition;
            Vector2 distance = endSwipePos - startSwipePos;

            if ((distance.y * -1) > (Screen.height / 4)) // height/4 - четверть экрана нужно провести пальцем чтоб двигатель начал обороты
            {
                CurrentPhase += 20 * Time.deltaTime;
                
                //Движение рычага
                if (lever.transform.localRotation.x > -0.5f && distance.y < 0)
                {
                    lever.transform.localRotation *= Quaternion.AngleAxis(distance.y * (0.005f), Vector3.right);

                }
            }
        }
        else
        {
            CurrentPhase -= 30 * Time.deltaTime;

            //Возвращение рычага
            if (lever.transform.localRotation.x < 0.5f)
            {
                lever.transform.rotation *= Quaternion.AngleAxis(Time.deltaTime * -100f, Vector3.left);
            }
        }

        //Стрелка
        arrow.transform.localRotation = Quaternion.AngleAxis(360 / 100 * CurrentPhase, Vector3.forward);

        //Тряска Тахометра
        tachometr.localPosition = UnityEngine.Random.insideUnitSphere * (CurrentPhase * 0.000005f);

        //Пропеллеры
        foreach (Transform blade in blades)
        {
            blade.transform.Rotate(new Vector3(0, 360 / 100 * (CurrentPhase/15),0));
        }

    }


    private void FinishController()
    {
        //Пропеллеры
        foreach (Transform blade in blades)
        {
            blade.transform.Rotate(new Vector3(0, 360 / 100*5, 0));
        }
    }

    private void InitTachometr()
    {
        tachometr = Camera.main.GetComponent<MainCamera>().tachometr.transform;
        tachometr.parent.gameObject.SetActive(true);
        tachometr.GetComponent<Animation>().Play("Up");
        lever = tachometr.GetChild(1);
        arrow = tachometr.GetChild(0);

        startLeverRotation = lever.transform.rotation;
        startArrowRotation = arrow.transform.rotation;
    }

    /// <summary>
    /// Берется первый дочерний объект каждого двигателя 
    /// </summary>
    /// <returns></returns>
    private Transform[] AutoGetBlades(GameObject[] engines)
    {
        Transform[] bladeArr = new Transform[engines.Length];

        for(int i = 0; i < bladeArr.Length; i++)
        {
            bladeArr[i] = engines[i].transform.GetChild(0);
        }
        return bladeArr;
    }
}

