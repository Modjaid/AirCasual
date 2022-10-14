using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirPark : MonoBehaviour
{
    [Multiline(7)]
    [SerializeField]private string Порядок = 
        "AN64\n" +
        "AC130\n" +
        "F22\n" +
        "F15\n" +
        "F16\n"+
        "UN60\n";
    [Header("Обязательно столько же сколько и дочерних объектов")]
    public GameObject[] airPrefabs;
    
    private Transform[] airPlaces;

    public GameObject CurrentAir { get; set; }

    public Animation scenePanel;

    public void Awake()
    {
        airPlaces = new Transform[transform.childCount];
        for(int i = 0; i < airPlaces.Length; i++)
        {
            airPlaces[i] = transform.GetChild(i);
        }
    }

    public GameObject InstanceAir(AirType type)
    {
        Transform parent = airPlaces[(int)type];
        
        GameObject newAir = Instantiate(airPrefabs[(int) type], parent);
        return newAir;
    }
    public GameObject InstanceAir(AirType type,GameObject airPrefab)
    {
        GameObject newAir = Instantiate(airPrefab, airPlaces[(int)type]);
        return newAir;
    }
    public Transform getAirPlace(AirType airType)
    {
        return airPlaces[(int)airType];
    }

  //  public IEnumerator await()
  //  {
  //      yield return new WaitForSeconds(4.4f);
  //      Next = true;
  //  }


}

public enum AirType
{
    AN64    ,
    AC130   ,
    F22     ,
    F15     ,
    F16     ,
    UN60
    
}
