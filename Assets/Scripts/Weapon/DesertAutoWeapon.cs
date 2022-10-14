using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesertAutoWeapon : Weapon
{
	public float WaitStartShot;

    public void Start()
    {
		CurrentCooldown = WaitStartShot;

	}

    public void Update()
    {
		Shot();
		CurrentCooldown -= Time.deltaTime;
    }

    public override bool Shot()
	{

		if (IsReadyShot)
		{
			Debug.Log("GOOO");
			DesertEnemyCalibr rocket = Instantiate(callibrPrefab, calibrPlace.transform).GetComponent<DesertEnemyCalibr>();
			rocket.Launch(DesertAirPark.selectedPlatform, callibrSpeed);
			return true;
			// Патрон должен долететь до цели (target в род классе через метод setTargetPos(Transform enemy))
		}
		else
		{
			return false;
		}

	}
	public void Restart()
    {
		CurrentCooldown = WaitStartShot;
	}



	public override void OffShot()
	{

	}
}
