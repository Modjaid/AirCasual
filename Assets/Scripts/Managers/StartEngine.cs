using MoreMountains.NiceVibrations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StartEngine : MonoBehaviour
{
    [Header("C какого числа начинается текущая фаза (Всего фазовых оборотов:100)")]
    [Range(0, 100f)]
    [SerializeField] private float greenPhase;
    [Range(0, 100f)]
    [SerializeField] private float redPhase;
    public Transform enginePlace;

    [Header("Камера смотрит на нозлы")]
    public Transform cameraPlace;

    public UnityEvent NeutralPhaseEvent;
    public UnityEvent GreenPhaseEvent;
    public UnityEvent RedPhaseEvent;
    public UnityEvent EndMechanicEvent;

    private float currentPhase = 0;
    public float CurrentPhase
    {
        get { return currentPhase; }
        set
        {
            currentPhase = value;

            if (currentPhase < 0)
            {
                currentPhase = 0;

            }
            else if (currentPhase < greenPhase)
            {
                NeutralPhaseEvent.Invoke();
            }
            else if (currentPhase < redPhase)
            {
                GreenPhaseEvent.Invoke();
            }
            else if (currentPhase < 100)
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
            if (currentPhase >= greenPhase && currentPhase < redPhase)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    private Transform tachometr;
    private Transform lever;
    private Transform arrow;

    private Vector2 startSwipePos;
    private Vector2 endSwipePos;

    private Action update;

    private void Start()
    {
        AutoSetTachometr();
        update = EngineController;
    }



    void Update()
    {
        #region TEST OUTPUT
        // if (CurrentPhase < greenPhase)
        // {
        //     TestText.color = Color.black;
        // }
        // else if (CurrentPhase < redPhase)
        // {
        //     TestText.color = Color.green;
        // }
        // else
        // {
        //     TestText.color = Color.red;
        // }
        // TestText.text = CurrentPhase.ToString();
        #endregion
        update();
        MMVibrationManager.TransientHaptic(currentPhase/100,.2f);
    }
    private void FixedUpdate()
    {
    }

    public void StartScript(MainCamera audioSources)
    {
        this.enabled = true;

        NeutralPhaseEvent.AddListener(delegate {
            if (!audioSources.neutralPhaseEngine.isPlaying)
            {
                audioSources.neutralPhaseEngine.Play();
                audioSources.redPhaseEngine.Stop();
                audioSources.greenPhaseEngine.Stop();
            }
        });
        GreenPhaseEvent.AddListener(delegate {
            if (!audioSources.greenPhaseEngine.isPlaying)
            {
                audioSources.neutralPhaseEngine.Stop();
                audioSources.redPhaseEngine.Stop();
                audioSources.greenPhaseEngine.Play();
            }
        });
        RedPhaseEvent.AddListener(delegate {
            if (!audioSources.redPhaseEngine.isPlaying)
            {
                audioSources.neutralPhaseEngine.Stop();
                audioSources.redPhaseEngine.Play();
                audioSources.greenPhaseEngine.Stop();
            }
        });
    }
    public void EndScript()
    {
        this.enabled = false;
        RedPhaseEvent.RemoveAllListeners();
        GreenPhaseEvent.RemoveAllListeners();
        NeutralPhaseEvent.RemoveAllListeners();
    }

    /// <summary>
    /// Отслеживает полный запуск от начала и до конца
    /// </summary>
    private void EngineController()
    {

        if (Input.GetMouseButtonDown(0))
        {
            startSwipePos = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0) && IsGreenPhase)
        {
            tachometr.GetComponent<Animation>().Play("Down");
            Destroy(tachometr.gameObject, 1f);
            update = delegate { };
            EndMechanicEvent?.Invoke();
        }
        else if (Input.GetMouseButton(0))
        {
            endSwipePos = Input.mousePosition;
            Vector2 distance = endSwipePos - startSwipePos;

            if ((distance.y * -1) > (Screen.height / 6)) 
            {
                CurrentPhase += 20 * Time.deltaTime;

                //Движение рычага
                if (lever.transform.localRotation.x > -0.5f && distance.y < 0)
                {
                    lever.transform.localRotation *= Quaternion.AngleAxis(distance.y * Time.deltaTime * (0.1f), Vector3.right);


                }
            }
        }
        else
        {
            CurrentPhase -= 30 * Time.deltaTime;

            //Возвращение рычага
            if (lever.transform.localRotation.x < 0.5f)
            {
                lever.transform.rotation *= Quaternion.AngleAxis(Time.deltaTime * -40f, Vector3.left);
            }
        }

        //Стрелка
        arrow.transform.localRotation = Quaternion.AngleAxis(360 / 100 * CurrentPhase, Vector3.forward);

        //Тряска Тахометра
        tachometr.localPosition = UnityEngine.Random.insideUnitSphere * (CurrentPhase * 0.000005f);
    }


    private void AutoSetTachometr()
    {
        tachometr = Camera.main.GetComponent<MainCamera>().tachometr.transform;
        tachometr.gameObject.SetActive(true);
        tachometr.GetComponent<Animation>().Play("Up");
        arrow = tachometr.GetChild(0);
        lever = tachometr.GetChild(1);

    }










}
