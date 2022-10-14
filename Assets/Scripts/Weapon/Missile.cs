using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls auto guided rocket.
/// </summary>
public class Missile : MonoBehaviour
{
    public float damage;
    public float detonationDistance;
    public float maxSpeed;
    public float acceleration;
    public float maxGuidanceRotationSpeed;

    public GameObject terrainEffect;

    public Vector3 target;
    private Vector3 velocity;

    private void Start()
    {

    }

    /// <summary>
    /// Launches rocket.
    /// </summary>
    public void Launch(float damage, float damageDistance, float maxSpeed, Vector3 target)
    {
        transform.parent = null;
        this.damage = damage;
        this.detonationDistance = damageDistance;
        this.maxSpeed = maxSpeed;
        this.target = target;

        StartCoroutine(Fly());
    }


    private IEnumerator Fly()
    {
        while (true)
        {
            float dt = Time.deltaTime;


            float velocityToMaxNormalized = velocity.magnitude / dt / maxSpeed;
            float allowedRotationSpeed = maxGuidanceRotationSpeed * velocityToMaxNormalized * dt;
            Vector3 directionToTarget = (target - transform.position).normalized;
            velocity = Vector3.RotateTowards(velocity, directionToTarget, allowedRotationSpeed, 0f);


            // Accelerate.
            velocity += transform.forward * acceleration * dt;
            velocity = Vector3.ClampMagnitude(velocity, maxSpeed * dt);

            transform.Translate(velocity, Space.World);
            transform.rotation = Quaternion.LookRotation(velocity.normalized);


            yield return null;
        }
    }

    //  private void Detonate()
    //  {
    //      Debug.Log("Boom!");
    //  }

    public void OnTriggerEnter(Collider collider)
    {
        // Debug.Log("TRIGGER");
        switch (collider.tag)
        {
            case "Terrain":
                GameObject boom = Instantiate(terrainEffect);
                boom.transform.position = this.transform.position;
                Destroy(this.gameObject);
                break;
            case "Enemy":
                collider.gameObject.GetComponent<Enemy>().GetDamage(damage, transform.position,(enemy) => Destroy(enemy));
                Destroy(this.gameObject);
                break;
        }
    }
}
