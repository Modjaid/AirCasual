using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/// <summary>
/// На канвасе
/// - Обязательно придать физики самому аиру для рабочего Пуша
/// </summary>
public class MenuColorUI : MonoBehaviour
{

    [SerializeField] private GameObject menuColorPanel;
    [SerializeField] private AirParkHangar airPark;

    [SerializeField] private GameObject Button1Background;
    [SerializeField] private GameObject Button2Background;
    [SerializeField] private GameObject Button3Background;
    [SerializeField] public ProgressBar progressBar;



    private Color[] colors;

    private Texture[] textures;

    private AudioSource paintSource;

    public static Color selectColor;
    public static Sprite selectSprite;
    public static bool IsTexturing;

    private PaintAir air;

    private Action clickCallback;
    private Action<int> colorSetting;


    private void Start()
    {
        colors = new Color[3];
        textures = new Texture[3];
        selectColor = Color.blue;
    }
    

    public void EnableMenuColors(PaintAir air, int indexColor0, int indexColor1, int indexColor2,AudioSource paintSource, Action clickCallback = null)
    {
        if (clickCallback == null) clickCallback = delegate { };

        menuColorPanel.SetActive(true);
        menuColorPanel.GetComponent<Animation>().Play("Up");
        colors[0] = airPark.colors[indexColor0];
        colors[1]  = airPark.colors[indexColor1];
        colors[2] = airPark.colors[indexColor2];
        Button1Background.GetComponent<Image>().color = colors[0];
        Button2Background.GetComponent<Image>().color = colors[1];
        Button3Background.GetComponent<Image>().color = colors[2];

        this.paintSource = paintSource;
        this.air = air;

        IsTexturing = false; // Для балончика в классе Tutorial
        progressBar.enabled = true;
        progressBar.paintAir = air;
        this.clickCallback = clickCallback;
        menuColorPanel.GetComponent<Animation>().Play("Up");

        this.colorSetting = SetColor;
        void SetColor(int indexColor)
        {
            selectColor = colors[indexColor];
            air.EnableScript(colors[indexColor], paintSource);
        }
    }
    public void EnableMenuTextures(PaintAir air, int indexTexture0, int indexTexture1, int indexTexture2, AudioSource paintSource, Action clickCallback = null)
    {
        if (clickCallback == null) clickCallback = delegate { };

        menuColorPanel.SetActive(true);
        menuColorPanel.GetComponent<Animation>().Play("Up");
        textures[0] = airPark.textures[indexTexture0];
        textures[1] = airPark.textures[indexTexture1];
        textures[2] = airPark.textures[indexTexture2];
        Button1Background.GetComponent<Image>().sprite = airPark.textureAvatars[indexTexture0];
        Button2Background.GetComponent<Image>().sprite = airPark.textureAvatars[indexTexture1];
        Button3Background.GetComponent<Image>().sprite = airPark.textureAvatars[indexTexture2];

        this.paintSource = paintSource;
        this.air = air;

        IsTexturing = true; // Для балончика в классе Tutorial
        progressBar.enabled = true;
        progressBar.paintAir = air;
        this.clickCallback = clickCallback;
        menuColorPanel.GetComponent<Animation>().Play("Up");

        this.colorSetting = SetTexture;

        void SetTexture(int indexColor)
        {
            selectSprite = airPark.textureAvatars[indexColor];
            air.EnableScript(textures[indexColor], paintSource);
        }
    }

    public void DisableMenuColors()
    {
        menuColorPanel.GetComponent<Animation>().Play("Down");
        StartCoroutine(delay());

        IEnumerator delay()
        {
            yield return new WaitForSeconds(1f);
            menuColorPanel.SetActive(false);
        }
    }

    public void ClickButton1()
    {
        colorSetting(0);
        progressBar.EnableScript();
        clickCallback();
        DisableMenuColors();
    }
    public void ClickButton2()
    {
        colorSetting(1);
        progressBar.EnableScript();
        clickCallback();
        DisableMenuColors();
    }
    public void ClickButton3()
    {
        colorSetting(2);
        progressBar.EnableScript();
        clickCallback();
        DisableMenuColors();

    }


}
[Serializable]
public class ColorClickButton : UnityEvent<Color, bool>
{

}
