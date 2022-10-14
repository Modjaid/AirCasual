using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 1) Component component = airWeaponSlots[2].weapon.GetComponent(typeof(IWeapon));
/// 2) weapons[0].GetComponent<MissionGun>()
/// 
/// - Обязательно придать физики самому аиру для рабочего Пуша
/// </summary>
public class EquipWeapon : MonoBehaviour
{
    #region Inspector Fields
    [Header("Все оружейные слоты вертолета")]
    [SerializeField] private List<WeaponSlot> airWeaponSlots;
    [Header("Позиция камеры на оружие")]
    [SerializeField] public Transform weaponCameraPos;
    [Header("Колеса для Joint")]
    [SerializeField] private GameObject wheel;
    #endregion

    private List<GameObject> instWeapons = new List<GameObject>();
 
  

    /// <summary>
    /// метод делает физ скачок Аиру для эффекта вооружения
    /// </summary>
    public void MiniPushTheAir()
    {
        transform.position = new Vector3(transform.parent.position.x, transform.position.y - 0.03f, transform.parent.position.z);
    }
  
    public void SetWeaponOnPlaces(GameObject weaponPrefab, WeaponType weaponType, bool animation = false)
    {
       var wantedSlot = airWeaponSlots.Find((x) => x.slotType == weaponType);
       wantedSlot.InstantiateWeapons(weaponPrefab, animation);
    }

    public Weapon GetWeapon()
    {
        foreach(WeaponSlot slot in airWeaponSlots)
        {
            var weapons = slot.getWeapons();
            if(weapons.Count > 0)
            {
                return weapons[0].GetComponent<Weapon>();
            }
        }
        return null;
    }
    public Weapon[] GetWeapons()
    {
        foreach (WeaponSlot slot in airWeaponSlots)
        {
            var components = slot.getWeapons();
            if (components.Count > 0)
            {
                Weapon[] weapons = new Weapon[components.Count];
                for(int i = 0; i < components.Count; i++)
                {
                    weapons[i] = components[i].GetComponent<Weapon>();
                }
                return weapons;
            }
        }
        return null;
    }


    public void ClearAllSlots()
    {
        foreach(WeaponSlot slot in airWeaponSlots)
        {
            slot.DestroyWeapons();
        }
    }
    public void ActivationWheel()
    {
        GetComponent<Animation>().Play("WheelUp");
        StartCoroutine(delayActiveGravity());

        IEnumerator delayActiveGravity()
        {
            yield return new WaitForSeconds(1f);


            wheel.GetComponent<Rigidbody>().isKinematic = false;
            wheel.GetComponent<FixedJoint>().connectedAnchor = transform.position;
            
            GetComponent<Rigidbody>().isKinematic = false;
        }
    }
    public void ActivationWheelImmidiately()
    {
        GetComponent<Animation>().Play("WheelUpImmidiatly");
        StartCoroutine(delayActiveGravity());
        IEnumerator delayActiveGravity()
        {
            yield return new WaitForSeconds(0.1f);


            wheel.GetComponent<Rigidbody>().isKinematic = false;
            wheel.GetComponent<FixedJoint>().connectedAnchor = transform.position;

            GetComponent<Rigidbody>().isKinematic = false;
        }
    }

    public void DeactivationWheel()
    {
      //  wheel.GetComponent<Rigidbody>().isKinematic = true;
      //  wheel.GetComponent<FixedJoint>().connectedAnchor = transform.position;
      //
      //  GetComponent<Rigidbody>().isKinematic = true;
        GetComponent<Animation>().Play("WheelDown");
    }
   

    




    [Serializable]
    internal class WeaponSlot
    {
        [Header("Оружейные слоты (ПАРНЫЕ)")]
        [SerializeField]private List<Transform> weaponPlaces;
        public WeaponType slotType;
  
        private DateTime startTimer;
        public bool IsFull
        {
            get
            {
                if (weaponPlaces[0].childCount > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
  
  
        public void InstantiateWeapons(GameObject weaponPrefab, bool animation)
        {
            foreach(Transform transform in weaponPlaces)
            {
                if(animation == true)
                {
                   GameObject newWeapon = Instantiate(weaponPrefab, transform);
                  // newWeapon.GetComponent<Animation>().Play("Up");
                }
            }
        }
        
        public void DestroyWeapons()
        {
            foreach (Transform transform in weaponPlaces)
            {
                if (transform.childCount == 1)
                {
                    Destroy(transform.GetChild(0).gameObject);
                }
            }
        }
  
        public List<Component> getWeapons()
        {
            List<Component> weapons = new List<Component>();
            foreach(Transform item in weaponPlaces)
            {
                if (item.childCount > 0)
                {
                    weapons.Add(item.GetChild(0).GetComponent(typeof(Weapon)));
                }
            }
  
            return weapons;
        }
    }

}
public enum WeaponType
{
    AutoGun,
    racket,
    VolleyGun,
    _25mm,
    _45mm,
    _105mm

}


