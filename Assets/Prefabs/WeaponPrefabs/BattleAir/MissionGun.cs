using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionGun : MonoBehaviour
{
    public float damage;
    public float detonationDistance;
    public float maxSpeed;
    public float acceleration;
    public float maxGuidanceRotationSpeed;
    [SerializeField] private int cooldawn;

    private Transform target;
    private Vector3 velocity;
    public bool flying = false;
    private Transform parent;

    private DateTime startTimer;

    float speed;

    public bool IsReady()
    {
        return !flying;
       // double restSeconds = ((TimeSpan)(DateTime.Now - startTimer)).TotalSeconds;
       // if (restSeconds > cooldawn)
       // {
       //     return true;
       // }
       // else
       // {
       //     return false;
       // }
    }

    public void Shot(Transform target)
    {
        if (!flying)
        {
          //  parent = transform.parent;
          //  transform.parent = null;
            this.target = target;
            flying = true;

            StartCoroutine(Fly());
        }
        //Invoke("DestroySelf",5f);
    }



    //public void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.tag == "Enemy")
    //    {
    //        Destroy(other.gameObject);
    //        transform.parent = parent;
    //        transform.position = parent.position;
    //        flying = false;

    //    }

    //}

    private void DestroySelf()
    {
        transform.parent = parent;
        transform.position = parent.position;
        flying = false;
    }

    private IEnumerator Fly()
    {
        while (flying)
        {
            // First, steer towards the target.
            float currentRotationSpeed = maxGuidanceRotationSpeed * Time.deltaTime;
            Quaternion targetRotation = Quaternion.LookRotation((target.position - transform.position).normalized);
            Quaternion newRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, currentRotationSpeed);
            transform.rotation = newRotation;

            // Accelerate
            speed += acceleration * Time.deltaTime;
            speed = Mathf.Clamp(speed * Time.deltaTime, 0, maxSpeed);

            // Move.
            transform.Translate(Vector3.forward * speed);


            // Compare distance to target.
            if (target != null)
            {
                float distance = Vector3.Distance(transform.position, target.position);
            }

            yield return new WaitForFixedUpdate();
        }
    }
}
