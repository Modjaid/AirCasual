using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragObject : MonoBehaviour
{
    public float Zposition { private get; set; }
    public event Action OnSetPistol;

    private bool Dragging;
    private Vector3 OffSet;

    private Quaternion oldPistolRotate;
    private Vector3 oldPistolPos;
    private Action update;

    private Transform bagTransform; // Бензоколонка
    private bool setOnBag = false;


    void Start()
    {
        gameObject.GetComponent<Rigidbody>().isKinematic = true;
        FixPosition();
        update = delegate ()
        {

        };
    }

    void FixedUpdate()
    {
        update();
        if (Dragging)
        {
            Vector3 position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Zposition);
            transform.position = Camera.main.ScreenToWorldPoint(position + new Vector3(OffSet.x, OffSet.y));
        }
    }

    private void OnMouseDown()
    {
        this.transform.localEulerAngles = new Vector3(-20, -90, -90);
      //  Debug.Log("BEFORE BEGIN");
        if (!Dragging)
        {
      //      Debug.Log("BEGIN");
            BeginDrag();
        }
    }

    /// <summary>
    /// Определяет при отпускании кнопки где находится пистолет, если в баке, то фиксирует в баке, иначе возвращает его на исходное
    /// </summary>
    private void OnMouseUp()
    {
        EndDrag();
        gameObject.GetComponent<Rigidbody>().useGravity = false;
        if (setOnBag)
        {
            gameObject.GetComponent<Rigidbody>().isKinematic = true;

            //Вставляет пистолет В бензобак
            update = delegate ()
            {
                transform.position = Vector3.Lerp(transform.position, bagTransform.position, Time.deltaTime * 4f);
                transform.rotation = Quaternion.Lerp(transform.rotation, bagTransform.rotation, Time.deltaTime * 4f);

                var heading = transform.position - bagTransform.position;

                if (heading.magnitude < 0.01f)
                {
                    OnSetPistol?.Invoke();
                }
            };
        }
        else
        {
            Invoke("ParkingBackPistol", 1f);
        }
    }

    public void BeginDrag()
    {
        Dragging = true;
        OffSet = Camera.main.WorldToScreenPoint(transform.position) - Input.mousePosition;
        gameObject.GetComponent<Rigidbody>().isKinematic = false;
    }

    public void EndDrag()
    {
        Dragging = false;
    }

    public void OnCollisionStay(Collision collision)
    {
        Debug.Log($"collision: {collision.gameObject.name}");
        if (collision.gameObject.name == "BagPlace")
        {
            setOnBag = true;
            bagTransform = collision.gameObject.transform;
        }
    }
    public void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.name == "BagPlace")
        {
            setOnBag = false;
        }
    }


    /// <summary>
    /// Отключает скрипт и ригидбади по причине что OnMouseDown(Up) продолжает работать даже при выключенном скрипте
    /// </summary>
    public void DragOff()
    {
        gameObject.GetComponent<BoxCollider>().enabled = false;
        gameObject.GetComponent<Rigidbody>().isKinematic = true;
        this.enabled = false;
    }

    /// <summary>
    /// Включает скрипт и ригидбади по причине что OnMouseDown(Up) продолжает работать даже при выключенном скрипте
    /// </summary>
    public void DragOn()
    {
        gameObject.GetComponent<BoxCollider>().enabled = true;
        gameObject.GetComponent<Rigidbody>().isKinematic = false;
        this.enabled = true;
    }

    /// <summary>
    /// Возвращает пистолет анимированно через делегат action()
    /// </summary>
    public void ParkingBackPistol()
    {
        gameObject.GetComponent<Rigidbody>().isKinematic = true;
        setOnBag = false;

        update = delegate ()
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, oldPistolPos, Time.deltaTime * 2f);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, oldPistolRotate, Time.deltaTime * 2f);
        };
    }

    /// <summary>
    /// Фиксирует стартовую позицию пистолета чтобы туда его возвращать в случае потери
    /// </summary>
    private void FixPosition()
    {
        oldPistolRotate = this.transform.localRotation;
        oldPistolPos = this.transform.localPosition;
    }

}
