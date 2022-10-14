using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotWall : MonoBehaviour
{
    public Weapon weapon;
    Outline _outline;


    void Start()
    {
        weapon.SetTargetPos(WallAirPark.RandomPointInBounds());
        _outline = GetComponent<Outline>();
    }

    // Update is called once per frame
    void Update()
    {

       bool isDone = weapon.Shot();
        if (isDone)
        {
            weapon.SetTargetPos(WallAirPark.RandomPointInBounds());
        }
    }
    public void TurnOnOutLine()
    {
        _outline.enabled = true;
    }

}
