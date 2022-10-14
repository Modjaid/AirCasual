using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class AirPlatform : MonoBehaviour
{
	[Header("В дальнейшем дописать, пока не обязательно")]
	[SerializeField] private List<GameObject> GunPrefabs;
	[Header("место для Орудия")]
	[SerializeField] public Transform GunPlace;
	[Header("место для Камеры")]
	[SerializeField] public Transform CameraStartPlace;
	[SerializeField] public Transform CameraPlace;
	[Header("место для Аира")]
	[SerializeField] public Transform airPlace;

	[Header("Вентиль (Если это вертолет)")]
	[SerializeField] public GameObject screw;


	[Header("Объект на канвасе (рейкастный)")]
	[SerializeField] private DragHandler DragHandler;

	[Header("(Helicopter) y: 126, 240; x: -50, 50")]
	[SerializeField] private Diapason diapasonX;
	[SerializeField] private Diapason diapasonY;

	[SerializeField] private LayerMask layerMask;

	#region Joystick
	[SerializeField] private float GunRotationSpeed;
	private Quaternion derivative; // Analog of currentVelocity in Mathf.SmoothDamp, but for Quaternion. Used for SmoothDamping.
	private Vector3 targetRotation; // For AIM
	#endregion

	public static List<GameObject> onScreenParticles = new List<GameObject>();

	private Transform desertGunTransform;
	private Weapon desertGunScript;

	public HealthData healthData;
	[Header("Здоровье Платформы")]
	public float healthPlatform;
	public Image healthSlideBar;
	
	public float cameraShake;
	public Shaker cameraShaker;
	public Animation animDamage;

	void Awake()
	{
		StartCoroutine("CheckForDeletedParticles");
	}
	private void Start()
	{
	//	cameraShaker = new Shaker();
	//	healthData = new HealthData(healthPlatform, 0.5f);

		desertGunTransform = GunPlace.GetChild(0);
		desertGunScript = desertGunTransform.GetComponent<Weapon>();
		targetRotation = desertGunTransform.localEulerAngles;

		DragHandler.gameObject.SetActive(true);
		DragHandler.onDrag.AddListener(RotateAim);
	}

	void Update()
	{
	//	Debug.DrawRay(desertGunTransform.position, -desertGunTransform.up, Color.red);
	//	cameraShaker.Shake(Camera.main.transform);
	//	healthData.Update();
	//	healthSlideBar.fillAmount = healthData.GetBarNormalized();

		if (Input.GetMouseButton(0))
		{
			Ray ray = new Ray(desertGunTransform.position, -desertGunTransform.up);
			RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(ray,out hit, 9999f,layerMask))
            {
				desertGunScript.SetTargetPos(hit.point);
				desertGunScript.Shot();

			}
            else
            {
				desertGunScript.OffShot();

			}

        }
        else
        {
			desertGunScript.OffShot();
		}

	}


	/// <summary>
	/// EVENT BY DRAGHANDLER
	/// </summary>
	/// <param name="input"></param>
	public void RotateAim(Vector2 input)
	{
	//	Debug.Log($"INPUT X:{input.x}; INPUT Y:{input.y}\n targetRot X:{targetRotation.x}; targetRot:{targetRotation.y}\n" +
	//		$"GunRotation: {desertGunTransform.transform.localEulerAngles}");

		targetRotation.x -= input.y * GunRotationSpeed;
		targetRotation.z += input.x * GunRotationSpeed;
		targetRotation.x = Mathf.Clamp(targetRotation.x, diapasonY.start, diapasonY.end);
		targetRotation.z = Mathf.Clamp(targetRotation.z, diapasonX.start, diapasonX.end);

		desertGunTransform.transform.localEulerAngles = targetRotation;
	}

	public void OffWeaponShot()
    {
		desertGunScript.OffShot();
	}

	/*private void RotateGunByAim()
	{
		airGun.transform.localRotation = SmoothDamp(airGun.transform.localRotation, Quaternion.Euler(targetRotation),
		 ref derivative, miniGunRotationSpeed);
	}
	*/

	public static Quaternion SmoothDamp(Quaternion rot, Quaternion target, ref Quaternion deriv, float time)
	{
		if (Time.deltaTime < Mathf.Epsilon) return rot;
		// account for double-cover
		var Dot = Quaternion.Dot(rot, target);
		var Multi = Dot > 0f ? 1f : -1f;
		target.x *= Multi;
		target.y *= Multi;
		target.z *= Multi;
		target.w *= Multi;
		// smooth damp (nlerp approx)
		var Result = new Vector4(
			Mathf.SmoothDamp(rot.x, target.x, ref deriv.x, time),
			Mathf.SmoothDamp(rot.y, target.y, ref deriv.y, time),
			Mathf.SmoothDamp(rot.z, target.z, ref deriv.z, time),
			Mathf.SmoothDamp(rot.w, target.w, ref deriv.w, time)
		).normalized;

		// ensure deriv is tangent
		var derivError = Vector4.Project(new Vector4(deriv.x, deriv.y, deriv.z, deriv.w), Result);
		deriv.x -= derivError.x;
		deriv.y -= derivError.y;
		deriv.z -= derivError.z;
		deriv.w -= derivError.w;

		return new Quaternion(Result.x, Result.y, Result.z, Result.w);
	}


    IEnumerator CheckForDeletedParticles()
	{
		while (true)
		{
			yield return new WaitForSeconds(5.0f);
			for (int i = onScreenParticles.Count - 1; i >= 0; i--)
			{
				if (onScreenParticles[i] == null)
				{
					onScreenParticles.RemoveAt(i);
				}
			}
		}
	}

	#region TEST UI
	public void RESTARTUI()
	{
		SceneManager.LoadScene("Desert");
	}
	#endregion


	private void DestroyParticles()
	{
		for (int i = onScreenParticles.Count - 1; i >= 0; i--)
		{
			if (onScreenParticles[i] != null)
			{
				GameObject.Destroy(onScreenParticles[i]);
			}

			onScreenParticles.RemoveAt(i);
		}
	}


    [Serializable]
	internal struct Diapason
    {
		public float start;
		public float end;
    }

	
}
