using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Первый дочерний Трансформ EngineParent Должна быть камера
/// </summary>
public class EquipEngine : MonoBehaviour
{
    [Header("Родитель слотов")]
    public Transform EngineParent;
    [Header("Список слотов")]
    public List<Transform> EngineSlots;
    [Header("Угол поворота Аира")]
    public float AirAngleZ;

    public Transform CameraPlace;

    [Header("Дефолтный двигатель на случай если выбирать не придется")]
    public GameObject defaultEngine;



    /// <summary>
    /// Engine который был выбран через меню UI
    /// </summary>
    [HideInInspector] public GameObject currentEngine;



    public void StartAnimEngine()
    {
        foreach (Transform slot in EngineSlots)
        {
            if (slot.GetChild(0))
            {
                slot.GetChild(0).GetComponent<Animation>().Play("Screw");
            }
        }
    }

    /// <summary>
    /// Устанавливает движок якобы на слот но держит его в буфере
    /// </summary>
    /// <param name="EnginePrefab"></param>
    public void SetCurrentEngine(GameObject EnginePrefab)
    {
        currentEngine = EnginePrefab;
        foreach (Transform slot in EngineSlots)
        {
            if (slot.childCount > 0)
            {
                Destroy(slot.GetChild(0).gameObject);
            }

            Instantiate(currentEngine, slot);
        }
    }
    public void SetCurrentEngine(GameObject EnginePrefab, Color color,Material standardMaterial)
    {
        currentEngine = EnginePrefab;
        foreach (Transform slot in EngineSlots)
        {
            if (slot.childCount > 0)
            {
                Destroy(slot.GetChild(0).gameObject);
            }

           GameObject newEngine = Instantiate(currentEngine, slot);
           newEngine.GetComponent<Renderer>().material = standardMaterial;
           newEngine.GetComponent<MeshRenderer>().material.color = color;
        }
    }
    public void SetCurrentEngine(GameObject EnginePrefab, Texture newTexture, Material standardMaterial)
    {
        currentEngine = EnginePrefab;
        foreach (Transform slot in EngineSlots)
        {
            if (slot.childCount > 0)
            {
                Destroy(slot.GetChild(0).gameObject);
            }

            GameObject newEngine = Instantiate(currentEngine, slot);
            newEngine.GetComponent<Renderer>().material = standardMaterial;
            newEngine.GetComponent<MeshRenderer>().material.mainTexture = newTexture;
        }
    }
    public GameObject[] GetAllInstEngines()
    {
        List<GameObject> engineArr = new List<GameObject>();
        foreach (Transform slot in EngineSlots)
        {
            if (slot.childCount > 0)
            {
                engineArr.Add(slot.GetChild(0).gameObject);
            }
        }
        Debug.Log("COUNT  " + engineArr.Count);
        return engineArr.ToArray();
    }
}
