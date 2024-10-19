using System;
using UnityEngine;
using UnityEngine.EventSystems;

public sealed class TouchButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Action onPointerDown { get; set; }
    public Action onPointerUp { get; set; }

    public void OnPointerDown(PointerEventData eventData)
    {
        onPointerDown?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        onPointerUp?.Invoke();
    }
}