using MoreMountains.NiceVibrations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesertDualGun : Weapon
{
    [Header("Эффект выстрела")]
    [SerializeField] public GameObject shootEffect2;
    [Header("Начало выстрела")]
    [SerializeField] public Transform calibrPlace2;

    [SerializeField] private Transform aimSprite;
    [SerializeField] private float aimDistance;
    [SerializeField] private float aimOffset;


    private Shaker shakerGun;

    private bool isLeft;

    private Action lateUpdate;

    private void Start()
    {
        //targetRotation = aimSprite.eulerAngles;
        aimSprite.gameObject.SetActive(true);
        aimSprite.transform.position = transform.position + (-transform.up * aimDistance) + new Vector3(0, aimOffset, 0);

        shakerGun = new Shaker();
        lateUpdate = delegate { };
    }
    private void LateUpdate()
    {
        lateUpdate();
    }

    public override void OffShot()
    {
        shootEffect2.SetActive(false);
        shootEffect.SetActive(false);
    }

    public override bool Shot()
    {
        if (IsReadyShot)
        {
            Vector3 currentMuzzle;
            GameObject currentEffect;
            if (isLeft)
            {
                currentMuzzle = calibrPlace.transform.position;
                currentEffect = shootEffect;
                isLeft = false;

            }
            else
            {
                currentMuzzle = calibrPlace2.transform.position;
                currentEffect = shootEffect2;
                isLeft = true;
            }

            Rocket rocket = Instantiate(callibrPrefab, currentMuzzle,
            Quaternion.LookRotation(transform.right)).GetComponent<Rocket>();

            //Rocket rocket = Instantiate(callibrPrefab).GetComponent<Rocket>();
            Vector2 random = UnityEngine.Random.insideUnitCircle * hitRadius * 2;
            Vector3 hitPosition = new Vector3(target.x + random.x, target.y, target.z + random.y);

            rocket.Launch(callibrSpeed, callibrDamage, callibrRadiusDamage, hitPosition);
            //Instance патрона по всем параметрам что взяты из инспектора данного оружия
            GunEffectOn(currentEffect);
            return true;
            // Патрон должен долететь до цели (target в род классе через метод setTargetPos(Transform enemy))
        }
        else
        {
            return false;
        }
    }

    private void GunEffectOn(GameObject currentEffect)
    {
        shotSound.pitch = UnityEngine.Random.Range(1f, 1.2f);
        shotSound.PlayOneShot(shotSound.clip);
        currentEffect.SetActive(true);
        shakerGun.Push(100, 10);
        lateUpdate = delegate
        {
           shakerGun.Shake(Camera.main.transform);
        };
        MMVibrationManager.Haptic(HapticTypes.HeavyImpact, false, true, this);
    }
}
