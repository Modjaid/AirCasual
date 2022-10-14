using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class EngineMenuUI : MonoBehaviour
{
    public UnityEvent FinishMechanic;

    private Action<int> clickHandler;

    [SerializeField] private GameObject equipMenuPanel;
    [SerializeField] private GameObject Button1Background;
    [SerializeField] private GameObject Button2Background;
    [SerializeField] private GameObject Button3Background;

    [SerializeField] private AirParkHangar airPark;

    private GameObject[] engines;

    private EquipEngine engineAir;

    private Color engineNewColor;

    private Material standardMaterial;
    private Texture newTexture;

    private void Start()
    {
        engines = new GameObject[3];
    }

    public void EnableEquipEngine(EquipEngine air, AirType airType, int indexEngine0, int indexEngine1, int indexEngine2)
    {
        equipMenuPanel.SetActive(true);
        equipMenuPanel.GetComponent<Animation>().Play("Up");

        var accessEngine = airPark.engines[(int)airType].details;
        var engineAvatars = airPark.engines[(int)airType].avatars;

        Button1Background.GetComponent<Image>().sprite = engineAvatars[indexEngine0];
        Button2Background.GetComponent<Image>().sprite = engineAvatars[indexEngine1];
        Button3Background.GetComponent<Image>().sprite = engineAvatars[indexEngine2];

        engines[0] = accessEngine[indexEngine0];  // weapons на деле engine просто чтобы не создавать лишнюю переменную
        engines[1] = accessEngine[indexEngine1];  // weapons на деле engine просто чтобы не создавать лишнюю переменную
        engines[2] = accessEngine[indexEngine2];  // weapons на деле engine просто чтобы не создавать лишнюю переменную

        engineAir = air;
        equipMenuPanel.GetComponent<Animation>().Play("Up");
        clickHandler = (index) => engineAir.SetCurrentEngine(engines[index]);
    }

    public void EnableEquipEngine(EquipEngine air, AirType airType, int indexEngine0, int indexEngine1, int indexEngine2, Color colorEngine, Material stdMat)
    {
        equipMenuPanel.SetActive(true);
        equipMenuPanel.GetComponent<Animation>().Play("Up");

        var accessEngine = airPark.engines[(int)airType].details;
        var engineAvatars = airPark.engines[(int)airType].avatars;

        Button1Background.GetComponent<Image>().sprite = engineAvatars[indexEngine0];
        Button2Background.GetComponent<Image>().sprite = engineAvatars[indexEngine1];
        Button3Background.GetComponent<Image>().sprite = engineAvatars[indexEngine2];



        engines[0] = accessEngine[indexEngine0];  
        engines[1] = accessEngine[indexEngine1];  
        engines[2] = accessEngine[indexEngine2];  

        engineAir = air;
        this.standardMaterial = stdMat;
        equipMenuPanel.GetComponent<Animation>().Play("Up");
        engineNewColor = colorEngine;


        clickHandler = OnColorHandler;
    }
    public void EnableEquipEngine(EquipEngine air, AirType airType, int indexEngine0, int indexEngine1, int indexEngine2, Texture textureEngine, Material stdMat)
    {
        equipMenuPanel.SetActive(true);
        equipMenuPanel.GetComponent<Animation>().Play("Up");

        var accessEngine = airPark.engines[(int)airType].details;
        var engineAvatars = airPark.engines[(int)airType].avatars;

        Button1Background.GetComponent<Image>().sprite = engineAvatars[indexEngine0];
        Button2Background.GetComponent<Image>().sprite = engineAvatars[indexEngine1];
        Button3Background.GetComponent<Image>().sprite = engineAvatars[indexEngine2];



        engines[0] = accessEngine[indexEngine0];
        engines[1] = accessEngine[indexEngine1];
        engines[2] = accessEngine[indexEngine2];

        engineAir = air;
        this.standardMaterial = stdMat;
        equipMenuPanel.GetComponent<Animation>().Play("Up");

        newTexture = textureEngine;

        clickHandler = OnTextureHandler;
    }


    public void DisableMenuEngine()
    {
        equipMenuPanel.GetComponent<Animation>().Play("Down");
        StartCoroutine(delay());

        IEnumerator delay()
        {
            yield return new WaitForSeconds(1f);
            equipMenuPanel.SetActive(false);
        }
    }

    public void ClickEngineButton1()
    {
        clickHandler(0);
        FinishMechanic?.Invoke();
    }
    public void ClickEngineButton2()
    {
        clickHandler(1);
        FinishMechanic?.Invoke();
    }
    public void ClickEngineButton3()
    {
        clickHandler(2);
        FinishMechanic?.Invoke();
    }


    private void OnColorHandler(int index)
    {
        engineAir.SetCurrentEngine(engines[index], engineNewColor, standardMaterial);
    }
    private void OnTextureHandler(int index)
    {
        engineAir.SetCurrentEngine(engines[index], newTexture, standardMaterial);
    }
}
