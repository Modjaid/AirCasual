using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.NiceVibrations;
using UnityEngine;

public class DesertMiniGun : Weapon
{
	[SerializeField] private Transform aimSprite;
	[SerializeField] private float aimDistance;
	[SerializeField] private float aimOffset;
	[Header("Дуло минигана для вращения")]
	[SerializeField] private Transform muzzle;

	private Quaternion newMuzzleRotate; // Новый угол вращения дула минигана

	private Shaker shakerGun;
	private Shaker shakerCamera;

	private Action lateUpdate;

	//private Action muzzleNewRotate;
	private float muzzleAxis;

    private void Start()
    {
		//targetRotation = aimSprite.eulerAngles;
		aimSprite.gameObject.SetActive(true);
		aimSprite.transform.position = transform.position + (-transform.up * aimDistance) + new Vector3(0, aimOffset, 0);

		newMuzzleRotate = muzzle.transform.localRotation;

		shakerGun = new Shaker();
		lateUpdate = delegate { };
	}
    private void LateUpdate()
	{
		//Вращение дула минигана
		muzzle.transform.localRotation = Quaternion.Lerp(muzzle.transform.localRotation, newMuzzleRotate, Time.deltaTime * 10f);
		//muzzle.transform.localRotation = Quaternion.Lerp(muzzle.transform.localRotation, newMuzzleRotate, Time.deltaTime * 10f);
		lateUpdate();
	}


	public override bool Shot()
	{

		if (IsReadyShot)
		{

			Rocket rocket = Instantiate(callibrPrefab, calibrPlace.transform).GetComponent<Rocket>();
			rocket.transform.parent = null;
			shotSound.pitch = UnityEngine.Random.Range(1f, 1.2f);

			shotSound.PlayOneShot(shotSound.clip);
			//Rocket rocket = Instantiate(callibrPrefab).GetComponent<Rocket>();
			Vector2 random = UnityEngine.Random.insideUnitCircle * hitRadius * 2;
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
		shotSound.pitch = UnityEngine.Random.Range(1f, 1.2f);
		shotSound.PlayOneShot(shotSound.clip);
		shootEffect.SetActive(true);
		newMuzzleRotate *= Quaternion.Euler(0, muzzleAxis, 0);
		muzzleAxis += 5f;
		shakerGun.Push(100,3);
		lateUpdate = delegate
		{
				shakerGun.Shake(Camera.main.transform);

		};
		MMVibrationManager.Haptic(HapticTypes.MediumImpact, false, true, this);
	}


	private void UpdateAimPosition()
    {
		aimSprite.transform.position = transform.position + (transform.forward * 15f) + new Vector3(0, 0.5f, 0);
	}

    public override void OffShot()
    {
		shootEffect.SetActive(false);
	}
}


