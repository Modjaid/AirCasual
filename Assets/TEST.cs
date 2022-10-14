using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TEST : MonoBehaviour
{
    public InputField input;
    public Text errorText;
    public GameObject panel;

    public static TEST test;

    public void Awake()
    {
        if (!test)
        {
            test = this;
            DontDestroyOnLoad(transform.gameObject);
            input.onEndEdit.AddListener(EnableLevel);
        }
        else
        {
            Destroy(gameObject);
        }


    }

    public void EnableLevel(string input)
    {
        int newLvl = Convert.ToInt32(input);
        if(LevelManager.instance.levels.Count - 1 < newLvl || newLvl < 0)
        {
           // errorText.text = "Такого лвла не существует";
        }
        else
        {
           // errorText.text = "";
            LevelManager.instance.StopAllCoroutines();
            LevelManager.instance.CurrentLvl = newLvl;
            LevelManager.instance.Start();
        }
    }

    public void ActivePanel()
    {
        if (panel.activeSelf)
        {
            panel.SetActive(false);
        }
        else
        {
            panel.SetActive(true);
        }
    }


    
}
