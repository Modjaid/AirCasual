using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shaker 
{
    private float shakeScore = 0;
    private float ShakeScore
    {
        set
        {
            shakeScore = value;
            if (shakeScore < 0)
            {
                shakeScore = 0;
            }
        }
        get
        {
            return shakeScore;
        }
    }
    private float dicrementSpeed;
    
    /// <summary>
    /// В функции update
    /// </summary>
    public void Shake(Transform obj)
    {
        obj.localPosition = Random.insideUnitSphere * (ShakeScore * 0.0005f);
        ShakeScore -= dicrementSpeed * Time.deltaTime;
    }
    public void Push(float force,float dicrementSpeed = 1)
    {
        this.dicrementSpeed = dicrementSpeed * 100;
        ShakeScore = force;
    }
}
