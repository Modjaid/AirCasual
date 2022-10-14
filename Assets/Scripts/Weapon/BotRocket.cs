using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotRocket : Rocket
{

    public void OnTriggerEnter(Collider other)
    {
        // Debug.Log("TRIGGER");

        if (other.tag == "Terrain")
        {
            Destroy(this.gameObject);
        }
        if (other.tag == "Player")
        {
            other.gameObject.GetComponent<Enemy>().GetDamage(damage, transform.position,(enemy)=> Debug.Log("Destroy"));
            Destroy(this.gameObject);
        }

    }
}

