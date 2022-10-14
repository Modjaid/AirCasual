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
public class MenuDecalUI : MonoBehaviour
{

    [SerializeField] private GameObject menuDecalPanel;
    [SerializeField] private AirParkHangar airPark;

    [SerializeField] private GameObject Button1Background;
    [SerializeField] private GameObject Button2Background;
    [SerializeField] private GameObject Button3Background;


    private Texture[] textures;
    private AudioSource decalSound;

    private PaintDecalAir air;
    private void Start()
    {
        textures = new Texture[3];
    }
    

    public void EnableMenuDecals(PaintDecalAir air, int indexColor0, int indexColor1, int indexColor2, AudioSource decalSound)
    {
        menuDecalPanel.SetActive(true);
        menuDecalPanel.GetComponent<Animation>().Play("Up");

        textures[0] = airPark.decals[indexColor0];
        textures[1]  = airPark.decals[indexColor1];
        textures[2] = airPark.decals[indexColor2];
        Button1Background.GetComponent<Image>().sprite = airPark.decalAvatars[indexColor0];
        Button2Background.GetComponent<Image>().sprite = airPark.decalAvatars[indexColor1];
        Button3Background.GetComponent<Image>().sprite = airPark.decalAvatars[indexColor2];


        this.air = air;
        this.air.EnableDecal(textures[0],decalSound);
        menuDecalPanel.GetComponent<Animation>().Play("Up");
    }

    public void DisableMenuDecals()
    {
        menuDecalPanel.GetComponent<Animation>().Play("Down");
        StartCoroutine(delay());

        IEnumerator delay()
        {
            yield return new WaitForSeconds(1f);
            menuDecalPanel.SetActive(false);
        }
    }

    public void ClickButton1()
    {
        air.SetDecal(textures[0]);
    }
    public void ClickButton2()
    {
        air.SetDecal(textures[1]);
    }
    public void ClickButton3()
    {
        air.SetDecal(textures[2]);

    }
    public void ClickNextButton()
    {
        air.ClickFinishDecal();
    }


}
