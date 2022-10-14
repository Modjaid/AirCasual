using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject ButtonNext;

    public Action LoadScene;
    public event Action OnButtonPressed;

    public void EnableNextButton(string sceneName)
    {
        ButtonNext.SetActive(true);
        ButtonNext.GetComponent<Animation>().Play("Up");
        ButtonNext.GetComponent<Animation>().Play("Jumping");
        LoadScene += () => SceneManager.LoadScene(sceneName);
    }
    public void EnableNextButton()
    {
        ButtonNext.SetActive(true);
        ButtonNext.GetComponent<Animation>().Play("Up");
        ButtonNext.GetComponent<Animation>().Play("Jumping");
        LoadScene = delegate { };
    }


    public void DisableButton()
    {
        OnButtonPressed?.Invoke();
        ButtonNext.GetComponent<Animation>().Play("Down");
        StartCoroutine(delay());

        IEnumerator delay()
        {
            yield return new WaitForSeconds(1f);
            ButtonNext.SetActive(false);
            LoadScene();
        }
    }


    public void NextLevel()
    {
        LevelManager.instance.NEXTLEVEL = true;
    }

}
