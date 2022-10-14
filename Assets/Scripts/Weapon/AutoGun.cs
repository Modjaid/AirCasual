using MoreMountains.NiceVibrations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoGun : Weapon
{
	[SerializeField] private Transform aimSprite;
	[SerializeField] private float aimDistance;
	[SerializeField] private float aimOffset;

	private Shaker shakerGun;



	[SerializeField] private float shakeStrength;
	[SerializeField] private float shakeDicrement;

	private Action lateUpdate;

	private void Start()
	{
		//targetRotation = aimSprite.eulerAngles;
		if(aimSprite != null)
        {
			aimSprite.gameObject.SetActive(true);
			aimSprite.transform.position = transform.position + (-transform.up * aimDistance) + new Vector3(0, aimOffset, 0);
		}

		shakerGun = new Shaker();
		lateUpdate = delegate { };
	}
	private void LateUpdate()
	{
		lateUpdate();
	}


	public override bool Shot()
	{

		if (IsReadyShot)
		{

			Rocket rocket = Instantiate(callibrPrefab, calibrPlace.transform.position,
			Quaternion.LookRotation(transform.right)).GetComponent<Rocket>();

			//Rocket rocket = Instantiate(callibrPrefab).GetComponent<Rocket>();
			Vector2 random = UnityEngine.Random.insideUnitCircle * hitRadius * 2;
			Vector3 hitPosition = new Vector3(target.x + random.x, target.y, target.z + random.y);

			rocket.Launch(callibrSpeed, callibrDamage, callibrRadiusDamage, hitPosition);
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
		shotSound.pitch = UnityEngine.Random.Range(1f, 1.2f);
		shotSound.PlayOneShot(shotSound.clip);
		GameObject newEffect = Instantiate(shootEffect,calibrPlace);
		newEffect.transform.parent = null;
		newEffect.transform.position = calibrPlace.transform.position;

		shakerGun.Push(shakeStrength, shakeDicrement);

		lateUpdate = delegate
		{
			if (shakeStrength > 0)
			{
				shakerGun.Shake(Camera.main.transform);
			}
		};
		//MMVibrationManager.Haptic(HapticTypes.Failure, false, true, this);
		MMVibrationManager.ContinuousHaptic(.9f, .7f, .4f);
	}


	private void UpdateAimPosition()
	{
		aimSprite.transform.position = transform.position + (transform.forward * 15f) + new Vector3(0, 0.5f, 0);
	}

	public override void OffShot()
	{

	}
}
