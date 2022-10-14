using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideBarData
{
    private float slideBarPoints;
    private float speedUp;

    public float slideBarDiapason;
    private float targetPoints;
    public float TargetPoints
    {
        get { return targetPoints; }
        set
        {
            if (value > slideBarDiapason)
            {
                targetPoints = slideBarDiapason;
            }
            else
            {
                targetPoints = value;
            }
        }
    }

    public SlideBarData(float slideBarDiapason)
    {
        this.slideBarDiapason = slideBarDiapason;
        slideBarPoints = 0;
        speedUp = 0.5f;
        targetPoints = 0;
    }
    public float GetBarNormalized()
    {
        return slideBarPoints / slideBarDiapason;
    }
    public void Update()
    {
        if (targetPoints > slideBarPoints)
        {
            slideBarPoints += speedUp * Time.deltaTime;
        }else if (targetPoints < slideBarPoints)
        {
            slideBarPoints -= speedUp/2 * Time.deltaTime;
        }
    }

}
