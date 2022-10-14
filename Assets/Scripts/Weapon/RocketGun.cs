using MoreMountains.NiceVibrations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketGun : Weapon
{
    public override void OffShot()
    {

    }

    public override bool Shot()
    {
        if (IsReadyShot)
        {

            Missile rocket = Instantiate(callibrPrefab, calibrPlace.transform.position,
            Quaternion.LookRotation(transform.forward)).GetComponent<Missile>();

            //Rocket rocket = Instantiate(callibrPrefab).GetComponent<Rocket>();
            Vector2 random = UnityEngine.Random.insideUnitCircle * hitRadius * 2;
            Vector3 hitPosition = new Vector3(target.x + random.x, target.y, target.z + random.y);

            rocket.Launch(callibrDamage, callibrRadiusDamage, callibrSpeed, hitPosition);
            //Instance патрона по всем параметрам что взяты из инспектора данного оружия
            GunEffectOn();
            return true;
            // Патрон должен долететь до цели (target в род классе через метод setTargetPos(Transform enemy))
        }
        else
        {
            return false;
        }
    }

    private void GunEffectOn()
    {
        this.gameObject.SetActive(false);
        float cooldown = CurrentCooldown;
        MMVibrationManager.ContinuousHaptic(.9f, .7f, .4f);
        Invoke("EnableRocket", cooldown);

    }
    private void EnableRocket()
    {
        this.gameObject.SetActive(true);
    }


}
