using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthLocationRocket : MonoBehaviour
{
    private Vector3 startRocketPos;
    private Quaternion startRocketRotate;

    // Start is called before the first frame update
    void Start()
    {
        startRocketPos = transform.position;
        startRocketRotate = transform.rotation;
    }
    public void OnTriggerEnter(Collider other)
    {
        var enemy = other.transform.GetComponent<FollowBot>();
        enemy.Liquidation();
        transform.rotation = startRocketRotate;
        transform.position = startRocketPos;
        GetComponent<MissionGun>().flying = false;
    }
}
