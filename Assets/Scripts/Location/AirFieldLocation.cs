using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirFieldLocation : AirPark
{
    /// <summary>
    /// Для механики катапульты
    /// </summary>
    public Transform camCatapultPlace;
    /// <summary>
    /// Отдаленная камера от авианосца для полного стартового обзора
    /// </summary>
    public Transform camViewPlace_1;
    /// <summary>
    /// Для выезда камеры между механиками чтобы не проходить сквозь
    /// </summary>
    public Transform camViewPlace_2;
    /// <summary>
    /// для наблюдения за тем как самолет улетает
    /// </summary>
    public Transform camViewPlace_3;

    public Tutorial tutorial;



    void Start()
    {
        
    }

    
    void Update()
    {
        
    }
}
