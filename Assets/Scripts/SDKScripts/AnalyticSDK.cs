using Facebook.Unity;
using GameAnalyticsSDK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Компонент вешается на геймобжект DontDestroyOnLoad
/// </summary>
public class AnalyticSDK : MonoBehaviour
{

    /// <summary>
    /// Инициализация Фейсбука и Гейм аналитики
    /// </summary>
    void Start()
    {
        GameAnalytics.Initialize();
        Facebook_init();


        /// Подписка на выход приложения для фиксации дроп уровня
        Application.quitting += () => OnQuitGame_FB(LevelManager.instance.CurrentLvl);
        Application.quitting += () => OnQuitGame_GA(LevelManager.instance.CurrentLvl);
    }

    /// <summary>
    /// метод вызывается на старте данного класса
    /// </summary>
    private void Facebook_init()
    {
        if (!FB.IsInitialized)
        {
            // Initialize the Facebook SDK
            FB.Init(InitCallback, OnHideUnity);
        }
        else
        {
            // Already initialized, signal an app activation App Event
            FB.ActivateApp();
        }

        void InitCallback()
        {
            if (FB.IsInitialized)
            {
                // Signal an app activation App Event
                FB.ActivateApp();
                // Continue with Facebook SDK
                // ...
            }
            else
            {
                Debug.Log("Failed to Initialize the Facebook SDK");
            }
        }
        void OnHideUnity(bool isGameShown)
        {
            if (!isGameShown)
            {
                // Pause the game - we will need to hide
                Time.timeScale = 0;
            }
            else
            {
                // Resume the game - we're getting focus again
                Time.timeScale = 1;
            }
        }
    }

    /// <summary>
    /// Вызывается на финише  при нажатии кнопки NEXT, передаются значения текущего уровня и сколько все было пройденно
    /// </summary>
    /// <param name="lvl">Номер пройденного уровня</param>
    public void OnLvlEnded_FB(int currentLvl,int CompletedLevels)
    {
        var Params = new Dictionary<string, object>();

        Params["Current Level"] = currentLvl.ToString();
        Params["Completed Levels"] = CompletedLevels.ToString();

        FB.LogAppEvent(
            "Level",
            parameters: Params
        );
    }

    /// <summary>
    /// Вызывается на финише  при нажатии кнопки NEXT, передаются значения текущего уровня и сколько все было пройденно
    /// </summary>
    public void OnLvlEnded_GA(int currentLvl, int CompletedLevels)
    {
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "Current Level: " + currentLvl.ToString());
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "Completed levels: " + CompletedLevels.ToString());
    }

    /// <summary>
    /// Вызывается если матч прошел с фейлом
    /// </summary>
    /// <param name="currentLevel"></param>
    public void OnFailGame_FB(int currentLevel)
    {
        var Params = new Dictionary<string, object>();

        Params["Fail Level"] = currentLevel.ToString();

        FB.LogAppEvent(
            "Fail",
            parameters: Params
        );
    }

    /// <summary>
    /// Вызывается если матч прошел с фейлом
    /// </summary>
    /// <param name="currentLevel"></param>
    public void OnFailGame_GA(int currentLevel)
    {
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, "Fail Level: " + currentLevel);
    }

    /// <summary>
    /// Метод подписан на событие с старта
    /// </summary>
    /// <param name="currentLevel"></param>
    public void OnQuitGame_FB(int currentLevel)
    {
        var Params = new Dictionary<string, object>();

        Params["Quit With Level"] = currentLevel.ToString();

        FB.LogAppEvent(
            "Quit with Game",
            parameters: Params
        );
    }

    /// <summary>
    /// Метод подписан на событие с старта
    /// </summary>
    public void OnQuitGame_GA(int currentLevel)
    {
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, "Quit with Level: " + currentLevel);
    }
}
