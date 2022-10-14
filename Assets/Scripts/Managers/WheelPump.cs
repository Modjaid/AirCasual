using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

/// <summary>
/// - При активации скрипта (OnEnable) скрипт отключает все nativeWheel и вкл все pseudoWheel, камера принимает позицию у первого колеса
/// - После парковки камеры к нужному колесу, активировать механику накачки
/// - Зажатие активирует анимацию, если палец сбрасывается то анимация идет на исходную
/// - Зажатие до 100% переходит на следующее колесо, можно сделать евент мне для дальнейшего UI
/// - эффект подъема техники при накачке
/// - Все колеса готовы: вернуть камеру на исходное состояние и дестроить все pseudoWheel
/// - уместить все в этот скрипт
/// </summary>
public class WheelPump : MonoBehaviour
{
    [Header("Все колеса доступные для накачки")]
    [SerializeField] private List<Wheel> wheels;

    public float pumpingSpeed = 0.4f;
    public float pumpingReverseSpeed = 0.4f;

    public UnityEvent onWheelPumpingComplete;
    public UnityEvent onAllWheelsPumpingComplete;
    public float pumpPushUpSensitivity;

    private Transform currentCameraPlace;
    private bool pointerDown;
    private bool cameraArrivedInDestination;
    private float pumpingProgress;
    private IEnumerator pumpRoutine;
    private IEnumerator pumpReversalRoutine;
    private int currentWheelIndex = 0;
    private List<Animator> wheelAnimators;

    private int bonus = 0;

    /// /////////////////////////////////////////////////////////////////////
    #region BAROMETR
    [Header("C какого числа начинается текущая фаза (Всего фазовых оборотов:100)")]
    [Range(0, 1f)]
    [SerializeField] private float greenPhase;
    [Range(0, 1f)]
    [SerializeField] private float redPhase;

    public UnityEvent NeutralPhaseEvent;
    public UnityEvent GreenPhaseEvent;
    public UnityEvent RedPhaseEvent;

    private float currentPhase = 0;

    private bool IsGreenPhase
    {
        get
        {
            if (pumpingProgress >= greenPhase && pumpingProgress < redPhase)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    private Transform barometr;
    private Transform arrow;
    private Quaternion startArrowRotation;
    private float shakingPoints;
    private float ShakingPoints
    {
        get { return shakingPoints; }
        set
        {
            shakingPoints = value;
            if (shakingPoints < 0)
            {
                shakingPoints = 0;
            }
        }
    }
    #endregion

    void Start()
    {

    }


    private void OnEnable()
    {
        wheelAnimators = new List<Animator>();
        InitBarometr();
        ActivePseudoWheels(true);
        NextWheel(wheels[currentWheelIndex]);
        //  ActivePseudoWheels(true);
        //  NextWheel(wheels[currentWheelIndex]);
    }
    private void OnDisable()
    {
      //  ActivePseudoWheels(false);
    }

    private void Update()
    {
        ProcessInput();

        //Стрелка
        arrow.transform.localRotation = Quaternion.AngleAxis((pumpingProgress * 280) - 135, Vector3.left);

       //Тряска Тахометра
       barometr.localPosition = UnityEngine.Random.insideUnitSphere * (ShakingPoints * 0.0000005f);
        ShakingPoints -= 300 * Time.deltaTime;

        bool IsGreen = false;
        
        if (pumpingProgress >= redPhase)
        {
            ShakingPoints = 100;
            IsGreen = false;
        }
        else if (pumpingProgress >= greenPhase)
        {
            IsGreen = true;
        }
    }

    private void InitBarometr()
    {
       barometr = Camera.main.transform.Find("Barometr").GetChild(0);
       arrow = barometr.GetChild(1);
       LevelManager.instance.MainCamera.OnFinishParking.AddListener(() => barometr.gameObject.SetActive(true));
    }

    private void OnPointerDown()
    {
        // If camera arrived in destination, start Pumping routine.

        if(cameraArrivedInDestination)
        {
            StartPumping();

        }
    }

    private void OnPointerUp()
    {

    }   
    
    // Input stuff. Works on mobile, too.
    private void ProcessInput()
    {
        //  bool newValue = Input.GetMouseButton(0);
        //
        //  if(newValue == true && pointerDown == false)
        //  {
        //      pointerDown = true;
        //
        //      OnPointerDown();
        //  }
        //
        //  if(newValue == false && pointerDown == true)
        //  {
        //      pointerDown = false;
        //
        //      OnPointerUp();
        //  }
        if (Input.GetMouseButton(0))
        {
            pointerDown = true;
            OnPointerDown();
        }
        else
        {
            pointerDown = false;
            OnPointerUp();
        }
    }

    // Start Pumping routine.
    private void StartPumping()
    {
        if(pumpRoutine != null)
        StopCoroutine(pumpRoutine);
        pumpRoutine = Pumping();
        StartCoroutine(pumpRoutine);
    }

    // Stop pump routine.
    private void StopPumping()
    {
        if(pumpRoutine != null)
        StopCoroutine(pumpRoutine);
    }

    private IEnumerator Pumping()
    {
        while(pointerDown && pumpingProgress < 1f)
        {
            float progressAmount = pumpingSpeed * Time.deltaTime; 
            pumpingProgress += progressAmount;

            UpdatePushUpCollider(currentWheelIndex);

            UpdateWheelAnimationProgress(currentWheelIndex, Mathf.Clamp01(pumpingProgress));

            yield return null;
        }

        if (pumpingProgress >= 1f)
        {
            onWheelPumpingComplete?.Invoke();

            if (currentWheelIndex < wheels.Count - 1)
            {
                currentWheelIndex++;

                NextWheel(wheels[currentWheelIndex]);

                pumpingProgress = 0f;
            }
            else
            {
                OnAllWheelPumpingComplete();
            }
        }
        else if (pumpingProgress >= greenPhase && Input.GetMouseButtonUp(0))
        {
            onWheelPumpingComplete?.Invoke();

            if (currentWheelIndex < wheels.Count - 1)
            {
                currentWheelIndex++;

                NextWheel(wheels[currentWheelIndex]);

                pumpingProgress = 0f;
            }
            else
            {
                OnAllWheelPumpingComplete();
            }

        }
        else
        {
            StartSmoothPumpingReversal();
        }
    }   

    private void StartSmoothPumpingReversal()
    {
        if(pumpReversalRoutine != null)
        StopCoroutine(pumpReversalRoutine);
        pumpReversalRoutine = SmoothlyReversePumpingProgress();
        StartCoroutine(pumpReversalRoutine);
    }

    private IEnumerator SmoothlyReversePumpingProgress()
    {
        while(!pointerDown)
        {
            float progressAmount = pumpingReverseSpeed * Time.deltaTime;

            pumpingProgress -= progressAmount;
            
            pumpingProgress = Mathf.Clamp01(pumpingProgress);

            UpdatePushUpCollider(currentWheelIndex);

            UpdateWheelAnimationProgress(currentWheelIndex, pumpingProgress);

            yield return null;
        }
    }

    private void UpdatePushUpCollider(int wheelIndex)
    {
        Wheel wheel = wheels[wheelIndex];
        Vector3 newPosition = wheel.pushUpColliderStartingPosition;
        newPosition.y += pumpingProgress * pumpPushUpSensitivity;
    
        wheel.pushUpCollider.position = newPosition;
    }

    // Triggered when all wheels are pumped
    private void OnAllWheelPumpingComplete()
    {
        ActivePseudoWheels(false);

         onAllWheelsPumpingComplete?.Invoke();

        //TODO: Создать в ангаре скрипт AirPark где будут разные позиции камер
       // LevelManager.instance.mainCamera.CameraStartPos();
    }

    private void ActivePseudoWheels(bool isActive)
    {
        foreach (Wheel wheel in wheels)
        {
            wheelAnimators.Add(wheel.pseudoWheel.gameObject.GetComponent<Animator>());
            wheel.pushUpColliderStartingPosition = wheel.pushUpCollider.transform.position;
            wheel.pushUpCollider.transform.parent = null;
            wheel.pseudoWheel.transform.parent = null;
            wheel.cameraPlace.transform.parent = null;
        }

        // Make sure that all wheel animators are stopped at Start.
        foreach (Animator wheelAnimator in wheelAnimators)
        {
            wheelAnimator.speed = 0f;
        }


        foreach (Wheel wheel in wheels)
        {
            wheel.pseudoWheel.gameObject.SetActive(isActive);
            wheel.cameraPlace.gameObject.SetActive(isActive);
            wheel.pushUpCollider.gameObject.SetActive(isActive);
        }

        LevelManager.instance.MainCamera.OnFinishParking.AddListener(() => barometr.gameObject.SetActive(true));
        LevelManager.instance.MainCamera.OnFinishParking.AddListener(() => cameraArrivedInDestination = true);
        LevelManager.instance.MainCamera.OnFinishParking.AddListener(() => ShakingPoints = 100);
    }

    public void DestroyAllPseudoWheels()
    {
        foreach(Wheel wheel in wheels)
        {
            Destroy(wheel.pseudoWheel.gameObject);
            Destroy(wheel.cameraPlace.gameObject);
            Destroy(wheel.pushUpCollider.gameObject);
        }
    }


    private void UpdateWheelAnimationProgress(int wheelIndex, float progress)
    {
        wheelAnimators[wheelIndex].Play("Pump", 0, progress);
    }

    private void NextWheel(Wheel wheel)
    {
        barometr.gameObject.SetActive(false);
        currentCameraPlace = wheel.cameraPlace;
        LevelManager.instance.MainCamera.SetCameraParking(currentCameraPlace,0.05f);
        cameraArrivedInDestination = false;
    }


    [Serializable]
    internal class Wheel
    {
        [Header("Позиция для паркинка камеры")]
        public Transform cameraPlace;
        [Header("Позиция для анимационных колес для анимации")]
        public Transform pseudoWheel;
        [Header("Позиция для настоящих колес")]
        public Transform nativeWheel;
        [Header("Пуш ап коллайдер")]
        public Rigidbody pushUpCollider;

        [HideInInspector] public Vector3 pushUpColliderStartingPosition;
    }
}
