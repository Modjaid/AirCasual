using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleHelicopter : MonoBehaviour
{
    public int maxHeight = 8;
    public int minHeight = 6;
    public int shootHeight = 8;
    public float speed = 5f;
    public WallAirPark wallLocation;

    public HealthData healthData;
    [Header("Индикатор здоровья")]
    public Image slideBarImage;

    private Weapon[] weapons;
    private Transform currentEnemy;

    [HideInInspector] public GameObject currentAir;

    #region DELETE
    private void Start()
    {
      // wallLocation.InstantiateNewEnemy_Immidately();
      //
      // wallLocation.OnFinishBattle += DisableScript;
      //
      //
      // var health = GetComponent<Enemy>().health;
      // healthData = new HealthData(health);
      // GetComponent<Enemy>().OnDamaged += (damage) => healthData.RoundPoints -= damage;
    }
    #endregion


    void FixedUpdate()
    {
        float posY = transform.position.y;
        if (currentEnemy == null)
        {
            currentEnemy = WallAirPark.GetEnemy();
            currentEnemy.GetComponent<BotWall>().TurnOnOutLine();
        }


        if (Input.GetMouseButton(0))
        {
            if (posY < maxHeight)
            {
                Vector3 position = transform.position;
                position.y += speed * Time.deltaTime;
                transform.position = position;
            }


            var enemy = currentEnemy.transform.position;
            Vector3 direction = new Vector3(enemy.x,enemy.y + 300, enemy.z) - transform.position;
            Quaternion rot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, rot, 1 * Time.deltaTime);
        }
        else
        {
            var enemy = currentEnemy.transform.position;
            Vector3 direction = new Vector3(enemy.x, enemy.y + 300, enemy.z) - transform.position;
            Quaternion rot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, rot, 1 * Time.deltaTime);

            if (posY > minHeight)
            {
                Vector3 position = transform.position;
                position.y -= speed * Time.deltaTime;
                transform.position = position;
            }
        }
  
         if (posY >= shootHeight)
         {

            foreach (Weapon weapon in weapons)
            {

                weapon.SetTargetPos(currentEnemy);
                weapon.Shot();
            }

         }

        healthData.Update();
        slideBarImage.fillAmount = healthData.GetBarNormalized();
    }

    /// <summary>
    /// this.enabled = true;
    /// air.transform.parent = this.transform;
    /// weapons = air.GetWeapons();
    /// WallAirPark.OnDeathEnemy += () => currentEnemy = WallAirPark.GetEnemy();
    /// </summary>
    public void EnableScript(GameObject air, int currentLvl)
    {
        transform.GetChild(0).gameObject.SetActive(true); // Индикатор здоровья должен быть виден только в этой механике
        this.enabled = true;
        air.transform.parent = this.transform;
        air.transform.position = this.transform.position;
        air.transform.rotation = this.transform.rotation;
        air.transform.localScale = air.transform.localScale * 3;
        currentAir = air;
        weapons = air.GetComponent<EquipWeapon>().GetWeapons();
        WallAirPark.ActiveMap(currentLvl);

        wallLocation.InstantiateNewEnemy_Immidately();

        wallLocation.OnFinishBattle += DisableScript;


        var health = GetComponent<Enemy>().health;
        healthData = new HealthData(health);
        GetComponent<Enemy>().OnDamaged += (damage) => healthData.RoundPoints -= damage;
        GetComponent<Enemy>().OnDeath += OnDeath;
    }

    public void DisableScript()
    {
        Debug.Log("Победа");
        GetComponent<Enemy>().enabled = false;
        this.enabled = false;
    } 

    /// <summary>
    /// Вызывается кнопкой
    /// </summary>
    public void Restart()
    {
        this.gameObject.SetActive(true);
        wallLocation.removeAllEnemies();
        LevelManager.instance.RestartPanelDown();
        wallLocation.InstantiateNewEnemy_Immidately();
        healthData.Restart();
        GetComponent<Enemy>().health = healthData.slideBarDiapason;
        currentAir.SetActive(true);

    }

    private void OnDeath(Enemy air)
    {
        LevelManager.instance.RestartPanelUp();
        air.gameObject.SetActive(false);
    }


}
