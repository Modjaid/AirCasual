using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.PostProcessing;

public class MainCamera : MonoBehaviour
{
    public UnityEvent OnFinishParking;
    public GameObject tachometr;
    [Header("AIRFIELD")]
    public AudioSource takeUpSound;
    public AudioSource neutralPhaseEngine;
    public AudioSource greenPhaseEngine;
    public AudioSource redPhaseEngine;

    private Transform targetTransform; //Позциция которую занимает камера
    private Transform lookTarget; //Позциция на которую смотрит камера
    private Action transformPosUpd;
    private Action lookRotUpd;
    private float currentSpeed;
    private float defaultSpeed;

    private float distanceForNext;

    void Awake()
    {
        transformPosUpd = delegate { };
        lookRotUpd = delegate { };
        defaultSpeed = 2f;
        currentSpeed = defaultSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        transformPosUpd();
    }
    private void LateUpdate()
    {
        lookRotUpd();
    }

    public void SetCameraParking(Transform newTarget,float distanceForNext)
    {
        this.distanceForNext = distanceForNext;
        targetTransform = newTarget;
        transformPosUpd = CameraParking;
    }

    public void SetCameraParking(Transform newTarget, float distanceForNext,float speed)
    {
        this.currentSpeed = speed;
        this.distanceForNext = distanceForNext;
        targetTransform = newTarget;
        transformPosUpd = CameraParking;
    }

    private void CameraParking()
    {
        transform.position = Vector3.Lerp(transform.position, targetTransform.position, Time.deltaTime * currentSpeed);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetTransform.rotation, Time.deltaTime * currentSpeed);

        var distance = Vector3.Distance(transform.position, targetTransform.position);

        //TODO: РЕАЛИЗОВАТЬ ТАКЖЕ И ПОВОРОТ САМОЙ ТЕХНИКИ ПОМИМО КАМЕРЫ ДЛЯ КРУТОСТИ
        // this.transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(-90, AirAngleZ, 0), Time.deltaTime * 4f);
        if (distance < distanceForNext)
        {
            transformPosUpd = delegate { };
            currentSpeed = defaultSpeed;
            OnFinishParking?.Invoke();
        }
    }
    /// <summary>
    /// Смотрит на объект обязательно выключить через функцию LookAtOff()
    /// </summary>
    /// <param name="target"></param>
    public void LookAtOn(Transform target)
    {
        lookTarget = target;

        lookRotUpd = delegate {
            transform.LookAt(target);
        };
    }
    public void LookAtOff()
    {
        lookRotUpd = delegate {};
    }

    public void SetPos(Transform pos)
    {
        this.transform.position = pos.position;
        this.transform.rotation = pos.rotation;
    }

    public void PostEffectOn()
    {
        //GetComponent<PostProcessVolume>().
    }
}
