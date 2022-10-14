using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class BezierTest : MonoBehaviour
{

    public Transform P0;
    public Transform P1;
    public Transform P2;
    public Transform P3;

    [Range(0,1)]
    public float t;
    public float speed;
    private float T
    {
        get
        {
            return t;
        }
        set
        {
            t = value;
            if (t >= 1)
            {
                t = 0;
            }
        }
    }

    void Update()
    {
        transform.position = Bezier.GetPoint(P0.position, P1.position, P2.position, P3.position, t);
        transform.rotation = Quaternion.LookRotation(Bezier.GetFirstDerivative(P0.position, P1.position, P2.position, P3.position, t));
        T += Time.deltaTime * speed * 0.0001f;
    }


    private void OnDrawGizmos() {

        int sigmentsNumber = 20;
        Vector3 preveousePoint = P0.position;

        for (int i = 0; i < sigmentsNumber + 1; i++) {
            float paremeter = (float)i / sigmentsNumber;
            Vector3 point = Bezier.GetPoint(P0.position, P1.position, P2.position, P3.position, paremeter);
            Gizmos.DrawLine(preveousePoint, point);
            preveousePoint = point;
        }

    }

}
