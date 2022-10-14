using MoreMountains.NiceVibrations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    public float rotationSpeed;
    public float acceleration;
    public GameObject trail;

    [Header("Ёффект столкновени€ с обычным террэйном")]
    public GameObject terrainEffect;
    [Header("Ёффект столкновени€ с взрывоопасными предметами")]
    public GameObject stuffEffect;

    [Header("√асить ли ракету вручную при достижении цели")]
    public bool IsDetonate = false;

    protected float damage;
    protected float damageRadius;
    protected float maxSpeed;
    protected Vector3 target;
    protected float speed;

   // protected float startDistance;

    public void Launch(float speed, float damage, float damageRadius, Vector3 target)
    {
        maxSpeed = speed;
        this.damage = damage;
        this.damageRadius = damageRadius;
        this.target = target;
        //startDistance = Vector3.Distance(transform.position, target);

        StartFly();
    }

    protected virtual void StartFly()
    {
        StartCoroutine(Fly());
    }

    private IEnumerator Fly()
    {
        while (true)
        {
            // First, steer towards the target.
            float currentRotationSpeed = rotationSpeed * Time.deltaTime;
            Quaternion targetRotation = Quaternion.LookRotation((target - transform.position).normalized);
            Quaternion newRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, currentRotationSpeed);
            transform.rotation = newRotation;

            // Accelerate
            speed += acceleration * Time.deltaTime;
            speed = Mathf.Clamp(speed, 0, maxSpeed);

            // Move.
            transform.Translate(Vector3.forward * speed);


            if (IsDetonate)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target);

                if (distanceToTarget < damageRadius)
                {
                    Detonate();
                }
            }
                
            yield return null;
        }
    }

    protected void Detonate()
    {
        trail.transform.SetParent(null);
     //   GameObject boom = Instantiate(terrainEffect);
     //   boom.transform.position = this.transform.position;
        
        Destroy(this.gameObject);
    }

    /// <summary>
    /// ƒл€ DesertWall
    /// </summary>
    public void OnTriggerEnter(Collider collider)
    {
        MMVibrationManager.ContinuousHaptic(.2f, .07f, .4f);
        Debug.Log("OnTriggerEnter");
        switch (collider.tag)
        {
            case "Terrain":
               GameObject boom = Instantiate(terrainEffect);
               boom.transform.position = this.transform.position;
               Destroy(this.gameObject);
               break;
            case "Enemy":
                collider.gameObject.GetComponent<Enemy>().GetDamage(damage,transform.position, Callback);
                Destroy(this.gameObject);
                break;
        }
  
        void Callback(GameObject enemy)
        {
            enemy.GetComponent<Enemy>().health = 10000000;
            MMVibrationManager.ContinuousHaptic(.9f, .07f, .4f);
            Destroy(enemy,0.1f);
        }
    }

    /// <summary>
    /// ƒл€ Gunship
    /// </summary>
    public void OnCollisionEnter(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Terrain":
                GameObject boom = Instantiate(terrainEffect);
                boom.transform.position = this.transform.position;
                Destroy(this.gameObject);
                break;
            case "Enemy":
                collision.gameObject.GetComponent<Enemy>().GetDamage(damage, transform.position, Callback);
                Destroy(this.gameObject);
                break;
        }
        void Callback(GameObject enemy)
        {
            enemy.GetComponent<Enemy>().health = 10000000;
            enemy.SetActive(false);
            //Destroy(enemy, 0.01f);


            StartCoroutine(delay());

            IEnumerator delay()
            {
                yield return new WaitForSeconds(0.01f);
                enemy.SetActive(false);
            }
        }
    }

}