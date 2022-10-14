using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallAirPark : AirPark
{
    #region INSPECTOR
    public GameObject[] enemyPrefabs;

    public int countEnemy;

    public Collider shotBox;

    public Transform cameraPlace;

    public BattleHelicopter battleHelicopter;

    public Transform[] enemySpawns;

    public GameObject[] maps;

    [SerializeField]
    public ObjectsStruct[] hideObjs;

    [Range(0,10)]
    public float instanceNewEnemyTimer;
    #endregion

    private static Bounds shotBounds;

    private int currentCountEnemy;

    public event Action OnFinishBattle;

    private static WallAirPark instance;

    public static List<Enemy> enemies;

    public void Start()
    {
        instance = this;
        enemies = new List<Enemy>();
        currentCountEnemy = countEnemy;
        shotBounds = shotBox.bounds;
    }

    public static Transform GetEnemy()
    {
        if(enemies.Count > 0)
        {
            int random = UnityEngine.Random.Range(0, enemies.Count);
            return enemies[random].transform;
        }
        return null;
    }

    public static Vector3 RandomPointInBounds()
    {
        return new Vector3(
            UnityEngine.Random.Range(shotBounds.min.x, shotBounds.max.x),
            UnityEngine.Random.Range(shotBounds.min.y, shotBounds.max.y),
            UnityEngine.Random.Range(shotBounds.min.z, shotBounds.max.z)
        );
    }

    public void InstantiateNewEnemy()
    {
        foreach(Transform spawn in enemySpawns)
        {
            StartCoroutine(delay(spawn));
        }

        IEnumerator delay(Transform spawn)
        {
            yield return new WaitForSeconds(instanceNewEnemyTimer);

            if (spawn.childCount == 0 && currentCountEnemy > 0)
            {
                int random = UnityEngine.Random.Range(0, enemyPrefabs.Length);
                Enemy newEnemy = Instantiate(enemyPrefabs[random].GetComponent<Enemy>(), spawn);
                newEnemy.OnDeath += DeathControl;
                enemies.Add(newEnemy);
                currentCountEnemy--;
            }
        }
    }
    public void InstantiateNewEnemy_Immidately()
    {

        foreach (Transform spawn in enemySpawns)
        {
            if (spawn.childCount == 0 && currentCountEnemy > 0)
            {
                int random = UnityEngine.Random.Range(0, enemyPrefabs.Length);
                Enemy newEnemy = Instantiate(enemyPrefabs[random].GetComponent<Enemy>(), spawn);
                newEnemy.OnDeath += DeathControl;
                enemies.Add(newEnemy);
                currentCountEnemy--;
            }
        }
    }
    public void removeAllEnemies()
    {
        foreach (Transform spawn in enemySpawns)
        {
            if (spawn.childCount > 0)
            {
              var enemy = spawn.GetChild(0).gameObject;
                DestroyImmediate(enemy);
            }
        }
        enemies.Clear();
        currentCountEnemy = countEnemy;
    }

    /// <summary>
    /// Делает цвет локации активным исходя из уровня переданного от LevelManager
    /// </summary>
    /// <param name="currentLvl"></param>
    public static void ActiveMap(int currentLvl)
    {
        instance.maps[(int)Map.GetMapType(currentLvl)].SetActive(true);
    }

    /// <summary>
    /// Очищает площадку для передвижения вражеских юнитов нужно для EarthBattle
    /// </summary>
    /// <param name="currentLvl"></param>
    public static void HideObjs(int currentLvl)
    {
        GameObject[] objs = instance.hideObjs[(int)Map.GetMapType(currentLvl)].objs;
        for(int i = 0; i < objs.Length; i++)
        {
            objs[i].SetActive(false);
        }
    }

    public void SetCountEnemy(int countEnemy)
    {
        currentCountEnemy = countEnemy;
    }

    private void DeathControl(Enemy deathEnemy)
    {
        enemies.Remove(deathEnemy);
        InstantiateNewEnemy();
        if(enemies.Count == 0)
        {
            OnFinishBattle?.Invoke();
        }
    }

    /// <summary>
    /// Вызывается кнопкой из LevelManager
    /// </summary>
    public void Restart()
    {
        battleHelicopter.gameObject.SetActive(true);
        removeAllEnemies();
        InstantiateNewEnemy_Immidately();
        battleHelicopter.healthData.Restart();
        battleHelicopter.GetComponent<Enemy>().health = battleHelicopter.healthData.slideBarDiapason;
        battleHelicopter.currentAir.SetActive(true);
    }

    [Serializable]
    public struct ObjectsStruct
    {
        public GameObject[] objs;
    }
}


