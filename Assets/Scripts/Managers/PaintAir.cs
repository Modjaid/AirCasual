using PaintIn3D;
using PaintIn3D.Examples;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


/// <summary>
/// Класс отслеживает балл Отмывки или покраски Аира, на евент окончания подписан GameData
/// </summary>
public class PaintAir : MonoBehaviour
{
    [Header("Объект карандаш лежит в иерархии объектов")]
    public GameObject HitColorObject;
    public GameObject HitTextureObject;

    public List<P3dChangeCounter> Counters { get { if (counters == null) counters = new List<P3dChangeCounter>(); return counters; } }
    [SerializeField] private List<P3dChangeCounter> counters;

    private float percent;
    public float Percent
    {
        get
        {
            return (100 * percent) / finishPoint;
        }
    }

    public bool Inverse { set { inverse = value; } get { return inverse; } }

    [Tooltip("Иногда полезно инверсировать итератор балла покраски")]
    [SerializeField] private bool inverse;

    public UnityEvent OnFinish3DPainting;

    [Tooltip("На скольки баллах можно заканчивать")]
    [Range(0,70)]
    public float finishColorPaint;
    [Range(0, 70)]
    public float finishTexturePaint;
    private float finishPoint;

    [Header("Дополнительные объекты которые закрашивать на финише")]
    [SerializeField] public List<P3dPaintable> addedP3dStuff;

    private AudioSource paintSource;
    private Quaternion startRot;

    private Quaternion rotationY;

    [Header("TEST")]
    public float R;
    public float G;

    private void RotationObject()
    {
        #region МЕХАНИКА ВРАЩЕНИЯ Старая версия
        /*
        if (Input.GetMouseButton(0))
        {
            float mouseX = Input.mousePosition.x - (Screen.width / 2);
            float halfScreen = Screen.currentResolution.width / 2;

            Quaternion rotationY;

            if (Input.mousePosition.x > (Screen.width / 2 + Screen.width / 3))
            {
                rotationY = Quaternion.AngleAxis(mouseX * (0.005f), Vector3.forward);
                transform.rotation *= rotationY;
            }
            else if (Input.mousePosition.x < (Screen.width / 2 - Screen.width / 3))
            {
 
                rotationY = Quaternion.AngleAxis(mouseX * (-0.005f), Vector3.back);
                transform.rotation *= rotationY;
            }
            paintSource.pitch = UnityEngine.Random.Range(1f, 1.2f);
            paintSource.PlayOneShot(paintSource.clip);
        }
        */
        #endregion

        #region Новая механика вращения
        if (Input.GetMouseButton(0))
        {
            float mouseWidth = Input.mousePosition.x;
            float halfScreen = Screen.width / 2;
            float deathSector = Screen.width / 20;
            


            if (mouseWidth > halfScreen + deathSector)
            {
                var speed = mouseWidth - (halfScreen + deathSector);
                rotationY = Quaternion.AngleAxis(SpeedStabilize(speed), Vector3.forward);
                transform.rotation *= rotationY;
            }
            else if(mouseWidth < halfScreen - deathSector)
            {
                var speed = (mouseWidth - halfScreen + deathSector) * -1;
                rotationY = Quaternion.AngleAxis(SpeedStabilize(speed), Vector3.back);
              //  transform.rotation *= rotationY;
            }
            paintSource.pitch = UnityEngine.Random.Range(1f, 1.2f);
            paintSource.PlayOneShot(paintSource.clip);
        }

        transform.rotation = Quaternion.Lerp(transform.rotation,rotationY,Time.deltaTime * 1f);
        #endregion
    }




    private float SpeedStabilize(float speed)
    {
        float maxSpeed = 3f;
        float acceleration = Time.deltaTime * 0.6f;
        if(speed * acceleration > maxSpeed)
        {
      //      Debug.Log("speed " + maxSpeed);
            return maxSpeed;
        }
     //   Debug.Log("speed " + speed * acceleration);
        return speed * acceleration;
    }

    public void EnableScript(Color newColor, AudioSource paintSource)
    {
        this.enabled = true;
        finishPoint = finishColorPaint;
        startRot = this.transform.rotation;
        rotationY = startRot;
        StartCoroutine(StartPaint());
        HitColorObject.SetActive(true);
        HitColorObject.GetComponent<P3dPaintSphere>().Color = newColor;
        this.paintSource = paintSource;

    }

    public void EnableScript(AudioSource paintSource)
    {
        this.enabled = true;
        startRot = this.transform.rotation;
        rotationY = startRot;
        HitColorObject.SetActive(true);
        StartCoroutine(StartPaint());
        this.paintSource = paintSource;
    }

    public void EnableScript(Texture newTexture, AudioSource paintSource)
    {
        this.enabled = true;
        finishPoint = finishTexturePaint;
        startRot = this.transform.rotation;
        rotationY = startRot;
        StartCoroutine(StartPaint());
        HitTextureObject.SetActive(true);
        var blendMode = HitTextureObject.GetComponent<P3dPaintSphere>().BlendMode;
        blendMode.Texture = newTexture;
        HitTextureObject.GetComponent<P3dPaintSphere>().BlendMode = blendMode;
        this.paintSource = paintSource;

    }

    public void DisableScript()
    {
        //  this.GetComponent<Animation>().Play("Rotation");
        HitColorObject.SetActive(false);
        HitTextureObject.SetActive(false);
        this.enabled = false;

    }

    public void FinishColor(Material stdMaterial)
    {
        var color = GetMainColor();

        this.GetComponent<P3dPaintable>().CachedRenderer.material = stdMaterial;
        this.GetComponent<P3dPaintable>().CachedRenderer.material.color = GetMainColor();

        foreach(P3dPaintable stuff in addedP3dStuff)
        {
            stuff.CachedRenderer.material = stdMaterial;
            stuff.CachedRenderer.material.color = GetMainColor();
        }
    }
    public void FinishTexture(Material stdMaterial)
    {
        var texture = GetMainTexture();

        this.GetComponent<P3dPaintable>().CachedRenderer.material = stdMaterial;
        this.GetComponent<P3dPaintable>().CachedRenderer.material.mainTexture = texture;

        foreach (P3dPaintable stuff in addedP3dStuff)
        {
            stuff.CachedRenderer.material = stdMaterial;
            stuff.CachedRenderer.material.mainTexture = texture;
        }
    }
    public void FinishTexture(Material stdMaterial,Texture texture)
    {
        this.GetComponent<P3dPaintable>().CachedRenderer.material = stdMaterial;
        this.GetComponent<P3dPaintable>().CachedRenderer.material.mainTexture = texture;

        foreach (P3dPaintable stuff in addedP3dStuff)
        {
            stuff.CachedRenderer.material = stdMaterial;
            stuff.CachedRenderer.material.mainTexture = texture;
        }
    }



    public Color GetMainColor()
    {
        Color color = HitColorObject.GetComponent<P3dPaintSphere>().Color;
        color = new Color(color.r + R,color.g + G,color.b);


        return color;
    }
    public Texture GetMainTexture()
    {
        return HitTextureObject.GetComponent<P3dPaintSphere>().BlendMode.Texture;
    }


    IEnumerator StartPaint()
    {
        OnFinish3DPainting.AddListener(OnFinishFunc);

        while (true)
        {
            

            #region Новая механика вращения
            if (Input.GetMouseButton(0))
            {
                float mouseWidth = Input.mousePosition.x;
                float halfScreen = Screen.width / 2;
                float deathSector = Screen.width / 12;


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
                paintSource.pitch = UnityEngine.Random.Range(1f, 1.2f);
                paintSource.PlayOneShot(paintSource.clip);
            }
            #endregion

            #region BallCounter
            var finalCounters = counters.Count > 0 ? counters : null;
            var total = P3dChangeCounter.GetTotal(finalCounters);
            var count = P3dChangeCounter.GetCount(finalCounters);

            if (inverse == true)
            {
                count = total - count;
            }

            percent = P3dHelper.RatioToPercentage(P3dHelper.Divide(count, total), 0);
            if (percent >= finishPoint)
            {
                OnFinish3DPainting?.Invoke();
            }
            Debug.Log("BALL PAINT: " + percent);
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
        StopAllCoroutines();
        HitColorObject.SetActive(false);
        HitTextureObject.SetActive(false);
        this.enabled = false;
    }

    private void OnFinishFunc()
    {
        StopAllCoroutines();
        StartCoroutine(FinishRotation());
        StartCoroutine(Victory());
    }

}
