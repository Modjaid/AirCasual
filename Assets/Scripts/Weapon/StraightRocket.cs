using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StraightRocket : Rocket
{


    protected override void StartFly()
    {
        StartCoroutine(Fly());
    }

    private IEnumerator Fly()
    {
        while (true)
        {

            float currentRotationSpeed = rotationSpeed * Time.deltaTime;
            Quaternion targetRotation = Quaternion.LookRotation((target - transform.position).normalized);
            Quaternion newRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, currentRotationSpeed);
            transform.rotation = newRotation;

            ////////////////////////////////////
          //  Quaternion targetRotation = Quaternion.LookRotation((target - transform.position).normalized);
          //  transform.rotation = targetRotation;
          ////////////////////////////////////////////

            // Accelerate
            speed += acceleration * Time.deltaTime;
            speed = Mathf.Clamp(speed, 0, maxSpeed);
  

            transform.Translate(Vector3.forward * speed);
        //
        //    var distance = Vector3.Distance(transform.position,target);
        //    var head = startDistance - distance;

            yield return null;
        }
    }

}