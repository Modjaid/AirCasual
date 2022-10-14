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
public class EquipMenuUI : MonoBehaviour
{
    public UnityEvent FinishMechanic;

    [SerializeField] private GameObject equipMenuPanel;
    [SerializeField] private GameObject Button1Background;
    [SerializeField] private GameObject Button2Background;
    [SerializeField] private GameObject Button3Background;

    [SerializeField] private AirParkHangar airPark;
    

    private GameObject[] weapons;
    private WeaponType[] weaponTypes;

    public int selectIndexWeapon;

    private EquipWeapon weaponAir;

    private void Start()
    {
        weaponTypes = new WeaponType[3];
        weapons = new GameObject[3];
    }
    


    public void EnableEquipMenu(EquipWeapon air, AirType airType,int indexWeapon0, int indexWeapon1, int indexWeapon2)
    {
        equipMenuPanel.SetActive(true);
        equipMenuPanel.GetComponent<Animation>().Play("Up");
        var airWeapons = airPark.weapons[(int)airType].details;
        var airWeaponTypes = airPark.weapons[(int)airType].weaponTypes;
        var airWeaponAvatars = airPark.weapons[(int)airType].avatars;

        Button1Background.GetComponent<Image>().sprite = airWeaponAvatars[indexWeapon0];
        Button2Background.GetComponent<Image>().sprite = airWeaponAvatars[indexWeapon1];
        Button3Background.GetComponent<Image>().sprite = airWeaponAvatars[indexWeapon2];

        weaponTypes[0] = airWeaponTypes[indexWeapon0];
        weaponTypes[1] = airWeaponTypes[indexWeapon1];
        weaponTypes[2] = airWeaponTypes[indexWeapon2];
      
        weapons[0] = airWeapons[indexWeapon0];
        weapons[1] = airWeapons[indexWeapon1];
        weapons[2] = airWeapons[indexWeapon2];
      
        this.weaponAir = air;
        equipMenuPanel.GetComponent<Animation>().Play("Up");
    }


    public void DisableEquipMenu()
    {
        equipMenuPanel.GetComponent<Animation>().Play("Down");
        StartCoroutine(delay());

        IEnumerator delay()
        {
            yield return new WaitForSeconds(1f);
            equipMenuPanel.SetActive(false);
        }
    }

    public void ClickButton1()
    {
        weaponAir.ClearAllSlots();
        weaponAir.SetWeaponOnPlaces(weapons[0],weaponTypes[0],true);
        selectIndexWeapon = 0;
        FinishMechanic?.Invoke();
    }
    public void ClickButton2()
    {
        weaponAir.ClearAllSlots();
        weaponAir.SetWeaponOnPlaces(weapons[1], weaponTypes[1], true);
        selectIndexWeapon = 1;
        FinishMechanic?.Invoke();
    }
    public void ClickButton3()
    {
        weaponAir.ClearAllSlots();
        weaponAir.SetWeaponOnPlaces(weapons[2], weaponTypes[2], true);
        selectIndexWeapon = 2;
        FinishMechanic?.Invoke();
    }



}
