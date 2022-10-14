using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ФИНИШ НА 126 строчке
/// </summary>
public class FullAir : MonoBehaviour
{
    [Header("Объект")]
    public Transform GasStation;
    [Header("Расстояние пистолета от камеры при зажатии клика")]
    public float Zposition;
    [Header("Угол поворота Аира")]
    public float AirAngleZ;

    private Action update;
    private Transform cameraBagStationPlace;
    private DragObject pistol;

   // private Text testText;
    private float fullCount = 0;

    private Quaternion cameraStartRotation;
    private Vector3 cameraStartPosition;

    private void Start()
    {
        cameraBagStationPlace = GasStation.transform.GetChild(0);
        pistol = GasStation.transform.GetChild(1).GetComponent<DragObject>();
        pistol.OnSetPistol += StartFulling;
      //  testText = LevelManager.instance.canvas.transform.GetChild(0).GetComponent<Text>();
        FixPosition();
    }

    void OnEnable()
    {
        update = CameraParkingToGasStation;
        GasStation.gameObject.SetActive(true);
    }


    
    private void FixedUpdate()
    {
        update();

        pistol.Zposition = Zposition;
    }

    /// <summary>
    /// Отвечающий за плавный переход камеры и поворот аира, для дальнейшей заправки
    /// </summary>
    private void CameraParkingToGasStation()
    {
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, cameraBagStationPlace.position, Time.deltaTime * 2f);
        Camera.main.transform.rotation = Quaternion.Lerp(Camera.main.transform.rotation, cameraBagStationPlace.rotation, Time.deltaTime * 2f);

        var heading = Camera.main.transform.position - cameraBagStationPlace.position;

        this.transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(-90, AirAngleZ, 0), Time.deltaTime * 4f);

        if (heading.magnitude < 0.01)
        {
            update = delegate {
             //   Debug.Log($"");
            };
        }
    }

    /// <summary>
    /// Возвращает камеру и вертолет на места
    /// !!! ВНИМАНИЕ !!! Quaternion.Euler(-90, ???, 0) Может неправильно вовзращать вертолет
    /// </summary>
    private void CameraParkingToStartPos()
    {
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, cameraStartPosition, Time.deltaTime * 2f);
        Camera.main.transform.rotation = Quaternion.Lerp(Camera.main.transform.rotation, cameraStartRotation, Time.deltaTime * 2f);

        var heading = Camera.main.transform.position - cameraStartPosition;

        this.transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(-90, 180, 0), Time.deltaTime * 2f);

        if (heading.magnitude < 0.01)
        {
            update = delegate { };
        }
    }

    /// <summary>
    /// Вызывается с DragObject через евент OnSetPistol
    /// Запускает механику заправки самолета
    /// </summary>
    private void StartFulling()
    {
       // pistol.OnSetPistol -= StartFulling;
        pistol.DragOff();
       // testText.gameObject.SetActive(true);
        fullCount = 3f;

        update = delegate
        {
        //    testText.text = fullCount.ToString();

            if (Input.GetMouseButton(0))
            {
                fullCount += Time.deltaTime;
            }
            else
            {
                fullCount -= Time.deltaTime;
            }
            if (fullCount < 0)
            {
                pistol.DragOn();
                pistol.ParkingBackPistol();
       //         testText.gameObject.SetActive(false);
                update = delegate { };
            }
            else if(fullCount > 4)
            {
                Destroy(GasStation.gameObject);
        //        testText.text = "finish";
                update = CameraParkingToStartPos; // ФИНИШ ////////////////////////////////////////////////////////////////////////////
            }
        };
    }

    /// <summary>
    /// Фиксирует стартовую позицию камеры чтобы туда её возвращать в случае потери
    /// </summary>
    private void FixPosition()
    {
        cameraStartRotation = Camera.main.transform.rotation;
        cameraStartPosition = Camera.main.transform.position;
    }


}
