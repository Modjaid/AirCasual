using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[ExecuteInEditMode]

public class ProgressBar : MonoBehaviour
{

    [Header("Title Setting")]
    public string Title;
    public Color TitleColor;
    public Font TitleFont;
    public int TitleFontSize = 10;

    [Header("Bar Setting")]
    public Color BarColor;   
    public Color BarBackGroundColor;
    public Sprite BarBackGroundSprite;
    [Range(1f, 100f)]
    public int Alert = 20;
    public Color BarAlertColor;

    [Header("Sound Alert")]
    public AudioClip sound;
    public bool repeat = false;
    public float RepeatRate = 1f;

    private Image bar, barBackground;
    private float nextPlay;
    private AudioSource audiosource;
    private Text txtTitle;
    private float barValue;
    public float BarValue
    {
        get { return barValue; }

        set
        {
            value = Mathf.Clamp(value, 0, 100);
            barValue = value;
            UpdateValue(barValue);

        }
    }

    [HideInInspector] public PaintAir paintAir;

        

    private void Awake()
    {
        bar = transform.Find("Bar").GetComponent<Image>();
        barBackground = GetComponent<Image>();
        txtTitle = transform.Find("Text").GetComponent<Text>();
        barBackground = transform.Find("BarBackground").GetComponent<Image>();
        audiosource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        txtTitle.text = Title;
        txtTitle.color = TitleColor;
        txtTitle.font = TitleFont;
        txtTitle.fontSize = TitleFontSize;

        bar.color = BarColor;
        barBackground.color = BarBackGroundColor; 
        barBackground.sprite = BarBackGroundSprite;

        UpdateValue(barValue);
        StartCoroutine(CallNextButton());
        paintAir.OnFinish3DPainting.AddListener(DisableScript);
    }

    void UpdateValue(float val)
    {
        bar.fillAmount = val / 100;
        txtTitle.text = Title + " " + val + "%";

        if (Alert >= val)
        {
            bar.color = BarAlertColor;
        }
        else
        {
            bar.color = BarColor;
        }

    }


    private void Update()
    {
        //    if (!Application.isPlaying)
        //    {           
        //        UpdateValue(50);
        //        txtTitle.color = TitleColor;
        //        txtTitle.font = TitleFont;
        //        txtTitle.fontSize = TitleFontSize;
        //
        //        bar.color = BarColor;
        //        barBackground.color = BarBackGroundColor;
        //
        //        barBackground.sprite = BarBackGroundSprite;
        //    }
        //    else
        //    {
        //        if (Alert >= barValue && Time.time > nextPlay)
        //        {
        //            nextPlay = Time.time + RepeatRate;
        //            audiosource.PlayOneShot(sound);
        //        }
        //    }

        try
        {
            BarValue = paintAir.Percent;
        }catch(NullReferenceException x)
        {

        }
    }

    public void EnableScript()
    {
        this.enabled = true;
        GetComponent<Animation>().Play("Up");
    }

    public void DisableScript()
    {
        GetComponent<Animation>().Play("Down");
    }

    public void ClickNextButton()
    {
        paintAir.OnFinish3DPainting?.Invoke();
    }

    private IEnumerator CallNextButton()
    {
        bool IsPlay = true;
        while (IsPlay)
        {
            if (BarValue > 70)
            {
                GetComponent<Animation>().Play("2Up");
                IsPlay = false;
            }
            yield return null;
        }
    }

}
