using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AirParkHangar : AirPark
{
    /// <summary>
    /// "AN64
    /// "AC130
    /// "F22
    /// "F15
    /// "F16
    /// "UN60
    /// </summary>
    public GameObject[] airDirtyPrefabs;

    [SerializeField] public AirDetails[] weapons;

    [SerializeField] public AirDetails[] engines;

    [SerializeField] public GameObject canvas;
    
    [Space(2)]
    [Header("КАМЕРЫ")]
    /// <summary>
    /// Камера прямо на Аир
    /// </summary>
    [SerializeField] public Transform CameraPos1;

    /// <summary>
    /// Камера смотрит на Аир под углом
    /// </summary>
    [SerializeField] public Transform CameraPos2;

    /// <summary>
    /// Камера смотрит на Аир под углом для расскраски
    /// </summary>
    [SerializeField] public Transform CameraPos3;

    [Header("Для рисования")]
    [SerializeField] public Color[] colors;
    [SerializeField] public Texture[] textures;
    [SerializeField] public Sprite[] textureAvatars;
    [SerializeField] public Texture[] decals;
    [SerializeField] public Sprite[] decalAvatars;
    [SerializeField] public AudioSource paintSound;
    [SerializeField] public AudioSource washSound;
    [SerializeField] public AudioSource decalSound;

    [SerializeField] public Material standardMaterial;

}
[Serializable]
public class AirDetails
{
    [SerializeField] private string AirName;
    [SerializeField] public GameObject[] details;
    [SerializeField] public Sprite[] avatars;
    [Header("Только для оружия")]
    [SerializeField] public WeaponType[] weaponTypes;
}
