using MoreMountains.NiceVibrations;
using PathCreation.Examples;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class EarthBattle : MonoBehaviour
{
    public UnityEvent Victory;
    public static EarthBattle instance;


    [Header("JOYSTICK")]
    [SerializeField] private DragHandler DragHandler;
    [SerializeField] private float aimMoveSpeed = 2;
    [SerializeField] private Diapason diapasonX;
    [SerializeField] private Diapason diapasonY;
    [SerializeField] private Transform aim;
    [SerializeField] private LayerMask enemyLayer;

    private Vector3 aimNewPos;

    [SerializeField] private MissionGun rocket;
    [SerializeField] private AudioSource rocketSound;
    [SerializeField] private AudioSource aimFailSound;
    [SerializeField] private AudioSource aimStartSound;
    [SerializeField] private GameObject aimCamera;
    [SerializeField] private int countEnemy;
    private int CountEnemy;
    public Transform cameraPlace;
    public Transform helicopterPlace;
    public GameObject boomEffect;

    public GameObject[] enemyObjs;
    public Tutorial tutorial;



    #region InitializationAim
    [SerializeField] float shotTimer;
    private float currentShotTimer;
    private GameObject currentEnemyAim;
    #endregion

    private Shaker shakerCamera;

    private void Start()
    {
        instance = this;
    }
    private void Update()
    {
        InputClick();
        shakerCamera.Shake(Camera.main.transform);
    }

    public void EnableScript(Transform air,int currentLvl)
    {
        this.enabled = true;
        aimCamera.SetActive(true);
        CountEnemy = countEnemy;
        shakerCamera = new Shaker();
        Camera.main.transform.parent = cameraPlace;
        Camera.main.transform.position = cameraPlace.position;
        Camera.main.transform.rotation = cameraPlace.rotation;
        initJoystick();

        WallAirPark.ActiveMap(currentLvl);
        WallAirPark.HideObjs(currentLvl);

        foreach (GameObject enemy in enemyObjs)
        {
            enemy.transform.parent.gameObject.SetActive(true);
            enemy.GetComponent<FollowBot>().RestartPath();
        }
        air.parent = helicopterPlace;
        air.position = helicopterPlace.position;
        air.rotation = helicopterPlace.rotation;
        air.localScale = helicopterPlace.localScale;

    }

    public void DecreaseEnemy()
    {
        CountEnemy--;
        if(CountEnemy < 1)
        {
            Victory?.Invoke();
            DisableBots();
        }
    }

    public void RestartScript()
    {
        CountEnemy = countEnemy;
        helicopterPlace.GetChild(0).gameObject.SetActive(true);

        foreach (GameObject enemy in enemyObjs)
        {
            enemy.transform.parent.gameObject.SetActive(true);
            enemy.GetComponent<FollowBot>().RestartPath();
        }
    }

    public void DisableBots()
    {
        foreach (GameObject enemy in enemyObjs)
        {
            enemy.transform.parent.gameObject.SetActive(false);
        }
    }

   // public void DestroyHelicopter()
   // {
   //     helicopterPlace.GetChild(0).gameObject.SetActive(false) ;
   //     Instantiate(boomEffect,this.transform).transform.parent = null;
   // }

  //  public void RestartScript()
  //  {
  //
  //  }



    #region Joystick
    /// <summary>
    /// EVENT BY DRAGHANDLER
    /// </summary>
    /// <param name="input"></param>
    public void SetPosAimByJoystick(Vector2 input)
    {
        //	Debug.Log($"INPUT X:{input.x}; INPUT Y:{input.y}\n targetRot X:{targetRotation.x}; targetRot:{targetRotation.y}\n" +
        //		$"GunRotation: {desertGunTransform.transform.localEulerAngles}");

        aimNewPos.y += input.y * aimMoveSpeed;
        aimNewPos.z -= input.x * aimMoveSpeed;
        aimNewPos.y = Mathf.Clamp(aimNewPos.y, diapasonY.start, diapasonY.end);
        aimNewPos.z = Mathf.Clamp(aimNewPos.z, diapasonX.start, diapasonX.end);

        aim.transform.position = aimNewPos;
    }

    private void initJoystick()
    {
        aimNewPos = aim.transform.position;
        DragHandler.gameObject.SetActive(true);
        DragHandler.onDrag.AddListener(SetPosAimByJoystick);
    }

    private void InputClick()
    {
        RaycastHit hit = new RaycastHit();


        if (Input.GetMouseButtonUp(0))
        {
            aim.gameObject.SetActive(false);
            TargetInitShot(null);

        }
        else if (Input.GetMouseButton(0))
        {
            aim.gameObject.SetActive(true);
            if (IsDirectHit(out hit))
            {
                TargetInitShot(hit.collider.gameObject);
            }
            else
            {
                TargetInitShot(null);
            }
        }
    }

    /// <summary>
    /// Если рейкаст цели указывает ровно на врага возвращает true
    /// </summary>
    /// <param name="hit"></param>
    /// <returns></returns>
    public bool IsDirectHit(out RaycastHit hit)
    {
        var dir = (Camera.main.transform.position - aim.transform.position).normalized;
        Ray ray = new Ray(Camera.main.transform.position, -dir);

        Debug.DrawRay(ray.origin, ray.direction * 100, Color.red);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, enemyLayer))
        {
            if (hit.collider.tag == "Enemy")
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Есть пять статусов инциализации цели:
    /// 1) Цель была на прицеле но сбросилась -FAILED
    /// 2) Прицел на пустоте - No TARGET
    /// 3) Прицел попал на новую цель -NEW TARGET
    /// 4) Прицел выстреливает -TARGET ATTACK
    /// </summary>
    /// <param name="targetObject">Объект попавший под рэйкаст луч</param>
    /// <returns>True Если можно стрелять</returns>
    public bool TargetInitShot(GameObject updEnemyAim)
    {

        ///////////////////////////////////////////////////
        //FAILED
        if (updEnemyAim == null && currentEnemyAim != null)
        {
            ActivationAllColliders(false);
            StartCoroutine(DelayActivation(1f, true));
            currentEnemyAim.GetComponent<FollowBot>().AimFailed();
            aimStartSound.Stop();
            aimFailSound.Play();
            currentEnemyAim = null;
            currentShotTimer = shotTimer;
            return false;
        }
        //NO TARGET
        if (updEnemyAim == null)
        {
            aimStartSound.Stop();
            currentEnemyAim = null;
            return false;
        }
        //NEW TARGET
        if (updEnemyAim != null && updEnemyAim != currentEnemyAim)
        {
            OffAnimationAllEnemies();
            ActivationAllColliders(false);
            currentShotTimer = shotTimer;
            currentEnemyAim = updEnemyAim;
            currentEnemyAim.tag = "Enemy";
            currentEnemyAim.layer = 9;
            currentEnemyAim.GetComponent<FollowBot>().EnableAim();
            aimStartSound.Play();
            return true;

        }
        currentShotTimer -= Time.deltaTime;
        //TARGET ATTACK
        if (currentShotTimer <= 0)
        {
            BoxCollider[] colliders = currentEnemyAim.GetComponents<BoxCollider>();
            colliders[0].enabled = true;
            colliders[1].enabled = false;

            rocket.Shot(currentEnemyAim.transform);
            rocketSound.Play();
            shakerCamera.Push(300,2);
            MMVibrationManager.ContinuousHaptic(.9f, .07f, .4f);
            currentShotTimer = shotTimer;
            StartCoroutine(DelayActivation(2.4f, true));
            currentEnemyAim.tag = "Untagged";
            currentEnemyAim.layer = 0;
            currentEnemyAim = null;
        }
        return true;

    }

    public void ActivationAllColliders(bool activation)
    {
        string newTag;
        int newLayer;
        if (activation)
        {
            newTag = "Enemy";
            newLayer = 9;
        }
        else
        {
            newTag = "Untagged";
            newLayer = 0;
        }

        foreach (GameObject enemy in enemyObjs)
        {
            enemy.GetComponents<BoxCollider>()[1].enabled = true;
            enemy.tag = newTag;
            enemy.layer = newLayer;
            enemy.name = newTag.ToString();
        }
    }

    public void OffAnimationAllEnemies()
    {
        foreach (GameObject enemy in enemyObjs)
        {
            enemy.transform.GetChild(0).GetComponent<Animation>().Play("AllOff");
        }
    }

    public IEnumerator DelayActivation(float delay, bool activation)
    {
        yield return new WaitForSeconds(delay);
        Debug.Log("DELAY DONE");
        ActivationAllColliders(activation);
    }

    [Serializable]
    internal struct Diapason
    {
        public float start;
        public float end;
    }
    #endregion

}
