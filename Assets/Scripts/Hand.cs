using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    public Vector3 localOffset;
    public LayerMask mask;

    private Camera camera;
    private Vector3 targetPosition;
    private Quaternion targetRotation;

    private Vector3 startPosition;
    private Quaternion startRotation;

    private void Start()
    {
        camera = Camera.main;

        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    private void LateUpdate()
    {
        RaycastHit hit;
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, float.MaxValue, mask))
        {
            targetPosition = hit.point + transform.TransformVector(localOffset);
            targetRotation = Quaternion.LookRotation(hit.normal);
        }
        else
        {
            targetPosition = startPosition;
            targetRotation = startRotation;
        }

        transform.position = Vector3.Lerp(transform.position, targetPosition, 0.3f);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.3f);
    }
}
