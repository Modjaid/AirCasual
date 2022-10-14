using MoreMountains.NiceVibrations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// class WEAPON - 
/// public GameObject callibrPrefab; // Префаб патрона
/// public float hitRadius; // Вот здесь нужно задать радиус погрешности попадания по цели
/// public int callibrSpeed; // Передавать параметр патрону при его Instance
/// public float callibrDamage; // Передавать параметр патрону при его Instance
/// 
/// Дым идет следом за патроном
/// </summary>
public class VolleyGun : Weapon
{
    public override void OffShot()
    {
        throw new System.NotImplementedException();
    }

    public override bool Shot()
    {
        if (IsReadyShot)
        {
            Rocket rocket = Instantiate(callibrPrefab, calibrPlace.position, 
            Quaternion.LookRotation(transform.forward)).GetComponent<Rocket>();

            Vector2 random = Random.insideUnitCircle * hitRadius * 2;
            Vector3 hitPosition = new Vector3(target.x + random.x, target.y, target.z + random.y);

            rocket.Launch(callibrSpeed, callibrDamage, callibrRadiusDamage, hitPosition);
            //Instance патрона по всем параметрам что взяты из инспектора данного оружия
            GunEffectOn();
            // Патрон должен долететь до цели (target в род классе через метод setTargetPos(Transform enemy))
            return true;
        }
        else
        {
            return false;
        }
    }

    private void GunEffectOn()
    {
        GameObject newEffect = Instantiate(shootEffect, calibrPlace);
        newEffect.transform.parent = null;
        newEffect.transform.position = calibrPlace.transform.position;
        MMVibrationManager.ContinuousHaptic(.9f,.7f,.4f);
    }

}
