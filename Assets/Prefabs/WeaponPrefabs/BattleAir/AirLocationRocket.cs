using UnityEngine;

public class AirLocationRocket : MonoBehaviour
{
    private Vector3 startRocketPos;
    private Quaternion startRocketRotate;

    // Start is called before the first frame update
    void Start()
    {
        startRocketPos = transform.position;
        startRocketRotate = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

  //  public void OnCollisionEnter(Collision collision)
  //  {
  //      Debug.Log(collision.gameObject.tag);
  //      if(collision.collider.tag == "Enemy")
  //      {
  //          var enemy = collision.transform.GetComponent<AirLocationEnemy>();
  //          enemy.Liquidation();
  //          transform.rotation = startRocketRotate;
  //          transform.position = startRocketPos;
  //          GetComponent<MissionGun>().flying = false;
  //      }
  //  }
    public void OnTriggerEnter(Collider other)
    {
        var enemy = other.transform.GetComponent<AirLocationEnemy>();
        enemy.Liquidation();
         transform.rotation = startRocketRotate;
         transform.position = startRocketPos;
         GetComponent<MissionGun>().flying = false;
    }
}
