using MoreMountains.NiceVibrations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// Заспавненый враг прежде чем преследовать Defenderа должен достичь точки attackPoint
/// После должен идти с огнем предерживаясь дистанции с рандомной погрешностью
/// </summary>
public class FireCover : AirPark
{
    public UnityEvent<bool> finishGame;

    public static FireCover Instance { get; private set; }
    public Spawn[] enemySpawns;
    public Enemy mainDefender;
    public Enemy[] followDefenders;
    private BezierTest[] followDefenderPaths;
    public Image[] slideBarImages;
    public HealthData[] healthDefenderData;
    private HealthData mainHealthData;
    public float speedDefender;
    public float healthDefender;
    public float healthFollowDefender;
    
    [SerializeField] private Transform aim;
    [SerializeField] private GameObject[] weapons;
    [SerializeField] private GameObject shotEffect;
    [SerializeField] private GameObject collisionEffect;

    [SerializeField] private GameObject botPrefab;


    private static System.Random rng = new System.Random();

    [SerializeField] private float cooldawn;
    private Transform weapon;
    [SerializeField] private AudioSource shotSound;
    private float restCooldawn;
    private bool IsReadyShot
    {
        get
        {
            if (restCooldawn <= 0)
            {
                restCooldawn = cooldawn;
                return true;
            }
            return false;
        }
    }

    private BezierTest defenderPath;
    public Image slideBarImage;

    private List<GameObject> enemies;

    [SerializeField] private GameObject[] locations;

    #region Joystick
    [Header("JOYSTICK")]
    [SerializeField] private DragHandler DragHandler;
    [SerializeField] private float aimMoveSpeed = 2;
    [SerializeField] private Diapason aimDiapasonX;
    [SerializeField] private Diapason aimDiapasonY;


    private Vector3 aimNewPos;
    private Vector3 muzzleRot;
    [SerializeField] private LayerMask enemyLayer;
    #endregion

    private void Awake()
    {
        base.Awake();
        Instance = this;
    }

    private void Start()
    {

        SpawnRandomBots();
        enemies = new List<GameObject>();

        mainDefender.OnDeath += (enemy) => finishGame?.Invoke(false);
        mainDefender.OnDeath += (enemy) => FailGame();
        mainDefender.health = healthDefender;

        mainHealthData = new HealthData(healthDefender,100);
        mainDefender.OnDamaged += (damage) => mainHealthData.RoundPoints -= damage;

        defenderPath = mainDefender.transform.parent.gameObject.GetComponent<BezierTest>();
        defenderPath.speed = speedDefender;
        weapon = weapons[0].transform.parent;
        initJoystick();
    }

    public void EnableScript(GameObject air,int levelNum, int indexWeapon,Weapon selectHangarWeapon, int followDefendersCount)
    {
        this.enabled = true;
        selectHangarWeapon.gameObject.SetActive(false);
        air.transform.parent = this.transform;
        air.transform.localPosition = new Vector3(0, 0, 0);
        air.transform.localRotation = Quaternion.Euler(0, 0, 0);
        air.transform.localScale = new Vector3(20, 20, 20);
        weapons[indexWeapon].SetActive(true);

        ActivationLocation(levelNum);

        #region INIT FOLLOW DEFENDERS
        if (followDefendersCount > followDefenders.Length) followDefendersCount = followDefenders.Length;
        healthDefenderData = new HealthData[followDefendersCount];
        followDefenderPaths = new BezierTest[followDefendersCount];

        for (int i = 0; i < followDefendersCount; i++)
        {
            followDefenders[i].health = healthFollowDefender;
            followDefenderPaths[i] = followDefenders[i].transform.parent.gameObject.GetComponent<BezierTest>();
            healthDefenderData[i] = new HealthData(healthFollowDefender, 100);
            followDefenderPaths[i].speed = speedDefender;
            followDefenders[i].OnDeath += (enemy) => DeathFollowDefender(enemy);
            followDefenders[i].OnDamaged += (damage) => healthDefenderData[i].RoundPoints -= damage;
            followDefenderPaths[i].gameObject.SetActive(true);
        }
        #endregion
    }
    public void EnableScript(GameObject air,int levelNum, int indexWeapon, int followDefendersCount)
    {
        this.enabled = true;
        air.transform.parent = this.transform;
        air.transform.localPosition = new Vector3(0, 0, 0);
        air.transform.localRotation = Quaternion.Euler(0, 0, 0);
        air.transform.localScale = new Vector3(20, 20, 20);
        weapons[indexWeapon].SetActive(true);

        ActivationLocation(levelNum);

        #region INIT FOLLOW DEFENDERS
        if (followDefendersCount > followDefenders.Length) followDefendersCount = followDefenders.Length;
        healthDefenderData = new HealthData[followDefendersCount];
        followDefenderPaths = new BezierTest[followDefendersCount];

        for (int i = 0; i < followDefendersCount; i++)
        {
            followDefenders[i].health = healthFollowDefender;
            followDefenderPaths[i] = followDefenders[i].transform.parent.gameObject.GetComponent<BezierTest>();
            healthDefenderData[i] = new HealthData(healthFollowDefender, 100);
            followDefenderPaths[i].speed = speedDefender;
            followDefenders[i].OnDeath += (enemy) => DeathFollowDefender(enemy);
          //  followDefenders[i].OnDamaged += (damage) => healthDefenderData[i].RoundPoints -= damage;
            followDefenderPaths[i].gameObject.SetActive(true);
        }
        #endregion
    }
    public void EnableScript(AirType airType, int indexWeapon)
    {
        this.enabled = true;
        InstanceAir(airType);
        weapons[indexWeapon].SetActive(true);
    }
    private void TestEnable(int indexWeapon)
    {
        weapons[indexWeapon].SetActive(true);
    }

    private void FixedUpdate()
    {
        InputClick();
    }

    private void Update()
    {
        controlFinishPath();

        slideBarImage.fillAmount = mainHealthData.GetRoundPointNormalized();

        for(int i = 0; i < healthDefenderData.Length; i++)
        {
            healthDefenderData[i].Update();
            slideBarImages[i].fillAmount = healthDefenderData[i].GetBarNormalized();

        }
    }

    private void SpawnBots()
    {
        foreach(var spawn in enemySpawns)
        {
            var bot = Instantiate(botPrefab, spawn.spawnPoint.position, Quaternion.identity).GetComponent<RunBot>();

            bot.SetAttackPoint(spawn.attackPoint);
        }
    }
    private void SpawnRandomBots()
    {
        Spawn[] randSpawns = enemySpawns;

        randSpawns = reshuffle(randSpawns);
        StartCoroutine(DelaySpawns());

        IEnumerator DelaySpawns()
        {
            while (true)
            {
                foreach (var spawn in randSpawns)
                {
                    var bot = Instantiate(botPrefab, spawn.spawnPoint.position, Quaternion.identity).GetComponent<RunBot>();
                    yield return new WaitForSeconds(0.1f);
                      enemies.Add(bot.gameObject);
                    bot.SetAttackPoint(spawn.attackPoint);

                    yield return new WaitForSeconds(0.5f);
                }
                randSpawns = reshuffle(randSpawns);
                yield return new WaitForSeconds(1f);
            }
        }
    }

    [System.Serializable]
    public class Spawn
    {
        [Header("Точка спавна врага")]
        public Transform spawnPoint;
        [Header("Точка до куда бежит заспавненый враг прежде чем начать преследование")]
        public Transform attackPoint;
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

        aimNewPos.x -= input.y * aimMoveSpeed;
        aimNewPos.z += input.x * aimMoveSpeed;
        aimNewPos.x = Mathf.Clamp(aimNewPos.x, aimDiapasonY.start, aimDiapasonY.end);
        aimNewPos.z = Mathf.Clamp(aimNewPos.z, aimDiapasonX.start, aimDiapasonX.end);

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
        restCooldawn -= Time.deltaTime;
        
        if (Input.GetMouseButton(0)&& IsReadyShot)
        {
            aim.gameObject.SetActive(true);
            shotEffect.SetActive(true);
            IsDirectHit(out hit);
            muzzleRot = hit.point;
            muzzleRot = new Vector3(muzzleRot.x, muzzleRot.y - 3.5f, muzzleRot.z);
            shotSound.pitch = UnityEngine.Random.Range(1f, 1.2f);
            shotSound.PlayOneShot(shotSound.clip);
            MMVibrationManager.ContinuousHaptic(.9f, .07f, .4f);
        }else if (!Input.GetMouseButton(0))
        {
            shotEffect.SetActive(false);
            aim.gameObject.SetActive(false);
        }
        Vector3 dir = muzzleRot - weapon.position;
        Quaternion rot = Quaternion.LookRotation(dir);
        weapon.rotation = Quaternion.Lerp(weapon.rotation,rot,10f * Time.deltaTime);
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

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, enemyLayer))
        {
            if (hit.collider.tag == "Enemy")
            {
                hit.collider.GetComponent<Enemy>().GetDamage(2, hit.point, (enemy) => enemy.GetComponent<RunBot>().Die());

            }
            GameObject newEffect = Instantiate(collisionEffect);
            newEffect.transform.position = hit.point;
        }
        return false;
    }

    #endregion

    private void controlFinishPath()
    {
        if(defenderPath.t > .95f)
        {
            mainDefender.gameObject.SetActive(false);
            finishGame?.Invoke(true);
        }
    }

    private void ClearEnemies()
    {
        foreach(GameObject enemy in enemies)
        {
            if(enemy != null)
            {
                Destroy(enemy);
            }
        }
        enemies.Clear();
    }

    public void RestartGame()
    {
        StopAllCoroutines();
        ClearEnemies();
        defenderPath.t = 0;
        defenderPath.speed = speedDefender;
        mainDefender.health = healthDefender;
        mainHealthData.Restart();
        mainDefender.transform.GetChild(0).GetComponent<Animator>().SetTrigger("Run");

        for (int i = 0; i < healthDefenderData.Length; i++)
        {
            followDefenderPaths[i].speed = speedDefender;
            followDefenders[i].health = healthFollowDefender;
            healthDefenderData[i].Restart();
            followDefenders[i].transform.GetChild(0).GetComponent<Animator>().SetTrigger("Run");
            if (i < 2)
            {
                followDefenderPaths[i].t = 0;
            }
            else
            {
                followDefenderPaths[i].t = 0.025f;
            }

        }

        SpawnRandomBots();
      //  defender.transform.GetChild(0).GetComponent<Animator>().parameters[4].defaultBool = false;
        StartCoroutine(delat());

        IEnumerator delat()
        {
            yield return new WaitForSeconds(0.2f);
            mainHealthData.Restart();
            slideBarImage.fillAmount = 1;

            for (int i = 0; i < healthDefenderData.Length; i++)
            {
                healthDefenderData[i].Restart();
                slideBarImages[i].fillAmount = 1;
            }
        }
    }

    private void FailGame()
    {
        // defender.gameObject.SetActive(false);
        mainDefender.GetComponent<Enemy>().health = 10000;
        defenderPath.speed = 0;
        mainDefender.transform.GetChild(0).GetComponent<Animator>().SetTrigger("Death");
        LevelManager.instance.RestartPanelUp();
    }

    private void DeathFollowDefender(Enemy enemy)
    {
        enemy.health = 10000;
        enemy.transform.GetChild(0).GetComponent<Animator>().SetTrigger("Death");
        enemy.transform.parent.GetComponent<BezierTest>().speed = 0;
    }

    Spawn[] reshuffle(Spawn[] spawns)
    {
        // Knuth shuffle algorithm :: courtesy of Wikipedia :)
        for (int t = 0; t < spawns.Length; t++)
        {
            Spawn tmp = spawns[t];
            int r = UnityEngine.Random.Range(t, spawns.Length);
            spawns[t] = spawns[r];
            spawns[r] = tmp;
        }
        return spawns;
    }

    private void ActivationLocation(int lvlNum)
    {
        MapType map = Map.GetMapType(lvlNum);
        locations[(int)map].SetActive(true);
    }

    [Serializable]
    internal struct Diapason
    {
        public float start;
        public float end;
    }

}
