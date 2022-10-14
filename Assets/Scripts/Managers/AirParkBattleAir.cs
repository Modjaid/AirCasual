using MoreMountains.NiceVibrations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AirParkBattleAir : AirPark
{
    public static UnityEvent Victory;

    public GameObject air;
    [Header("Ракета в иерархии Air")]
    [SerializeField] private MissionGun rocket;
    [SerializeField] private AudioSource rocketSound;
    [SerializeField] private AudioSource aimFailSound;
    [SerializeField] private AudioSource aimStartSound;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform skyBox;
    [SerializeField] private Collider moveBox;
    [Header("Сколько врагов надо будет победить в целом")]
  //  [SerializeField] private int enemyCount = 2;
    [Header("Точки появления врагов")]
    [SerializeField] private List<Transform> enemySpawns;
    [Header("Настройки прицела")]
    [SerializeField] private Transform aim;
    [SerializeField] private Color RedColor;
    [SerializeField] private Color GreenColor;
    [SerializeField] private LayerMask enemyLayer;
    public Tutorial tutorial;


    public static List<GameObject> enemies = new List<GameObject>();
    #region InitializationAim
    [SerializeField] float shotTimer;
    private float currentShotTimer;
    private GameObject currentEnemyAim;
    #endregion

    [Header("КАМЕРЫ")]
    [Space(2)]
    public Transform CameraPos1;
    public Transform CameraPos2;


    private static Bounds moveBoxBounds;
  //  private int enemyKills = 0;
  //  private bool IsVictory
  //  {
  //      get
  //      {
  //          if(enemyKills == enemyCount)
  //          {
  //              return true;
  //          }
  //          return false;
  //      }
  //  }
    private bool isLeftSpawn = true;
    private Shaker shakerCamera;

    #region Joystick
    [Header("JOYSTICK")]
    [SerializeField] private DragHandler DragHandler;
    [SerializeField] private float aimMoveSpeed = 2;
    [SerializeField] private Diapason diapasonX;
    [SerializeField] private Diapason diapasonY;
    private Vector3 aimNewPos;
    #endregion

    #region FINGER
    public Collider fingerPlane;
    #endregion

    #region SWITCH
    private Action update;
    #endregion


    void Update()
    {
        shakerCamera.Shake(Camera.main.transform);
        update();
    }
    private void Start()
    {
        shakerCamera = new Shaker();
    }

    public void EnableScript(GameObject air,int countEnemies)
    {
        this.enabled = true;

        air.transform.position = getAirPlace(AirType.F15).position;
        air.transform.rotation = getAirPlace(AirType.F15).rotation;
        air.transform.parent = getAirPlace(AirType.F15).transform;

        rocket.gameObject.SetActive(true);

        currentShotTimer = shotTimer;

        SetNewEnemies(countEnemies);

        moveBoxBounds = moveBox.bounds;

        SetJoystick();
        Victory = new UnityEvent();
        Victory.AddListener(() => Debug.Log("VICTORY"));
    }

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
        aimNewPos.x += input.x * aimMoveSpeed;
        aimNewPos.y = Mathf.Clamp(aimNewPos.y, diapasonY.start, diapasonY.end);
        aimNewPos.x = Mathf.Clamp(aimNewPos.x, diapasonX.start, diapasonX.end);

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
            if(IsDirectHit(out hit)){
                TargetInitShot(hit.collider.gameObject);
            }
            else
            {
                TargetInitShot(null);
            }
        }
        



        /*
         if (Input.GetMouseButtonUp(0))
         {
             aim.gameObject.SetActive(false);
             if (IsDirectHit(out hit))
             {
                 rocket.Shot(hit.collider.gameObject.transform);
                 shakerCamera.Push(100);
             }
         }
         else if (Input.GetMouseButton(0))
         {
             aim.gameObject.SetActive(true);
             if (IsDirectHit(out hit))
             {
                 aim.GetComponent<SpriteRenderer>().color = GreenColor;
             }
             else
             {
                 aim.GetComponent<SpriteRenderer>().color = RedColor;
             }
         }
        */
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

    #endregion

    #region Finger
    private void InitFinger()
    {
        aimNewPos = aim.transform.position;
    }

    private void InputFingerClick()
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


        //  if (Input.GetMouseButtonUp(0))
        //  {
        //      aim.gameObject.SetActive(false);
        //      if (IsFingerDirectHit(out hit))
        //      {
        //          rocket.Shot(hit.collider.gameObject.transform);
        //          shakerCamera.Push(100);
        //      }
        //  }
        //  else if (Input.GetMouseButton(0))
        //  {
        //      aim.gameObject.SetActive(true);
        //      if (IsFingerDirectHit(out hit))
        //      {
        //          aim.GetComponent<SpriteRenderer>().color = GreenColor;
        //      }
        //      else
        //      {
        //          aim.GetComponent<SpriteRenderer>().color = RedColor;
        //      }
        //  }
    }

    /// <summary>
    /// Если рейкаст цели указывает ровно на врага возвращает true
    /// </summary>
    /// <param name="hit"></param>
    /// <returns></returns>
    public bool IsFingerDirectHit(out RaycastHit hit)
    {

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

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

    public void SetNewPosAimByFinger()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if(Physics.Raycast(ray,out hit))
        {
            aim.transform.position = new Vector3(hit.point.x,hit.point.y,hit.point.z);
        }
    }

    #endregion

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
            currentEnemyAim.GetComponent<AirLocationEnemy>().AimFailed();
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
            currentEnemyAim.GetComponent<AirLocationEnemy>().EnableAim();
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
            shakerCamera.Push(100);

            currentShotTimer = shotTimer;
            StartCoroutine(DelayActivation(2.4f,true));
         //   Debug.Log("TARGET ATTACK");
            currentEnemyAim.tag = "Untagged";
            currentEnemyAim.layer = 0;
            currentEnemyAim = null;
        }
        return true;

    }
    public static IEnumerator DelayActivation(float delay, bool activation)
    {
        yield return new WaitForSeconds(delay);
        Debug.Log("DELAY DONE");
        ActivationAllColliders(activation);
    }
    public static void ActivationAllColliders(bool activation)
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

        foreach (GameObject enemy in enemies)
        {
            enemy.GetComponents<BoxCollider>()[1].enabled = true;
            enemy.tag = newTag;
            enemy.layer = newLayer;
            enemy.name = newTag.ToString();
        }
    }
    public static void OffAnimationAllEnemies()
    {
        foreach (GameObject enemy in AirParkBattleAir.enemies)
        {
            enemy.transform.GetChild(0).GetComponent<Animation>().Play("AllOff");
        }
    }

    public static Vector3 RandomPointInBounds()
    {
        return new Vector3(
            UnityEngine.Random.Range(moveBoxBounds.min.x, moveBoxBounds.max.x),
            UnityEngine.Random.Range(moveBoxBounds.min.y, moveBoxBounds.max.y),
            UnityEngine.Random.Range(moveBoxBounds.min.z, moveBoxBounds.max.z)
        );
    }

    private void InstanceEnemy()
    {
        GameObject newGO;
        Transform parent;
        if (isLeftSpawn)
        {
            parent = enemySpawns[0].transform;
        }
        else
        {
            parent = enemySpawns[1].transform;
        }

        newGO = Instantiate(enemyPrefab,parent);
        enemies.Add(newGO);
        isLeftSpawn = !isLeftSpawn;

    }


    private void InstantiateEffectPrefab(GameObject collisionEffect, Vector3 instancePosition)
    {
        GameObject effect = (GameObject)Instantiate(collisionEffect);
        effect.transform.position = instancePosition;
    }

    #region TEST UI
    public void SetJoystick()
    {
        DragHandler.onDrag.RemoveAllListeners();
        initJoystick();
        update = InputClick;

    }

    public void SetFinger()
    {
        DragHandler.onDrag.RemoveAllListeners();
        InitFinger();
        update = InputFingerClick;
        update += SetNewPosAimByFinger;
    }

    public void SetNewEnemies(int countEnemies)
    {
        int i = 0;
        float timer = .3f;
        while(i < countEnemies)
        {
            Invoke("InstanceEnemy", timer);
            timer += .5f;
            i++;
        }

        ActivationAllColliders(false);
        StartCoroutine(DelayActivation(2f, true));
    }
    #endregion


    [Serializable]
    internal struct Diapason
    {
        public float start;
        public float end;
    }
}
