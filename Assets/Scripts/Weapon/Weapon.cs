using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [Header("радиус погрешности попадания по цели")]
    [SerializeField] public float hitRadius; // Вот здесь нужно задать радиус погрешности попадания по цели

    [Header("Эффект выстрела")]
    [SerializeField] public GameObject shootEffect;
    [SerializeField] protected AudioSource shotSound;
    [Header("Начало выстрела")]
    [SerializeField] public Transform calibrPlace;

    [Header("Параметры патрона")]
    [SerializeField] public GameObject callibrPrefab; // Префаб патрона
    [SerializeField] public int callibrSpeed; // Передавать параметр патрону при его Instance
    [SerializeField] public float callibrDamage; // Передавать параметр патрону при его Instance
    [SerializeField] public float callibrRadiusDamage; // Передавать параметр патрону при его Instance
    [SerializeField] private float destroyDistance;
    [SerializeField] private float cooldown; // Время до выстрела !Обращаться только через свойства!

    protected Vector3 target;

    private float currentCooldown; 

    protected float CurrentCooldown
    {
        set
        {
            currentCooldown = value;
            if(currentCooldown < 0)
            {
                currentCooldown = 0;
            }
        }
        get
        {
            return currentCooldown;
        }
    }

    public void Update()
    {
        CurrentCooldown -= Time.deltaTime;
    }

    /// <summary>
    /// После true- будет авторестарт кулдауна
    /// </summary>
    protected bool IsReadyShot
    {
        get
        {
            if (currentCooldown > 0)
            {
                return false;
            }
            else
            {
                CurrentCooldown = cooldown;
                return true;
            }
        }
    }

    public abstract bool Shot();
    public abstract void OffShot();

    public void SetTargetPos(Transform enemy)
    {
        Vector3 AB = enemy.transform.position - calibrPlace.position;
        Vector3 BC = destroyDistance * AB;
        target = AB + BC;
    }
    public void SetTargetPos(Vector3 B)
    {
        // var direction = (target - transform.position).normalized;
        Vector3 AB = B - calibrPlace.position;
        Vector3 BC = destroyDistance * AB;
        target = AB + BC;
    }
}
