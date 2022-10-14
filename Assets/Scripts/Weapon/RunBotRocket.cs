using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunBotRocket : Rocket
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
            other.gameObject.GetComponent<Enemy>().GetDamage(damage, transform.position, (enemy) => PlayAnimDeath(enemy));
            Destroy(this.gameObject);
        }

    }
    private void PlayAnimDeath(GameObject enemy)
    {
        enemy.transform.GetChild(0).GetComponent<Animator>().SetTrigger("Death");
    }
}
