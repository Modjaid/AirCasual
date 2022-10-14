using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthData
{
    private float slideBarPoints;
    private float speedUp;

    public float slideBarDiapason;
    private float roundPoints;
    public float RoundPoints
    {
        get { return roundPoints; }
        set
        {
            if (value < 0)
            {
                roundPoints = 0;
            }
            else
            {
                if(value > slideBarPoints)
                {
                    slideBarPoints = value;
                }
                roundPoints = value;
            }
        }
    }


    public HealthData(float slideBarDiapason)
    {
        this.slideBarDiapason = slideBarDiapason;
        slideBarPoints = slideBarDiapason;
        speedUp = 4f;
        roundPoints = slideBarDiapason;
    }
    public HealthData(float slideBarDiapason,float speedUp)
    {
        this.slideBarDiapason = slideBarDiapason;
        slideBarPoints = slideBarDiapason;
        this.speedUp = speedUp;
        roundPoints = slideBarDiapason;
    }
    public float GetBarNormalized()
    {
        return slideBarPoints / slideBarDiapason;
    }
    public float GetRoundPointNormalized()
    {
        return roundPoints / slideBarDiapason;
    }
    public void Update()
    {
        if (slideBarPoints > roundPoints)
        {
            slideBarPoints -= speedUp * Time.deltaTime;
        }
    }
    public void Restart()
    {
        roundPoints = slideBarDiapason;
        slideBarPoints = slideBarDiapason;
    }
}
