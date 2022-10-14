using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesertEnemyCalibr : MonoBehaviour
{
    

    public void Launch(Transform playerTarget,float speed)
    {
        this.transform.parent = null;
        StartCoroutine(update());

        IEnumerator update()
        {
            while (true)
            {
                this.transform.position = Vector3.LerpUnclamped(transform.position, playerTarget.position, speed * Time.deltaTime * 0.2f);
                this.transform.LookAt(playerTarget);
                float distance = Vector3.Distance(transform.position, playerTarget.transform.position);

                if(distance < 10f)
                {
                    DesertAirPark.selectedPlatform.GetComponent<AirPlatform>().healthData.RoundPoints -= 1;
                    DesertAirPark.selectedPlatform.GetComponent<AirPlatform>().cameraShaker.Push(5,0.1f);
                    DesertAirPark.selectedPlatform.GetComponent<AirPlatform>().animDamage.Play("Push");
                    Instantiate(GetComponent<Enemy>().deathEffect, DesertAirPark.selectedPlatform);  
                    Destroy(this.gameObject);
                }

                yield return null;
            }
        }
    }
    public void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            DesertAirPark.selectedPlatform.GetComponent<AirPlatform>().cameraShaker.Push(4, 0.05f);
        }
    }
}
