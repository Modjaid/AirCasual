using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.Events;

public class DragHandler : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    public UnityEvent<Vector2> onDrag;

    public void OnDrag(PointerEventData eventData)
    {
        onDrag?.Invoke(eventData.delta);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        onDrag?.Invoke(eventData.delta);
    }
}

