using PaintIn3D;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintDecalAir : MonoBehaviour
{

    [SerializeField]private GameObject DecalPainting;
    private AudioSource decalSound;
    private Quaternion startRot;

    public event Action OnFinishDecal;

    public void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            decalSound.Play();
        }
    }




    public void EnableDecal(Texture texture,AudioSource decalSound)
    {
        this.enabled = true;
        startRot = this.transform.rotation;
        StartCoroutine(StartPaint());
        this.decalSound = decalSound;
        DecalPainting.SetActive(true);
        DecalPainting.GetComponent<P3dPaintDecal>().Texture = texture;
    }


    public void SetDecal(Texture texture)
    {
        DecalPainting.GetComponent<P3dPaintDecal>().Texture = texture;
    }

    IEnumerator StartPaint()
    {
        while (true)
        {
            if (Input.GetMouseButtonUp(0))
            {
                decalSound.Play();
            }
            #region Новая механика вращения
            if (Input.GetMouseButton(0))
            {
                float mouseWidth = Input.mousePosition.x;
                float halfScreen = Screen.width / 2;
                float deathSector = Screen.width / 20;


                Quaternion rotationY;

                if (mouseWidth > halfScreen + deathSector)
                {
                    var speed = mouseWidth - (halfScreen + deathSector);
                    rotationY = Quaternion.AngleAxis(SpeedStabilize(speed), Vector3.forward);
                    transform.rotation *= rotationY;
                }
                else if (mouseWidth < halfScreen - deathSector)
                {
                    var speed = (mouseWidth - halfScreen + deathSector) * -1;
                    rotationY = Quaternion.AngleAxis(SpeedStabilize(speed), Vector3.back);
                    transform.rotation *= rotationY;
                }
            }
            #endregion

            yield return null;
        }
    }

    IEnumerator FinishRotation()
    {
        while (true)
        {
            this.transform.rotation = Quaternion.Lerp(this.transform.rotation, startRot, 6 * Time.deltaTime);
            yield return null;
        }
    }
    IEnumerator Victory()
    {
        yield return new WaitForSeconds(1f);
        OnFinishDecal?.Invoke();
        StopAllCoroutines();
        DecalPainting.SetActive(false);
        this.enabled = false;
    }

    private float SpeedStabilize(float speed)
    {
        float maxSpeed = 3f;
        float acceleration = Time.deltaTime * 0.6f;
        if (speed * acceleration > maxSpeed)
        {
            Debug.Log("speed " + maxSpeed);
            return maxSpeed;
        }
        Debug.Log("speed " + speed * acceleration);
        return speed * acceleration;
    }

    public void ClickFinishDecal()
    {
        StopAllCoroutines();
        StartCoroutine(FinishRotation());
        StartCoroutine(Victory());
    }
}
