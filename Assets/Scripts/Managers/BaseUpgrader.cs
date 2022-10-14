using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseUpgrader : MonoBehaviour
{
    [Header("Upgrades")]
    [SerializeField] private Section[] buildings;
    [SerializeField] private Section[] technics;
    [SerializeField] private int[] earnings;

    [Space(5)]
    [Header("UI")]
    [SerializeField] private Text moneyCountText;
    [SerializeField] private Text buildingCostText;
    [SerializeField] private Text technicCostText;
    [SerializeField] private Text earningCostText;
    [SerializeField] private Button buidlingButton;
    [SerializeField] private Button technicButton;
    [SerializeField] private Button earningButton;

    [Space(5)]
    [Header("Animations")]
    [SerializeField] private Animation GoToMapButton;
    [SerializeField] private Animation SkillPanel;

    [Space(5)]
    [Header("Camera")]
    [SerializeField] private Transform cameraStartPos;

    [Space(5)]
    [Header("Base for Rotation")]
    [SerializeField] private Transform baseTransform;
    private Coroutine rotationCoroutine;
    private Quaternion startBaseRotation;


    private int moneyCount;
    private int buildingIndex;
    private int technicIndex;
    private int earningIndex;

    [Header("TEST")]
    public int moneyCount_TEST;
    public int buildingIndex_TEST;
    public int technicIndex_TEST;
    public int earningIndex_TEST;

    public int MoneyCount
    {
        get
        {
            return PlayerPrefs.GetInt("MoneyCount", moneyCount_TEST);
        }
        set
        {
            //  GetComponent<AnalyticSDK>().OnLvlEnded_FB(value);
            //  GetComponent<AnalyticSDK>().OnLvlEnded_GA(value);
            moneyCount = value;
            PlayerPrefs.SetInt("MoneyCount", moneyCount);
        }
    }
    public int BuildingIndex
    {
        get
        {
            return PlayerPrefs.GetInt("buildingIndex", -1); ;
        }
        set
        {
            //  GetComponent<AnalyticSDK>().OnLvlEnded_FB(value);
            //  GetComponent<AnalyticSDK>().OnLvlEnded_GA(value);


            buildingIndex = value;
            if (buildingIndex > buildings.Length - 2)
            {
                buidlingButton.interactable = false;
                buildingIndex = buildings.Length - 1;
            }

            PlayerPrefs.SetInt("buildingIndex", buildingIndex);
        }
    }
    public int TechnicIndex
    {
        get
        {
            return PlayerPrefs.GetInt("technicIndex", -1);
        }
        set
        {
            //  GetComponent<AnalyticSDK>().OnLvlEnded_FB(value);
            //  GetComponent<AnalyticSDK>().OnLvlEnded_GA(value);
            technicIndex = value;
            if (technicIndex > technics.Length - 2)
            {
                technicButton.interactable = false;
                technicIndex = technics.Length - 1;
            }
            PlayerPrefs.SetInt("technicIndex", technicIndex);
        }
    }
    public int EarningIndex
    {
        get
        {
            return PlayerPrefs.GetInt("earningIndex", -1);
        }
        set
        {
            //  GetComponent<AnalyticSDK>().OnLvlEnded_FB(value);
            //  GetComponent<AnalyticSDK>().OnLvlEnded_GA(value);
            earningIndex = value;
            if (earningIndex > earnings.Length - 2)
            {
                earningButton.interactable = false;
                earningIndex = earnings.Length - 1;
            }
            PlayerPrefs.SetInt("earningIndex", earningIndex);
        }
    }


    private MainCamera camera;


    void Start()
    {
        camera = Camera.main.GetComponent<MainCamera>();
        startBaseRotation = baseTransform.rotation;
    }


    public void StartBaseRotation()
    {
        rotationCoroutine = StartCoroutine(Rotation());

        IEnumerator Rotation()
        {
            yield return new WaitForSeconds(2.5f);
            while (true)
            {
                baseTransform.Rotate(new Vector3(0, 6, 0) * Time.deltaTime);
                yield return null;
            }
        }
    }
    public void StopBaseRotation()
    {
        StopCoroutine(rotationCoroutine);

        IEnumerator ComeBackBase()
        {
            while (true)
            {
                baseTransform.rotation = Quaternion.Lerp(baseTransform.rotation, startBaseRotation, 5 * Time.deltaTime);
                yield return null;
            }
        }
    }

    public void EnableBase(ref int moneyBonus)
    {
          moneyCount    = MoneyCount + moneyBonus;
          buildingIndex = BuildingIndex;
          technicIndex  = TechnicIndex;
          earningIndex  = EarningIndex;

        MoneyCount = moneyCount;
        BuildingIndex = buildingIndex;
        TechnicIndex = technicIndex;
        EarningIndex = earningIndex;
        moneyBonus = 0;
        // TEST////////////////////////////
        //   MoneyCount = moneyCount_TEST;
        //   TechnicIndex = technicIndex_TEST;
        //   BuildingIndex = buildingIndex_TEST;
        //   EarningIndex = earningIndex_TEST;
        //////////////////////////////////////


        moneyCountText.text = moneyCount.ToString();
        if (buildingIndex < buildings.Length - 1)
        {
            buildingCostText.text = buildings[buildingIndex + 1].cost.ToString();
        }
        if (technicIndex < technics.Length - 1)
        {
            technicCostText.text = technics[technicIndex + 1].cost.ToString();
        }
        if (earningIndex < earnings.Length - 1)
        {
            earningCostText.text = earnings[earningIndex + 1].ToString();
        }

        InitBase();
    }


    public void BuildingPay()
    {
        bool IsNotPermission = MoneyDecrease(buildings[buildingIndex + 1].cost);

        if (IsNotPermission)
        {
            buidlingButton.GetComponent<Animation>().Play("Red");
            return;
        }

        StartCoroutine(NextUpgrading(buildingIndex + 1,buildings,"Model"));

        BuildingIndex++;
        moneyCountText.text = moneyCount.ToString();
        if (buildingIndex < buildings.Length - 1)
        {
            buildingCostText.text = buildings[buildingIndex + 1].cost.ToString();
        }

    }
    public void TechnicPay()
    {
        bool IsNotPermission = MoneyDecrease(technics[technicIndex + 1].cost);

        if (IsNotPermission)
        {
            technicButton.GetComponent<Animation>().Play("Red");
            return;
        }

        camera.LookAtOn(technics[technicIndex + 1].model.transform);

        StartCoroutine(NextUpgrading(technicIndex + 1,technics,"Scale"));

        TechnicIndex++;
        moneyCountText.text = moneyCount.ToString();
        if (technicIndex < technics.Length - 1)
        {
            technicCostText.text = technics[technicIndex + 1].cost.ToString();
        }

    }
    public void EarningPay()
    {
        bool IsNotPermission = MoneyDecrease(earnings[earningIndex + 1]);

        if (IsNotPermission)
        {
            earningButton.GetComponent<Animation>().Play("Red");
            return;
        }

        EarningIndex++;
        moneyCountText.text = moneyCount.ToString();
        if (earningIndex < earnings.Length - 1)
        {
            earningCostText.text = earnings[earningIndex + 1].ToString();
        }
    }

    private bool MoneyDecrease(int decrease)
    {
        if (moneyCount >= decrease)
        {
            MoneyCount -= decrease;
            return false;
        }
        return true;
    }

    private void InitBase()
    {
        foreach(Section part in buildings)
        {
            part.model.SetActive(false);
        }

        foreach(Section part in technics)
        {
            part.model.SetActive(false);
        }

        for(int i = 0; i < buildingIndex + 1; i++)
        {
            buildings[i].model.SetActive(true);
            int changeIndex = buildings[i].changeModelIndex;
            if(changeIndex > -1)
            {
                buildings[changeIndex].model.SetActive(false);
            }
        }


        for (int i = 0; i < technicIndex + 1; i++)
        {
            technics[i].model.SetActive(true);
            int changeIndex = technics[i].changeModelIndex;
            if (changeIndex > -1)
            {
                technics[changeIndex].model.SetActive(false);
            }
        }


    }

    private IEnumerator NextUpgrading(int indexModel ,Section[] modelArray, string anim)
    {
        GoToMapButton.Play("Down");
        SkillPanel.Play("Down");
        camera.SetCameraParking(modelArray[indexModel].cameraPlace, 0.01f);
        StopBaseRotation();

        yield return new WaitForSeconds(2f);
        int changeModelIndex = modelArray[indexModel].changeModelIndex;

        if (changeModelIndex > -1)
        {
            modelArray[changeModelIndex].model.GetComponent<Animation>().Play(anim + "Down");

            yield return new WaitForSeconds(0.5f);
            modelArray[changeModelIndex].model.SetActive(false);
        }    


        modelArray[indexModel].model.SetActive(true);
        modelArray[indexModel].model.GetComponent<Animation>().Play(anim + "Up");

        yield return new WaitForSeconds(1f);
        StartBaseRotation();
        camera.SetCameraParking(cameraStartPos, 0.01f);
        GoToMapButton.Play("Up");
        SkillPanel.Play("Up");
        yield return new WaitForSeconds(1f);
        camera.LookAtOff();
    }

    public static int GetEarningIndex()
    {
        return PlayerPrefs.GetInt("earningIndex", -1);
        
    }
    [Serializable]
    public struct Section
    {
        public string name;
        [Header("Сам объект")]
        public GameObject model;
        [Header("для позиционирования камеры")]
        public Transform cameraPlace;
        [Header("Цена для активации")]
        public int cost;
        [Header("Индекс заменяемой модели")]
        [Tooltip("индекс модели в массиве которая будет отключена (Для замены одной модели на другую если == -1 то замена не нужна)")]
        public int changeModelIndex;
    }

    #region UI
    public void GoToBase()
    {
        GoToMapButton.Play("Up");
        SkillPanel.Play("Up");
        StartBaseRotation();
    }
    public void HideUI()
    {
        SkillPanel.Play("Down");
        GoToMapButton.Play("Down");
    }
    #endregion
}
