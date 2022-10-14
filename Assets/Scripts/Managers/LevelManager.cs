using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// СИНГЛТОН Работающий между сценами, контролирующий каждый переход между механиками, шагами, и сценами
/// Хранит поведение каждого левела через корутины в коллекции levels
/// </summary>
public class LevelManager : MonoBehaviour
{
    [SerializeField] private AnalyticSDK SDK;
    [SerializeField] private Animation FinishPanel;
    [SerializeField] private Animation RestartPanel;
    [Header("Каждый новый индекс перезаписывает игру")]
    [SerializeField] private int saveIndex;

    [Header("Денежная прибыль каждый левел")]
    [SerializeField] private int MoneyBonus;
    [SerializeField] private TextMeshProUGUI MoneyBonusText;
    [Header("Добавка к прибыли в зависимости от уровня индекса в BaseUpgrader")]
    [Tooltip("Длина массива обязательна должна быть равна длине == (int[]) BaseUpgrader.earnings.Length")]
    [SerializeField] private int[] addMoneyBonuses;
    private int moneyBonus;

    public static LevelManager instance;
    private WheelPump wheelPump;
    private Scene currentScene;


    public MainCamera MainCamera
    {
        get
        {
            return Camera.main.GetComponent<MainCamera>();
        }
    }

    /// <summary>
    /// Работает через Find лучше на каждом уровне записывать в какую нибудь переменную
    /// </summary>
    private AirPark AirPark
    {
        get
        {
            return GameObject.Find("AirPark").GetComponent<AirPark>();
        }
    }
    private Map Map
    {
        get
        {
            return GameObject.Find("Maps").GetComponent<Map>();
        }
    }

    private bool Next;
    [HideInInspector] public bool NEXTLEVEL;
    public int CompletedLevels
    {
        get
        {
            return PlayerPrefs.GetInt("CompletedLevels", 0);
        }
        set
        {
            PlayerPrefs.SetInt("CompletedLevels", value);
        }
    }

    public int CurrentLvl
    {
        get
        {
            return PlayerPrefs.GetInt("CurrentLevel", 0);
        }
        set 
        {
            if (value > levels.Count - 1)
            {
                value = 1;
            }
            PlayerPrefs.SetInt("CurrentLevel", value);
        }
    }
    public delegate IEnumerator LvlDelegate();
    public List<LvlDelegate> levels;

    /// <summary>
    /// 
    /// 
    /// 
    /// 
    /// 
    /// 
    /// 
    /// 
    /// 
    /// 
    /// 
    /// </summary>
 void Awake()
    {
        NEXTLEVEL = false;
        levels = new List<LvlDelegate>();
        // !!!!!!!!!!!!!!!!!!!!! Создание уровней !!!!!!!!!!!!!!!!!!!!!!!!!

      //  levels.Add(TestBase);
         levels.Add(NullLevel); 
         levels.Add(AC130_Weapon_DesertPlane);
         levels.Add(UN60_FireCover);
         levels.Add(TexturePaint_Weapon_DesertWall);
         levels.Add(Weapon_DesertHelicopter);
         levels.Add(Weapon_EarthBattle);
         levels.Add(F16_TexturePaint_Weapon_Catapult_StartEngine_Bomb);
         levels.Add(TexturePaint_Weapon_BattleAir);
         levels.Add(AC130_Weapon_DesertPlane);
         levels.Add(UN60_FireCover);
         levels.Add(TexturePaint_Weapon_DesertWall);
         levels.Add(Weapon_DesertHelicopter);
         levels.Add(Weapon_EarthBattle);
         levels.Add(F16_TexturePaint_Weapon_Catapult_StartEngine_Bomb);
         levels.Add(UN60_TexturePaint_Weapon_FireCover);
         levels.Add(TexturePaint_Weapon_DesertWall);
         levels.Add(F16_Catapult_StartEngine_AirBattle);
         levels.Add(AC130_EquipEngine_BladeEngine_DesertPlane);
         levels.Add(F15_TexturePaint_Weapon_Bomb);
         levels.Add(F15_BattleAir); 
         levels.Add(AN64_Weapon_DesertWall);
         levels.Add(UN60_TexturePaint_Weapon_FireCover);
         levels.Add(UN60_TexturePaint_Weapon_DesertHelicopter);
         levels.Add(F16_Engine_Weapon_Bomb);
         levels.Add(F15_Weapon_BattleAir);
         levels.Add(TexturePaint_Weapon_DesertWall);
         levels.Add(Weapon_DesertHelicopter);
         levels.Add(Weapon_EarthBattle);


        if (!instance)
        {
            instance = this;
            moneyBonus = 0;
            DontDestroyOnLoad(transform.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator F15_Weapon_BattleAir()
    {
        #region Map
        SceneManager.LoadScene("Map");
        yield return new WaitUntil(() => Next == true);
        Next = false;
        var map = Map;
        var baseUpgrader = Map.gameObject.GetComponent<BaseUpgrader>();
        map.StartMap(CurrentLvl);
        baseUpgrader.EnableBase(ref moneyBonus);

        map.FinishMap.AddListener(() => Next = true);
        yield return new WaitUntil(() => Next == true);
        Next = false;
        #endregion


        #region Инициализация
        SceneManager.LoadScene("Hangar");
        yield return new WaitUntil(() => Next == true);
        Next = false;
        AirParkHangar hangar = AirPark as AirParkHangar;
        var air = hangar.InstanceAir(AirType.F15);
        MainCamera.OnFinishParking.AddListener(() => Next = true);
        MainCamera.transform.position = hangar.CameraPos1.position;
        MainCamera.transform.rotation = hangar.CameraPos1.rotation;
        #endregion

        #region МЕХАНИКА СМЕНЫ ВООРУЖЕНИЯ
        var airWeapon = air.GetComponent<EquipWeapon>();
        var equipMenu = hangar.canvas.GetComponent<EquipMenuUI>();
        var mainMenu = hangar.canvas.GetComponent<MainMenuUI>();
        var accessWeapons = hangar.weapons[(int)AirType.F15].details;

        MainCamera.SetCameraParking(hangar.CameraPos2,0.01f);
        yield return new WaitUntil(() => Next == true);
        Next = false;


        

        equipMenu.EnableEquipMenu(airWeapon,AirType.F15,0,1,2);
        equipMenu.FinishMechanic.AddListener(() => Next = true);
        yield return new WaitUntil(() => Next == true);
        equipMenu.FinishMechanic.RemoveAllListeners();
        Next = false;
        mainMenu.EnableNextButton("BattleAir");
        mainMenu.OnButtonPressed += () => Next = true;
        mainMenu.OnButtonPressed += () => equipMenu.DisableEquipMenu();

        yield return new WaitUntil(() => Next == true);
        Next = false;
        air.transform.parent = this.transform;

        #endregion

        #region BattleAIR
        yield return new WaitUntil(() => Next == true);
        Next = false;
        airWeapon.DeactivationWheel();
        var battleAir = AirPark as AirParkBattleAir;

        battleAir.EnableScript(air, 2);
        
      //  MainCamera.SetPos(battleAir.CameraPos2);

        battleAir.tutorial.BattleAirOn();
        yield return new WaitUntil(() => Input.GetMouseButton(0));
        battleAir.enabled = true;

        yield return new WaitForSeconds(0.5f);
        AirParkBattleAir.Victory.AddListener(() => Next = true);

        yield return new WaitUntil(() => Next == true);
        Next = false;
        battleAir.tutorial.BattleAirOff();

        FinishPanelUp();
        #endregion
    }

    private IEnumerator NullLevel()
    {
        #region Инициализация
        SceneManager.LoadScene("Hangar");
        yield return new WaitUntil(() => Next == true);
        Next = false;
        AirParkHangar hangar = AirPark as AirParkHangar;
        var air = hangar.InstanceAir(AirType.F15);
        MainCamera.OnFinishParking.AddListener(() => Next = true);
        MainCamera.transform.position = hangar.CameraPos1.position;
        MainCamera.transform.rotation = hangar.CameraPos1.rotation;
        #endregion

        #region МЕХАНИКА СМЕНЫ ВООРУЖЕНИЯ
        var airWeapon = air.GetComponent<EquipWeapon>();
        var equipMenu = hangar.canvas.GetComponent<EquipMenuUI>();
        var mainMenu = hangar.canvas.GetComponent<MainMenuUI>();
        var accessWeapons = hangar.weapons[(int)AirType.F15].details;

        MainCamera.SetCameraParking(hangar.CameraPos2, 0.01f);
        yield return new WaitUntil(() => Next == true);
        Next = false;




        equipMenu.EnableEquipMenu(airWeapon, AirType.F15, 0, 1, 2);
        equipMenu.FinishMechanic.AddListener(() => Next = true);
        yield return new WaitUntil(() => Next == true);
        equipMenu.FinishMechanic.RemoveAllListeners();
        Next = false;
        mainMenu.EnableNextButton("BattleAir");
      //  mainMenu.OnButtonPressed += () => Next = true;
        mainMenu.OnButtonPressed += () => equipMenu.DisableEquipMenu();

        //yield return new WaitUntil(() => Next == true);
       // Next = false;
        air.transform.parent = this.transform;
        yield return new WaitUntil(() => Next == true);
        Next = false;
        #endregion


        #region BattleAIR
        var battleAir = AirPark as AirParkBattleAir;

        battleAir.EnableScript(air, 2);

        //  MainCamera.SetPos(battleAir.CameraPos2);

        battleAir.tutorial.BattleAirOn();
        yield return new WaitUntil(() => Input.GetMouseButton(0));
        battleAir.enabled = true;

        yield return new WaitForSeconds(0.5f);
        AirParkBattleAir.Victory.AddListener(() => Next = true);

        yield return new WaitUntil(() => Next == true);
        Next = false;
        battleAir.tutorial.BattleAirOff();

        FinishPanelUp();
        #endregion
    }

    private IEnumerator F15_BattleAir()
    {
        #region Map
        SceneManager.LoadScene("Map");
        yield return new WaitUntil(() => Next == true);
        Next = false;
        var map = Map;
        var baseUpgrader = Map.gameObject.GetComponent<BaseUpgrader>();
        map.StartMap(CurrentLvl);
        baseUpgrader.EnableBase(ref moneyBonus);

        map.FinishMap.AddListener(() => Next = true);
        yield return new WaitUntil(() => Next == true);
        Next = false;
        #endregion

        #region BattleAIR
        SceneManager.LoadScene("BattleAir");
        yield return new WaitUntil(() => Next == true);
        Next = false;
        var battleAir = AirPark as AirParkBattleAir;
        var air = battleAir.InstanceAir(AirType.F15);
        battleAir.EnableScript(air, 4);

        battleAir.tutorial.BattleAirOn();
        yield return new WaitUntil(() => Input.GetMouseButton(0));
        battleAir.enabled = true;

        yield return new WaitForSeconds(0.5f);
        AirParkBattleAir.Victory.AddListener(() => Next = true);

        yield return new WaitUntil(() => Next == true);
        Next = false;
        battleAir.tutorial.BattleAirOff();

        FinishPanelUp();
        #endregion
    }

    private IEnumerator TexturePaint_Weapon_BattleAir()
    {
        #region Map
        SceneManager.LoadScene("Map");
        yield return new WaitUntil(() => Next == true);
        Next = false;
        var map = Map;
        var baseUpgrader = Map.gameObject.GetComponent<BaseUpgrader>();
        map.StartMap(CurrentLvl);
        baseUpgrader.EnableBase(ref moneyBonus);

        map.FinishMap.AddListener(() => Next = true);
        yield return new WaitUntil(() => Next == true);
        Next = false;
        #endregion


        #region Инициализация
        SceneManager.LoadScene("Hangar");
        yield return new WaitUntil(() => Next == true);
        Next = false;
        AirParkHangar hangar = AirPark as AirParkHangar;
        var air = hangar.InstanceAir(AirType.F15);
        MainCamera.OnFinishParking.AddListener(() => Next = true);
        MainCamera.transform.position = hangar.CameraPos1.position;
        MainCamera.transform.rotation = hangar.CameraPos1.rotation;
        #endregion

        #region МЕХАНИКА ПОКРАСКИ
        var paintAir = air.GetComponent<PaintAir>();
        var colorMenu = hangar.canvas.GetComponent<MenuColorUI>();
        var accessColors = hangar.colors;

        MainCamera.SetCameraParking(hangar.CameraPos3, 0.1f);
        yield return new WaitUntil(() => Next == true);
        Next = false;

        colorMenu.EnableMenuTextures(paintAir, 0, 1, 2, hangar.paintSound);
        paintAir.OnFinish3DPainting.AddListener(() => Next = true);

        yield return new WaitUntil(() => Next == true);
        Next = false;
        hangar.canvas.GetComponent<Tutorial>().PaintFingerOff();
        paintAir.FinishTexture(hangar.standardMaterial);
        yield return new WaitForSeconds(2f);
        #endregion

        #region МЕХАНИКА СМЕНЫ ВООРУЖЕНИЯ
        var airWeapon = air.GetComponent<EquipWeapon>();
        var equipMenu = hangar.canvas.GetComponent<EquipMenuUI>();
        var mainMenu = hangar.canvas.GetComponent<MainMenuUI>();
        var accessWeapons = hangar.weapons[(int)AirType.F15].details;

        MainCamera.SetCameraParking(hangar.CameraPos2, 0.01f);
        yield return new WaitUntil(() => Next == true);
        Next = false;




        equipMenu.EnableEquipMenu(airWeapon, AirType.F15, 0, 1, 2);
        equipMenu.FinishMechanic.AddListener(() => Next = true);
        yield return new WaitUntil(() => Next == true);
        equipMenu.FinishMechanic.RemoveAllListeners();
        Next = false;
        mainMenu.EnableNextButton("BattleAir");
        mainMenu.OnButtonPressed += () => Next = true;
        mainMenu.OnButtonPressed += () => equipMenu.DisableEquipMenu();

        yield return new WaitUntil(() => Next == true);
        Next = false;
        air.transform.parent = this.transform;

        #endregion

        #region BattleAir
        yield return new WaitUntil(() => Next == true);
        Next = false;
        airWeapon.DeactivationWheel();
        var battleAir = AirPark as AirParkBattleAir;

        battleAir.EnableScript(air, 2);

        //  MainCamera.SetPos(battleAir.CameraPos2);

        battleAir.tutorial.BattleAirOn();
        yield return new WaitUntil(() => Input.GetMouseButton(0));
        battleAir.enabled = true;

        yield return new WaitForSeconds(0.5f);
        AirParkBattleAir.Victory.AddListener(() => Next = true);

        yield return new WaitUntil(() => Next == true);
        Next = false;
        battleAir.tutorial.BattleAirOff();

        FinishPanelUp();
        #endregion
    }

    private IEnumerator AC130_Weapon_DesertPlane()
    {
        #region Map
        SceneManager.LoadScene("Map");
        yield return new WaitUntil(() => Next == true);
        Next = false;
        var map = Map;
        var baseUpgrader = Map.gameObject.GetComponent<BaseUpgrader>();
        map.StartMap(CurrentLvl);
        baseUpgrader.EnableBase(ref moneyBonus);

        map.FinishMap.AddListener(() => Next = true);
        yield return new WaitUntil(() => Next == true);
        Next = false;
        #endregion

        #region Инициализация
        SceneManager.LoadScene("Hangar");
        yield return new WaitUntil(() => Next == true);
        Next = false;
        AirParkHangar hangar = AirPark as AirParkHangar;
        var air = hangar.InstanceAir(AirType.AC130);
        var anyEngine = hangar.engines[(int)AirType.AC130].details[0];
        air.GetComponent<EquipEngine>().SetCurrentEngine(anyEngine);
        MainCamera.OnFinishParking.AddListener(() => Next = true);
        MainCamera.transform.position = hangar.CameraPos1.position;
        MainCamera.transform.rotation = hangar.CameraPos1.rotation;
        #endregion

        #region МЕХАНИКА СМЕНЫ ВООРУЖЕНИЯ
        var airWeapon = air.GetComponent<EquipWeapon>();
        var equipMenu = hangar.canvas.GetComponent<EquipMenuUI>();
        var mainMenu = hangar.canvas.GetComponent<MainMenuUI>();

        MainCamera.SetCameraParking(hangar.CameraPos2, 0.1f);
        yield return new WaitUntil(() => Next == true);
        Next = false;


        MainCamera.SetCameraParking(airWeapon.weaponCameraPos, 0.01f);
        yield return new WaitUntil(() => Next == true);
        Next = false;

        equipMenu.EnableEquipMenu(airWeapon, AirType.AC130, 0, 1, 2);
        equipMenu.FinishMechanic.AddListener(() => Next = true);
        yield return new WaitUntil(() => Next == true);
        equipMenu.FinishMechanic.RemoveAllListeners();
        Next = false;


      //  mainMenu.OnButtonPressed += () => Next = true;
        mainMenu.OnButtonPressed += () => equipMenu.DisableEquipMenu();
        mainMenu.EnableNextButton("Desert");
        air.transform.parent = this.transform;

        #endregion

        #region BATTLE DESERT (PLANE)
        yield return new WaitUntil(() => Next == true);
        Next = false;


        DesertAirPark desertPark = AirPark as DesertAirPark;
        AirPlatform platform = desertPark.planePlatform.GetComponent<AirPlatform>();
        desertPark.StartTrack(CurrentLvl, air, true,30);
        restartButtonHandler = desertPark.Restart;
        MainCamera.OnFinishParking.AddListener(() => Next = true);

       // air.GetComponent<EquipEngine>().StartAnimEngine();


        MainCamera.SetPos(platform.CameraPlace);
        MainCamera.transform.parent = platform.CameraPlace;
        desertPark.planePath.speed = 20f;

        air.SetActive(false);
        Instantiate(airWeapon.GetWeapon(),platform.GunPlace);
        platform.enabled = true;

        desertPark.FinishTrack.AddListener((IsVictory) => Next = IsVictory);

        yield return new WaitUntil(() => Next == true);
        Next = false;

        platform.enabled = false;
        FinishPanelUp();
        #endregion
    }

    private IEnumerator F16_TexturePaint_Weapon_Catapult_StartEngine_Bomb()
    {
        #region Map
        SceneManager.LoadScene("Map");
        yield return new WaitUntil(() => Next == true);
        Next = false;
        var map = Map;
        var baseUpgrader = Map.gameObject.GetComponent<BaseUpgrader>();
        map.StartMap(CurrentLvl);
        baseUpgrader.EnableBase(ref moneyBonus);

        map.FinishMap.AddListener(() => Next = true);
        yield return new WaitUntil(() => Next == true);
        Next = false;
        #endregion

        #region Инициализация
        SceneManager.LoadScene("Hangar");
        yield return new WaitUntil(() => Next == true);
        Next = false;

        AirParkHangar hangar = AirPark as AirParkHangar;
        var air = hangar.InstanceAir(AirType.F16);
        MainCamera.OnFinishParking.AddListener(() => Next = true);
        MainCamera.SetPos(hangar.CameraPos1);
        #endregion

        #region МЕХАНИКА ПОКРАСКИ
        var paintAir = air.GetComponent<PaintAir>();
        var colorMenu = hangar.canvas.GetComponent<MenuColorUI>();
        var accessColors = hangar.colors;
        var engineAir = air.GetComponent<EquipEngine>();

        MainCamera.SetCameraParking(hangar.CameraPos3, 0.1f);
        yield return new WaitUntil(() => Next == true);
        Next = false;

        colorMenu.EnableMenuTextures(paintAir, 0, 1, 2,hangar.paintSound);
        paintAir.OnFinish3DPainting.AddListener(() => Next = true);

        yield return new WaitUntil(() => Next == true);
        Next = false;

        hangar.canvas.GetComponent<Tutorial>().PaintFingerOff();

        engineAir.SetCurrentEngine(hangar.engines[(int)AirType.F16].details[1], MenuColorUI.selectSprite.texture,hangar.standardMaterial);
        paintAir.FinishTexture(hangar.standardMaterial);


        yield return new WaitForSeconds(2f);
        #endregion

        #region МЕХАНИКА СМЕНЫ ВООРУЖЕНИЯ
        var airWeapon = air.GetComponent<EquipWeapon>();
        var equipMenu = hangar.canvas.GetComponent<EquipMenuUI>();
        var mainMenu = hangar.canvas.GetComponent<MainMenuUI>();

        MainCamera.SetCameraParking(hangar.CameraPos2, 0.1f);
        yield return new WaitUntil(() => Next == true);
        Next = false;


        equipMenu.EnableEquipMenu(airWeapon, AirType.F15, 3, 4, 5);
        equipMenu.FinishMechanic.AddListener(() => Next = true);
        yield return new WaitUntil(() => Next == true);
        equipMenu.FinishMechanic.RemoveAllListeners();
        Next = false;

        mainMenu.EnableNextButton("AirField");
      //  mainMenu.OnButtonPressed += () => Next = true;
        mainMenu.OnButtonPressed += () => equipMenu.DisableEquipMenu();
        air.transform.parent = this.transform;

        yield return new WaitUntil(() => Next == true);
        Next = false;


        #endregion
        
        #region Инициализация AIRFIELD
        AirFieldLocation airField = AirPark as AirFieldLocation;
        MainCamera.OnFinishParking.AddListener(() => Next = true);
        //   MainCamera.SetPos(airField.camViewPlace_1); Вдруг нужно вернуть чтоб камера стартовала из обзорного вида
        MainCamera.SetPos(airField.camCatapultPlace);
        airField.CurrentAir = air;
        air.transform.parent = airField.getAirPlace(AirType.F16);
        air.transform.localPosition = new Vector3(0, 0, 0);
        air.transform.localRotation = Quaternion.Euler(-90, 0, 0);
        air.transform.localScale = new Vector3(18, 18, 18);

        yield return new WaitForSeconds(1f);
        #endregion

        #region КАТАПУЛЬТА
               #region Вдруг нужно вернуть чтоб камера стартовала из обзорного вида
        //  MainCamera.SetCameraParking(airField.camViewPlace_2, 2f, 4);
        //
        //
        //  yield return new WaitUntil(() => Next == true);
        //  Next = false;
        //
        //  MainCamera.SetCameraParking(airField.camCatapultPlace, 0.1f);
        //
        //  yield return new WaitUntil(() => Next == true);
        //  Next = false;
              #endregion
        var catapult = airField.GetComponent<CatapultController>();


        catapult.EnableScript();
        airField.tutorial.CatapultOn();
      
        yield return new WaitForSeconds(0.4f);
        catapult.EnableScript();
        catapult.FinishGame.AddListener(() => Next = true);
      
        yield return new WaitUntil(() => Next == true);
        airField.tutorial.CatapultOff();

        yield return new WaitForSeconds(1f);
        Next = false;

        MainCamera.SetCameraParking(airField.camViewPlace_2, 0.1f);

        yield return new WaitForSeconds(1.5f);
        Next = false;
        #endregion

        #region ЗАПУСК ДВИГАТЕЛЯ
        var EngineController = air.GetComponent<StartEngine>();
        var tachometr = MainCamera.tachometr;
        var engine = EngineController.enginePlace.GetChild(0).GetComponent<AirEngine>();

        MainCamera.SetCameraParking(EngineController.cameraPlace, 0.1f);

        yield return new WaitUntil(() => Next == true);
        Next = false;

        airField.tutorial.StartEngineOn();

        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
        EngineController.StartScript(MainCamera);
        EngineController.EndMechanicEvent.AddListener(() => Next = true);

        yield return new WaitUntil(() => Next == true);
        Next = false;
        airField.tutorial.StartEngineOff();
        EngineController.EndScript();

        yield return new WaitForSeconds(1f);
        MainCamera.SetCameraParking(airField.camViewPlace_3, 1f);

        yield return new WaitUntil(() => Next == true);
        Next = false;

        MainCamera.LookAtOn(air.transform);
        air.GetComponent<Animation>().Play();
        MainCamera.takeUpSound.Play();
        MainCamera.greenPhaseEngine.Stop();
        yield return new WaitForSeconds(4f);
        air.GetComponent<Animation>().Stop();
        air.transform.parent = this.transform;
        #endregion
        
        #region Bomb
        SceneManager.LoadScene("BattleAir");
        yield return new WaitUntil(() => Next == true);
        Next = false;

        var battleAir = AirPark as AirParkBattleAir;
        air.transform.parent = battleAir.getAirPlace(AirType.F16);
        air.transform.localPosition = new Vector3(0, 0, 0);
        air.transform.localRotation = Quaternion.Euler(0, 0, 0);
        air.transform.localScale = new Vector3(18, 18, 18);
        var airBomb = battleAir.GetComponent<AirBomb>();

        airBomb.EnableScript(CurrentLvl,MainCamera);
        restartButtonHandler = airBomb.Restart;
        airBomb.FinishBomb.AddListener((IsVictory) => Next = IsVictory);
        yield return new WaitUntil(() => Next == true);
        Next = false;
        FinishPanelUp();
        #endregion


    }

    private IEnumerator F16_Engine_Weapon_Bomb()
    {
        #region Map
        SceneManager.LoadScene("Map");
        yield return new WaitUntil(() => Next == true);
        Next = false;
        var map = Map;
        var baseUpgrader = Map.gameObject.GetComponent<BaseUpgrader>();
        map.StartMap(CurrentLvl);
        baseUpgrader.EnableBase(ref moneyBonus);

        map.FinishMap.AddListener(() => Next = true);
        yield return new WaitUntil(() => Next == true);
        Next = false;
        #endregion

        #region Инициализация АВТОПОКРАСКА АВТОДВИГАТЕЛЬ
        SceneManager.LoadScene("Hangar");
        yield return new WaitUntil(() => Next == true);
        Next = false;

        AirParkHangar hangar = AirPark as AirParkHangar;
        var air = hangar.InstanceAir(AirType.F16);
        MainCamera.OnFinishParking.AddListener(() => Next = true);
        MainCamera.SetPos(hangar.CameraPos1);

        var paintAir = air.GetComponent<PaintAir>();
        var colorMenu = hangar.canvas.GetComponent<MenuColorUI>();
        var accessColors = hangar.colors;
        var engineAir = air.GetComponent<EquipEngine>();
        MainCamera.SetCameraParking(hangar.CameraPos2, 0.1f);
        engineAir.SetCurrentEngine(hangar.engines[(int)AirType.F16].details[1], hangar.textures[1], hangar.standardMaterial);
        paintAir.FinishTexture(hangar.standardMaterial, hangar.textures[1]);
        #endregion


        #region МЕХАНИКА СМЕНЫ ВООРУЖЕНИЯ
        var airWeapon = air.GetComponent<EquipWeapon>();
        var equipMenu = hangar.canvas.GetComponent<EquipMenuUI>();
        var mainMenu = hangar.canvas.GetComponent<MainMenuUI>();

        MainCamera.SetCameraParking(hangar.CameraPos2, 0.1f);
        yield return new WaitUntil(() => Next == true);
        Next = false;


        equipMenu.EnableEquipMenu(airWeapon, AirType.F16, 3, 4, 5);
        equipMenu.FinishMechanic.AddListener(() => Next = true);
        yield return new WaitUntil(() => Next == true);
        equipMenu.FinishMechanic.RemoveAllListeners();
        Next = false;

        mainMenu.EnableNextButton("BattleAir");
        //  mainMenu.OnButtonPressed += () => Next = true;
        mainMenu.OnButtonPressed += () => equipMenu.DisableEquipMenu();
        air.transform.parent = this.transform;

        yield return new WaitUntil(() => Next == true);
        Next = false;
        #endregion

        #region Bomb

        var battleAir = AirPark as AirParkBattleAir;
        air.transform.parent = battleAir.getAirPlace(AirType.F16);
        air.transform.localPosition = new Vector3(0, 0, 0);
        air.transform.localRotation = Quaternion.Euler(0, 0, 0);
        air.transform.localScale = new Vector3(18, 18, 18);
        var airBomb = battleAir.GetComponent<AirBomb>();

        airBomb.EnableScript(CurrentLvl, MainCamera);
        restartButtonHandler = airBomb.Restart;
        airBomb.FinishBomb.AddListener((IsVictory) => Next = IsVictory);
        yield return new WaitUntil(() => Next == true);
        Next = false;
        FinishPanelUp();
        #endregion
    }

    private IEnumerator F15_TexturePaint_Weapon_Bomb()
    {
        #region Map
        SceneManager.LoadScene("Map");
        yield return new WaitUntil(() => Next == true);
        Next = false;
        var map = Map;
        var baseUpgrader = Map.gameObject.GetComponent<BaseUpgrader>();
        map.StartMap(CurrentLvl);
        baseUpgrader.EnableBase(ref moneyBonus);

        map.FinishMap.AddListener(() => Next = true);
        yield return new WaitUntil(() => Next == true);
        Next = false;
        #endregion

        #region Инициализация
        SceneManager.LoadScene("Hangar");
        yield return new WaitUntil(() => Next == true);
        Next = false;

        AirParkHangar hangar = AirPark as AirParkHangar;
        var air = hangar.InstanceAir(AirType.F15);
        MainCamera.OnFinishParking.AddListener(() => Next = true);
        MainCamera.SetPos(hangar.CameraPos1);
        #endregion

        #region МЕХАНИКА ПОКРАСКИ
        var paintAir = air.GetComponent<PaintAir>();
        var colorMenu = hangar.canvas.GetComponent<MenuColorUI>();
        var accessColors = hangar.colors;
        var engineAir = air.GetComponent<EquipEngine>();

        MainCamera.SetCameraParking(hangar.CameraPos3, 0.1f);
        yield return new WaitUntil(() => Next == true);
        Next = false;

        colorMenu.EnableMenuTextures(paintAir, 0, 1, 2, hangar.paintSound);
        paintAir.OnFinish3DPainting.AddListener(() => Next = true);

        yield return new WaitUntil(() => Next == true);
        Next = false;

        hangar.canvas.GetComponent<Tutorial>().PaintFingerOff();

        paintAir.FinishTexture(hangar.standardMaterial);


        yield return new WaitForSeconds(2f);
        #endregion

        #region МЕХАНИКА СМЕНЫ ВООРУЖЕНИЯ
        var airWeapon = air.GetComponent<EquipWeapon>();
        var equipMenu = hangar.canvas.GetComponent<EquipMenuUI>();
        var mainMenu = hangar.canvas.GetComponent<MainMenuUI>();

        MainCamera.SetCameraParking(hangar.CameraPos2, 0.1f);
        yield return new WaitUntil(() => Next == true);
        Next = false;


        equipMenu.EnableEquipMenu(airWeapon, AirType.F15, 3, 4, 5);
        equipMenu.FinishMechanic.AddListener(() => Next = true);
        yield return new WaitUntil(() => Next == true);
        equipMenu.FinishMechanic.RemoveAllListeners();
        Next = false;

        mainMenu.EnableNextButton();
        //  mainMenu.OnButtonPressed += () => Next = true;
        mainMenu.OnButtonPressed += () => equipMenu.DisableEquipMenu();
        mainMenu.OnButtonPressed += () => Next = true;
        air.transform.parent = this.transform;

        yield return new WaitUntil(() => Next == true);
        Next = false;
        #endregion

        #region Bomb
        SceneManager.LoadScene("BattleAir");
        yield return new WaitUntil(() => Next == true);
        Next = false;

        var battleAir = AirPark as AirParkBattleAir;
        air.transform.parent = battleAir.getAirPlace(AirType.F16);
        air.transform.localPosition = new Vector3(0, 0, 0);
        air.transform.localRotation = Quaternion.Euler(0, 0, 0);
        air.transform.localScale = new Vector3(18, 18, 18);
        var airBomb = battleAir.GetComponent<AirBomb>();

        airBomb.EnableScript(CurrentLvl, MainCamera);
        restartButtonHandler = airBomb.Restart;
        airBomb.FinishBomb.AddListener((IsVictory) => Next = IsVictory);
        yield return new WaitUntil(() => Next == true);
        Next = false;
        FinishPanelUp();
        #endregion
    }

    private IEnumerator TexturePaint_Weapon_DesertWall()
    {
        #region Map
        SceneManager.LoadScene("Map");
        yield return new WaitUntil(() => Next == true);
        Next = false;
        var map = Map;
        var baseUpgrader = Map.gameObject.GetComponent<BaseUpgrader>();
        map.StartMap(CurrentLvl);
        baseUpgrader.EnableBase(ref moneyBonus);

        map.FinishMap.AddListener(() => Next = true);
        yield return new WaitUntil(() => Next == true);
        Next = false;
        #endregion

        #region Инициализация
        SceneManager.LoadScene("Hangar");
        yield return new WaitUntil(() => Next == true);
        Next = false;

        AirParkHangar hangar = AirPark as AirParkHangar;

        MainCamera.OnFinishParking.AddListener(() => Next = true);
        MainCamera.SetPos(hangar.CameraPos1);
        MainCamera.SetCameraParking(hangar.CameraPos3, 0.1f);
        var air = hangar.InstanceAir(AirType.AN64);
        #endregion

        #region ПОКРАСКА
        yield return new WaitUntil(() => Next == true);
        Next = false;

        var paintAir = air.GetComponent<PaintAir>();
        var colorMenu = hangar.canvas.GetComponent<MenuColorUI>();
        var accessColors = hangar.colors;

        colorMenu.EnableMenuTextures(paintAir, 0, 1, 2,hangar.paintSound);
        var tutorial = hangar.canvas.GetComponent<Tutorial>();
        paintAir.OnFinish3DPainting.AddListener(() => Next = true);

        yield return new WaitUntil(() => Next == true);
        Next = false;
        tutorial.PaintFingerOff();
        // paintAir.FinishColor(hangar.standardMaterial);
        paintAir.FinishTexture(hangar.standardMaterial);
        yield return new WaitForSeconds(1.5f);
        #endregion
        

        #region СМЕНА ОРУЖИЯ
        var weaponAir = air.GetComponent<EquipWeapon>();
        var weaponMenu = hangar.canvas.GetComponent<EquipMenuUI>();
        var mainMenu = hangar.canvas.GetComponent<MainMenuUI>();

        MainCamera.SetCameraParking(hangar.CameraPos2, 0.1f);

        yield return new WaitUntil(() => Next == true);
        Next = false;
        weaponMenu.EnableEquipMenu(weaponAir, AirType.AN64, 0, 1, 2);
        weaponMenu.FinishMechanic.AddListener(() => Next = true);


        yield return new WaitUntil(() => Next == true);
        weaponMenu.FinishMechanic.RemoveAllListeners();
        Next = false;

        mainMenu.EnableNextButton("DesertWall");
        mainMenu.OnButtonPressed += () => Next = true;
        mainMenu.OnButtonPressed += () => weaponMenu.DisableEquipMenu();

        yield return new WaitUntil(() => Next == true);
        Next = false;
        air.transform.parent = this.transform;

        yield return new WaitUntil(() => Next == true);
        Next = false;
        #endregion

        #region DESERT WALL
        WallAirPark wallPark = AirPark as WallAirPark;

        MainCamera.OnFinishParking.AddListener(() => Next = true);
        MainCamera.SetPos(wallPark.cameraPlace);
        wallPark.battleHelicopter.EnableScript(air,CurrentLvl);
        restartButtonHandler += wallPark.Restart;

        air.GetComponent<Animation>().Play("Screw");
        wallPark.OnFinishBattle += () => Next = true;

        yield return new WaitUntil(() => Next == true);
        Next = false;
        FinishPanelUp();
        #endregion

    }

    private IEnumerator AN64_Weapon_DesertWall()
    {
        #region Map
        SceneManager.LoadScene("Map");
        yield return new WaitUntil(() => Next == true);
        Next = false;
        var map = Map;
        var baseUpgrader = Map.gameObject.GetComponent<BaseUpgrader>();
        map.StartMap(CurrentLvl);
        baseUpgrader.EnableBase(ref moneyBonus);

        map.FinishMap.AddListener(() => Next = true);
        yield return new WaitUntil(() => Next == true);
        Next = false;
        #endregion

        #region Инициализация
        SceneManager.LoadScene("Hangar");
        yield return new WaitUntil(() => Next == true);
        Next = false;

        AirParkHangar hangar = AirPark as AirParkHangar;

        MainCamera.OnFinishParking.AddListener(() => Next = true);
        MainCamera.SetPos(hangar.CameraPos1);
        MainCamera.SetCameraParking(hangar.CameraPos2, 0.1f);
        var air = hangar.InstanceAir(AirType.AN64);
        #endregion


        #region СМЕНА ОРУЖИЯ
        var weaponAir = air.GetComponent<EquipWeapon>();
        var weaponMenu = hangar.canvas.GetComponent<EquipMenuUI>();
        var mainMenu = hangar.canvas.GetComponent<MainMenuUI>();

        MainCamera.SetCameraParking(hangar.CameraPos2, 0.1f);

        yield return new WaitUntil(() => Next == true);
        Next = false;
        weaponMenu.EnableEquipMenu(weaponAir, AirType.AN64, 0, 1, 2);
        weaponMenu.FinishMechanic.AddListener(() => Next = true);


        yield return new WaitUntil(() => Next == true);
        weaponMenu.FinishMechanic.RemoveAllListeners();
        Next = false;

        mainMenu.EnableNextButton("DesertWall");
        mainMenu.OnButtonPressed += () => Next = true;
        mainMenu.OnButtonPressed += () => weaponMenu.DisableEquipMenu();

        yield return new WaitUntil(() => Next == true);
        Next = false;
        air.transform.parent = this.transform;

        yield return new WaitUntil(() => Next == true);
        Next = false;
        #endregion

        #region DESERT WALL
        WallAirPark wallPark = AirPark as WallAirPark;

        MainCamera.OnFinishParking.AddListener(() => Next = true);
        MainCamera.SetPos(wallPark.cameraPlace);
        wallPark.battleHelicopter.EnableScript(air, CurrentLvl);
        restartButtonHandler += wallPark.Restart;

        air.GetComponent<Animation>().Play("Screw");
        wallPark.OnFinishBattle += () => Next = true;

        yield return new WaitUntil(() => Next == true);
        Next = false;
        FinishPanelUp();
        #endregion

    }

    private IEnumerator Weapon_DesertHelicopter()
    {
        #region Map
        SceneManager.LoadScene("Map");
        yield return new WaitUntil(() => Next == true);
        Next = false;
        var map = Map;
        var baseUpgrader = Map.gameObject.GetComponent<BaseUpgrader>();
        map.StartMap(CurrentLvl);
        baseUpgrader.EnableBase(ref moneyBonus);

        map.FinishMap.AddListener(() => Next = true);
        yield return new WaitUntil(() => Next == true);
        Next = false;
        #endregion

        #region Инициализация
        SceneManager.LoadScene("Hangar");
        yield return new WaitUntil(() => Next == true);
        Next = false;

        AirParkHangar hangar = AirPark as AirParkHangar;
        MainCamera.OnFinishParking.AddListener(() => Next = true);
        MainCamera.SetPos(hangar.CameraPos1);
        var air = hangar.InstanceAir(AirType.UN60);
        yield return new WaitForSeconds(1.5f);
        #endregion


        #region СМЕНА ОРУЖИЯ
        var mainMenu = hangar.canvas.GetComponent<MainMenuUI>();
        var weaponAir = air.GetComponent<EquipWeapon>();
        var weaponMenu = hangar.canvas.GetComponent<EquipMenuUI>();

        MainCamera.SetCameraParking(weaponAir.weaponCameraPos, 0.01f);

        yield return new WaitUntil(() => Next == true);
        Next = false;
        air.GetComponent<Animation>().Play("OpenDoor");
        weaponMenu.EnableEquipMenu(weaponAir, AirType.UN60, 0, 1, 2);
        weaponMenu.FinishMechanic.AddListener(() => Next = true);


        yield return new WaitUntil(() => Next == true);
        weaponMenu.FinishMechanic.RemoveAllListeners();
        Next = false;

        mainMenu.EnableNextButton("Desert");
        mainMenu.OnButtonPressed += () => Next = true;
        mainMenu.OnButtonPressed += () => weaponMenu.DisableEquipMenu();

        yield return new WaitUntil(() => Next == true);
        Next = false;
        air.transform.parent = this.transform;

        yield return new WaitUntil(() => Next == true);
        Next = false;
        #endregion

        #region DESERT HELICOPTER
        DesertAirPark desertPark = AirPark as DesertAirPark;
        desertPark.StartTrack(CurrentLvl,air,false,27);
        restartButtonHandler = desertPark.Restart;
        AirPlatform platform = desertPark.helicopterPlatform.GetComponent<AirPlatform>();
        air.GetComponent<Animation>().Play("Screw");

       // MainCamera.OnFinishParking.AddListener(() => Next = true); // Из за активации компонентов по таймеру фиксация финиша камеры не нужна!!!!
        MainCamera.SetPos(platform.CameraStartPlace);

        yield return new WaitForSeconds(1f);

        MainCamera.SetCameraParking(platform.CameraPlace, 0.001f);

        yield return new WaitForSeconds(1.5f);
        MainCamera.transform.parent = platform.CameraPlace;
        MainCamera.SetPos(platform.CameraPlace);
        air.SetActive(false);
        Instantiate(weaponAir.GetWeapon(), platform.GunPlace);
        platform.GetComponent<Animation>().Play("Track1");
        platform.screw.SetActive(true);
        platform.enabled = true;
        desertPark.FinishTrack.AddListener((IsVictory) => Next = IsVictory);
        Next = false;

        yield return new WaitUntil(() => Next == true);
        Next = false;
        FinishPanelUp();
        #endregion
    }

    private IEnumerator UN60_TexturePaint_Weapon_DesertHelicopter()
    {
        #region Map
        SceneManager.LoadScene("Map");
        yield return new WaitUntil(() => Next == true);
        Next = false;
        var map = Map;
        var baseUpgrader = Map.gameObject.GetComponent<BaseUpgrader>();
        map.StartMap(CurrentLvl);
        baseUpgrader.EnableBase(ref moneyBonus);

        map.FinishMap.AddListener(() => Next = true);
        yield return new WaitUntil(() => Next == true);
        Next = false;
        #endregion

        #region Инициализация
        SceneManager.LoadScene("Hangar");
        yield return new WaitUntil(() => Next == true);
        Next = false;

        AirParkHangar hangar = AirPark as AirParkHangar;
        MainCamera.OnFinishParking.AddListener(() => Next = true);
        MainCamera.SetPos(hangar.CameraPos1);
        var air = hangar.InstanceAir(AirType.UN60);
        yield return new WaitForSeconds(1.5f);
        #endregion

        #region ПОКРАСКА ТЕКСТУРОЙ
        MainCamera.SetCameraParking(hangar.CameraPos3, 0.1f);

        yield return new WaitUntil(() => Next == true);
        Next = false;

        var paintAir = air.GetComponent<PaintAir>();
        var colorMenu = hangar.canvas.GetComponent<MenuColorUI>();
        var accessColors = hangar.colors;

        colorMenu.EnableMenuTextures(paintAir, 0, 1, 2, hangar.paintSound);
        paintAir.OnFinish3DPainting.AddListener(() => Next = true);

        yield return new WaitUntil(() => Next == true);
        Next = false;
        hangar.canvas.GetComponent<Tutorial>().PaintFingerOff();
        paintAir.FinishTexture(hangar.standardMaterial);
        yield return new WaitForSeconds(1.5f);
        #endregion

        #region СМЕНА ОРУЖИЯ
        var mainMenu = hangar.canvas.GetComponent<MainMenuUI>();
        var weaponAir = air.GetComponent<EquipWeapon>();
        var weaponMenu = hangar.canvas.GetComponent<EquipMenuUI>();

        MainCamera.SetCameraParking(weaponAir.weaponCameraPos, 0.01f);

        yield return new WaitUntil(() => Next == true);
        Next = false;
        air.GetComponent<Animation>().Play("OpenDoor");
        weaponMenu.EnableEquipMenu(weaponAir, AirType.UN60, 0, 1, 2);
        weaponMenu.FinishMechanic.AddListener(() => Next = true);


        yield return new WaitUntil(() => Next == true);
        weaponMenu.FinishMechanic.RemoveAllListeners();
        Next = false;

        mainMenu.EnableNextButton("Desert");
        mainMenu.OnButtonPressed += () => Next = true;
        mainMenu.OnButtonPressed += () => weaponMenu.DisableEquipMenu();

        yield return new WaitUntil(() => Next == true);
        Next = false;
        air.transform.parent = this.transform;

        yield return new WaitUntil(() => Next == true);
        Next = false;
        #endregion

        #region DESERT HELICOPTER
        DesertAirPark desertPark = AirPark as DesertAirPark;
        desertPark.StartTrack(CurrentLvl, air, false, 27);
        restartButtonHandler = desertPark.Restart;
        AirPlatform platform = desertPark.helicopterPlatform.GetComponent<AirPlatform>();
        air.GetComponent<Animation>().Play("Screw");

        // MainCamera.OnFinishParking.AddListener(() => Next = true); // Из за активации компонентов по таймеру фиксация финиша камеры не нужна!!!!
        MainCamera.SetPos(platform.CameraStartPlace);

        yield return new WaitForSeconds(1f);

        MainCamera.SetCameraParking(platform.CameraPlace, 0.001f);

        yield return new WaitForSeconds(1.5f);
        MainCamera.transform.parent = platform.CameraPlace;
        MainCamera.SetPos(platform.CameraPlace);
        air.SetActive(false);
        Instantiate(weaponAir.GetWeapon(), platform.GunPlace);
        platform.GetComponent<Animation>().Play("Track1");
        platform.screw.SetActive(true);
        platform.enabled = true;
        desertPark.FinishTrack.AddListener((IsVictory) => Next = IsVictory);
        Next = false;

        yield return new WaitUntil(() => Next == true);
        Next = false;
        FinishPanelUp();
        #endregion
    }

    private IEnumerator Weapon_Bomb()
    {
        #region Map
        SceneManager.LoadScene("Map");
        yield return new WaitUntil(() => Next == true);
        Next = false;
        var map = Map;
        var baseUpgrader = Map.gameObject.GetComponent<BaseUpgrader>();
        map.StartMap(CurrentLvl);
        baseUpgrader.EnableBase(ref moneyBonus);

        map.FinishMap.AddListener(() => Next = true);
        yield return new WaitUntil(() => Next == true);
        Next = false;
        #endregion

        SceneManager.LoadScene("BattleAir");
        yield return new WaitUntil(() => Next == true);
        Next = false;

        var battleAir = AirPark as AirParkBattleAir;
        var air = battleAir.InstanceAir(AirType.F15);
        air.transform.position = battleAir.getAirPlace(AirType.F15).position;
        air.transform.rotation = battleAir.getAirPlace(AirType.F15).rotation;

      //  MainCamera.OnFinishParking.AddListener(() => Next = true);
        var airBomb = battleAir.GetComponent<AirBomb>();
        airBomb.EnableScript(CurrentLvl,MainCamera);
        restartButtonHandler = airBomb.Restart;
        airBomb.FinishBomb.AddListener((IsVictory) => Next = IsVictory);
        yield return new WaitUntil(() => Next == true);
        Next = false;
        FinishPanelUp();
    }

    private IEnumerator Weapon_EarthBattle()
    {

        #region Map
        SceneManager.LoadScene("Map");
        yield return new WaitUntil(() => Next == true);
        Next = false;
        var map = Map;
        var baseUpgrader = Map.gameObject.GetComponent<BaseUpgrader>();
        map.StartMap(CurrentLvl);
        baseUpgrader.EnableBase(ref moneyBonus);

        map.FinishMap.AddListener(() => Next = true);
        yield return new WaitUntil(() => Next == true);
        Next = false;
        #endregion

        #region Инициализация
        SceneManager.LoadScene("Hangar");
        yield return new WaitUntil(() => Next == true);
        Next = false;

        AirParkHangar hangar = AirPark as AirParkHangar;
        MainCamera.SetCameraParking(hangar.CameraPos2, 0.1f);
        var air = hangar.InstanceAir(AirType.AN64);
        MainCamera.OnFinishParking.AddListener(() => Next = true);
        MainCamera.SetPos(hangar.CameraPos1);
        #endregion

        #region СМЕНА ОРУЖИЯ
        var weaponAir = air.GetComponent<EquipWeapon>();
        var weaponMenu = hangar.canvas.GetComponent<EquipMenuUI>();
        var mainMenu = hangar.canvas.GetComponent<MainMenuUI>();

        MainCamera.SetCameraParking(hangar.CameraPos2, 0.1f);

        yield return new WaitUntil(() => Next == true);
        Next = false;
        weaponMenu.EnableEquipMenu(weaponAir, AirType.AN64, 1, 3, 4);
        weaponMenu.FinishMechanic.AddListener(() => Next = true);


        yield return new WaitUntil(() => Next == true);
        weaponMenu.FinishMechanic.RemoveAllListeners();
        Next = false;

        mainMenu.EnableNextButton("DesertWall");
        mainMenu.OnButtonPressed += () => Next = true;
        mainMenu.OnButtonPressed += () => weaponMenu.DisableEquipMenu();

        yield return new WaitUntil(() => Next == true);
        Next = false;
        air.transform.parent = this.transform;

        yield return new WaitUntil(() => Next == true);
        Next = false;
        #endregion
        
        #region DESERT WALL
        EarthBattle earthBattle = AirPark.GetComponent<EarthBattle>();
        earthBattle.EnableScript(air.transform,CurrentLvl);
        earthBattle.tutorial.BattleAirOn();
        restartButtonHandler = earthBattle.RestartScript;
       // restartButtonHandler += () => Next = true;
        air.GetComponent<Animation>().Play("Screw");
        earthBattle.Victory.AddListener(() => Next = true);

        yield return new WaitUntil(() => Next == true);
        Next = false;
        earthBattle.tutorial.BattleAirOff();
        FinishPanelUp();
        #endregion

    }

    private IEnumerator UN60_TexturePaint_Weapon_FireCover()
    {
        #region Map
        SceneManager.LoadScene("Map");
        yield return new WaitUntil(() => Next == true);
        Next = false;
        var map = Map;
        var baseUpgrader = Map.gameObject.GetComponent<BaseUpgrader>();
        map.StartMap(CurrentLvl);
        baseUpgrader.EnableBase(ref moneyBonus);

        map.FinishMap.AddListener(() => Next = true);
        yield return new WaitUntil(() => Next == true);
        Next = false;
        #endregion

        #region Инициализация
        SceneManager.LoadScene("Hangar");
        yield return new WaitUntil(() => Next == true);
        Next = false;

        AirParkHangar hangar = AirPark as AirParkHangar;

        MainCamera.OnFinishParking.AddListener(() => Next = true);
        MainCamera.SetPos(hangar.CameraPos1);
        MainCamera.SetCameraParking(hangar.CameraPos3, 0.1f);
        var air = hangar.InstanceAir(AirType.UN60);
        #endregion

        #region ПОКРАСКА
        yield return new WaitUntil(() => Next == true);
        Next = false;

        var paintAir = air.GetComponent<PaintAir>();
        var colorMenu = hangar.canvas.GetComponent<MenuColorUI>();
        var accessColors = hangar.colors;

        colorMenu.EnableMenuTextures(paintAir, 0, 1, 2, hangar.paintSound);
        var tutorial = hangar.canvas.GetComponent<Tutorial>();
        paintAir.OnFinish3DPainting.AddListener(() => Next = true);

        yield return new WaitUntil(() => Next == true);
        Next = false;
        tutorial.PaintFingerOff();
        // paintAir.FinishColor(hangar.standardMaterial);
        paintAir.FinishTexture(hangar.standardMaterial);
        yield return new WaitForSeconds(1.5f);
        #endregion  


        #region СМЕНА ОРУЖИЯ
        var mainMenu = hangar.canvas.GetComponent<MainMenuUI>();
        var weaponAir = air.GetComponent<EquipWeapon>();
        var weaponMenu = hangar.canvas.GetComponent<EquipMenuUI>();
        
        

        MainCamera.SetCameraParking(weaponAir.weaponCameraPos, 0.01f);

        yield return new WaitUntil(() => Next == true);
        Next = false;
        air.GetComponent<Animation>().Play("OpenDoor");
        weaponMenu.EnableEquipMenu(weaponAir, AirType.UN60, 0, 1, 2);
        weaponMenu.FinishMechanic.AddListener(() => Next = true);


        yield return new WaitUntil(() => Next == true);
        weaponMenu.FinishMechanic.RemoveAllListeners();
        Next = false;

        mainMenu.EnableNextButton("Defence");
        mainMenu.OnButtonPressed += () => Next = true;
        mainMenu.OnButtonPressed += () => weaponMenu.DisableEquipMenu();

        yield return new WaitUntil(() => Next == true);
        Next = false;
        air.transform.parent = this.transform;
        int weaponIndex = weaponMenu.selectIndexWeapon;
        yield return new WaitUntil(() => Next == true);
        Next = false;
        #endregion

        #region FireCover
        var fireCover = AirPark as FireCover;
        fireCover.EnableScript(air, CurrentLvl,weaponIndex, weaponAir.GetWeapon(),0);
        air.GetComponent<Animation>().Play("OpenDoor");
        yield return new WaitForSeconds(1f);
        air.GetComponent<Animation>().Play("Screw");

        fireCover.finishGame.AddListener((IsVivtory) => Next = IsVivtory);
        restartButtonHandler = fireCover.RestartGame;
        yield return new WaitUntil(() => Next == true);
        Next = false;
        FinishPanelUp();

        #endregion
    }

    private IEnumerator UN60_FireCover()
    {
        #region Map
        SceneManager.LoadScene("Map");
        yield return new WaitUntil(() => Next == true);
        Next = false;
        var map = Map;
        var baseUpgrader = Map.gameObject.GetComponent<BaseUpgrader>();
        map.StartMap(CurrentLvl);
        baseUpgrader.EnableBase(ref moneyBonus);

        map.FinishMap.AddListener(() => Next = true);
        yield return new WaitUntil(() => Next == true);
        Next = false;
        #endregion

        #region FireCover
        SceneManager.LoadScene("Defence");
        yield return new WaitUntil(() => Next == true);
        Next = false;

        var fireCover = AirPark as FireCover;
       var air = fireCover.InstanceAir(AirType.UN60);
        fireCover.EnableScript(air, CurrentLvl, 1,0);
        air.GetComponent<Animation>().Play("OpenDoor");
        yield return new WaitForSeconds(1f);
        air.GetComponent<Animation>().Play("Screw");

        fireCover.finishGame.AddListener((IsVivtory) => Next = IsVivtory);
        restartButtonHandler = fireCover.RestartGame;
        yield return new WaitUntil(() => Next == true);
        Next = false;
        FinishPanelUp();
        #endregion
    }

    private IEnumerator F16_Catapult_StartEngine_AirBattle()
    {
        #region Map
        SceneManager.LoadScene("Map");
        yield return new WaitUntil(() => Next == true);
        Next = false;
        var map = Map;
        var baseUpgrader = Map.gameObject.GetComponent<BaseUpgrader>();
        map.StartMap(CurrentLvl);
        baseUpgrader.EnableBase(ref moneyBonus);

        map.FinishMap.AddListener(() => Next = true);
        yield return new WaitUntil(() => Next == true);
        Next = false;
        #endregion

        #region AIRFIELD БЕЗ АНГАРА
        SceneManager.LoadScene("AirField");
        yield return new WaitUntil(() => Next == true);
        Next = false;

        AirFieldLocation airField = AirPark as AirFieldLocation;
        MainCamera.OnFinishParking.AddListener(() => Next = true);
        var air = airField.InstanceAir(AirType.F16);
        var equipEngine = air.GetComponent<EquipEngine>();
        equipEngine.SetCurrentEngine(equipEngine.defaultEngine);


        MainCamera.SetPos(airField.camCatapultPlace);
        airField.CurrentAir = air;
        air.transform.parent = airField.getAirPlace(AirType.F16);
        air.transform.localPosition = new Vector3(0, 0, 0);
        air.transform.localRotation = Quaternion.Euler(-90, 0, 0);
        air.transform.localScale = new Vector3(18, 18, 18);

        yield return new WaitForSeconds(1f);
        #endregion

        #region КАТАПУЛЬТА
        var catapult = airField.GetComponent<CatapultController>();


        catapult.EnableScript();
        airField.tutorial.CatapultOn();

        yield return new WaitForSeconds(0.4f);
        catapult.EnableScript();
        catapult.FinishGame.AddListener(() => Next = true);

        yield return new WaitUntil(() => Next == true);
        airField.tutorial.CatapultOff();

        yield return new WaitForSeconds(1f);
        Next = false;

        MainCamera.SetCameraParking(airField.camViewPlace_2, 0.1f);

        yield return new WaitForSeconds(1.5f);
        Next = false;
        #endregion

        #region ЗАПУСК ДВИГАТЕЛЯ
        var EngineController = air.GetComponent<StartEngine>();
        var tachometr = MainCamera.tachometr;
        var engine = EngineController.enginePlace.GetChild(0).GetComponent<AirEngine>();

        MainCamera.SetCameraParking(EngineController.cameraPlace, 0.1f);

        yield return new WaitUntil(() => Next == true);
        Next = false;

        airField.tutorial.StartEngineOn();

        yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
        EngineController.StartScript(MainCamera);
        EngineController.EndMechanicEvent.AddListener(() => Next = true);

        yield return new WaitUntil(() => Next == true);
        Next = false;
        airField.tutorial.StartEngineOff();
        EngineController.EndScript();

        yield return new WaitForSeconds(1f);
        MainCamera.SetCameraParking(airField.camViewPlace_3, 1f);

        yield return new WaitUntil(() => Next == true);
        Next = false;

        MainCamera.LookAtOn(air.transform);
        air.GetComponent<Animation>().Play();
        MainCamera.takeUpSound.Play();
        MainCamera.greenPhaseEngine.Stop();
        yield return new WaitForSeconds(4f);
        air.GetComponent<Animation>().Stop();
        air.transform.parent = this.transform;
        #endregion

        #region Bomb
        SceneManager.LoadScene("BattleAir");
        yield return new WaitUntil(() => Next == true);
        Next = false;

        var battleAir = AirPark as AirParkBattleAir;
        air.transform.parent = battleAir.getAirPlace(AirType.F16);
        air.transform.localPosition = new Vector3(0, 0, 0);
        air.transform.localRotation = Quaternion.Euler(0, 0, 0);
        air.transform.localScale = new Vector3(18, 18, 18);
        var airBomb = battleAir.GetComponent<AirBomb>();

        airBomb.EnableScript(CurrentLvl, MainCamera);
        restartButtonHandler = airBomb.Restart;
        airBomb.FinishBomb.AddListener((IsVictory) => Next = IsVictory);
        yield return new WaitUntil(() => Next == true);
        Next = false;
        FinishPanelUp();
        #endregion
    }

    private IEnumerator AC130_EquipEngine_BladeEngine_DesertPlane()
    {
        #region Map
        SceneManager.LoadScene("Map");
        yield return new WaitUntil(() => Next == true);
        Next = false;
        var map = Map;
        var baseUpgrader = Map.gameObject.GetComponent<BaseUpgrader>();
        map.StartMap(CurrentLvl);
        baseUpgrader.EnableBase(ref moneyBonus);

        map.FinishMap.AddListener(() => Next = true);
        yield return new WaitUntil(() => Next == true);
        Next = false;
        #endregion

        #region Инициализация
        SceneManager.LoadScene("Hangar");
        yield return new WaitUntil(() => Next == true);
        Next = false;

        AirParkHangar hangar = AirPark as AirParkHangar;

        MainCamera.OnFinishParking.AddListener(() => Next = true);
        MainCamera.SetPos(hangar.CameraPos1);
        var air = hangar.InstanceAir(AirType.AC130);
        #endregion

        
        #region МЕХАНИКА УСТАНОВКИ Двигателя
        var menuEngine = hangar.canvas.GetComponent<EngineMenuUI>();
        var engineAir = air.GetComponent<EquipEngine>();
        MainCamera.SetCameraParking(engineAir.CameraPlace, 0.1f);
        yield return new WaitUntil(() => Next == true);
        Next = false;


        menuEngine.EnableEquipEngine(engineAir, AirType.AC130, 0, 1, 2);
        menuEngine.FinishMechanic.AddListener(()=> Next = true);

        yield return new WaitUntil(() => Next == true);
        Next = false;
        menuEngine.FinishMechanic.RemoveAllListeners();
        var startEngine = air.GetComponent<StartEngine>();
        var mainMenu = hangar.canvas.GetComponent<MainMenuUI>();

       // air.transform.parent = this.transform;
        mainMenu.EnableNextButton();
        mainMenu.OnButtonPressed += () => Next = true;
       // mainMenu.OnButtonPressed += () => menuEngine.DisableMenuEngine();
        yield return new WaitUntil(() => Next == true);
        Next = false;
        menuEngine.DisableMenuEngine();
        startEngine.StartScript(MainCamera);
        startEngine.EndMechanicEvent.AddListener(() => Next = true);
        var tutorial = hangar.canvas.GetComponent<Tutorial>();
        tutorial.StartEngineOn();
        yield return new WaitUntil(() => Next == true);
        Next = false;
        MainCamera.SetCameraParking(hangar.CameraPos2,0.1f);
        tutorial.StartEngineOff();
        startEngine.EndScript();
        yield return new WaitUntil(() => Next == true);
        Next = false;
        var airWeapon = air.GetComponent<EquipWeapon>();
        var weapon = hangar.weapons[(int)AirType.AC130].details[0];
        air.transform.parent = this.transform;
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("Desert");
        #endregion

        #region BATTLE DESERT (PLANE)
        yield return new WaitUntil(() => Next == true);
        Next = false;

        DesertAirPark desertPark = AirPark as DesertAirPark;
        air.transform.parent = desertPark.getAirPlace(AirType.AC130);
        AirPlatform platform = desertPark.planePlatform.GetComponent<AirPlatform>();
        desertPark.StartTrack(CurrentLvl, air, true, 30);
        restartButtonHandler = desertPark.Restart;
        MainCamera.OnFinishParking.AddListener(() => Next = true);

        // air.GetComponent<EquipEngine>().StartAnimEngine();


        MainCamera.SetPos(platform.CameraPlace);
        MainCamera.transform.parent = platform.CameraPlace;
        desertPark.planePath.speed = 20f;

        air.SetActive(false);
        Instantiate(weapon, platform.GunPlace);
        platform.enabled = true;

        desertPark.FinishTrack.AddListener((IsVictory) => Next = IsVictory);

        yield return new WaitUntil(() => Next == true);
        Next = false;

        platform.enabled = false;
        FinishPanelUp();
        #endregion

    }

    private IEnumerator TestBase()
    {
        #region Map
        while (true)
        {
            SceneManager.LoadScene("Map");
            yield return new WaitUntil(() => Next == true);
            Next = false;
            var map = Map;
            var baseUpgrader = Map.gameObject.GetComponent<BaseUpgrader>();
            map.StartMap(CurrentLvl);
            baseUpgrader.EnableBase(ref moneyBonus);

            map.FinishMap.AddListener(() => Next = true);
            yield return new WaitUntil(() => Next == true);
            Next = false;
            PlayerPrefs.DeleteAll();
        }
        #endregion
    }

    #region !NO TOUCH!
    public void Start()
    {

        Next = false;
        StartCoroutine(STARTGAME());
    }


    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelGameScene;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelGameScene;
    }

    private void OnLevelGameScene(Scene scene, LoadSceneMode sceneMode)
    {
        currentScene = scene;
        Next = true;
    }

    private Action restartButtonHandler;

    public void ClickFinishButton()
    {
        NEXTLEVEL = true;
        FinishPanel.Play("Down");
    }
    public void RestartPanelUp()
    {
        SDK.OnFailGame_FB(CurrentLvl);
        SDK.OnFailGame_GA(CurrentLvl);
        RestartPanel.Play("Perfect");
    }
    public void RestartPanelDown()
    {
        RestartPanel.Play("Down");
    }

    public void FinishPanelUp()
    {
        FinishPanel.Play("Perfect");
        int addMoney;
        int index = BaseUpgrader.GetEarningIndex();
        if (index == -1)
        {
            addMoney = 0;
        }
        else
        {
            addMoney = addMoneyBonuses[index];
        }
        moneyBonus = MoneyBonus + addMoney;
        MoneyBonusText.text = "+" + moneyBonus;
    }
    public void FinishPanelDown()
    {
        FinishPanel.Play("Down");
    }
    public void ClickRestartButton()
    {
        restartButtonHandler?.Invoke();
        RestartPanel.Play("Down");
    }

    private IEnumerator STARTGAME()
    {
        int currentSaveIndex = PlayerPrefs.GetInt("SaveIndex", 0);
        if(currentSaveIndex != saveIndex)
        {
            PlayerPrefs.DeleteAll();
        }
        PlayerPrefs.SetInt("SaveIndex", saveIndex);

        while (true)
        {
            NEXTLEVEL = false;
            StartCoroutine(levels[CurrentLvl]());
            yield return new WaitUntil(() => NEXTLEVEL == true);

            int curLvl = CurrentLvl++;
            int completedLvls = CompletedLevels++;
            SDK.OnLvlEnded_FB(curLvl, completedLvls);
            SDK.OnLvlEnded_GA(curLvl, completedLvls);
        }
    }
    #endregion

    /*
        #region ОТМЫВКА
        MainCamera.SetCameraParking(hangar.CameraPos3, 0.1f);
        yield return new WaitUntil(() => Next == true);
        Next = false;

        var tutorial = hangar.canvas.GetComponent<Tutorial>();
        var washAir = air.GetComponent<PaintAir>();
        washAir.EnableScript(hangar.washSound);
        tutorial.PaintFingerOn(true);

        washAir.OnFinish3DPainting += () => Next = true;
        yield return new WaitUntil(() => Next == true);
        Next = false;
        tutorial.PaintFingerOff();
        yield return new WaitForSeconds(1f);
        Destroy(air);
        #endregion
        */

    /*
     *  #region СТИКЕРЫ
        var decalAir = air.GetComponent<PaintDecalAir>();
        var decalMenu = hangar.canvas.GetComponent<MenuDecalUI>();
        var mainMenu = hangar.canvas.GetComponent<MainMenuUI>();
        decalMenu.EnableMenuDecals(decalAir, 0, 1, 2, hangar.decalSound);

        decalAir.OnFinishDecal += () => Next = true;

        yield return new WaitUntil(() => Next == true);
        Next = false;
        decalMenu.DisableMenuDecals();

        yield return new WaitForSeconds(1f);
        #endregion
     */
}
