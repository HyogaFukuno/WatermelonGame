using System;
using UnityEngine;

public sealed class FruitsCallback : MonoBehaviour
{
    Action<Fruits> onFruitsEnter;

    public event Action<Fruits> OnFruitsEnter
    {
        add => onFruitsEnter += value;
        remove => onFruitsEnter -= value;
    }

    public bool IsFalling { get; set; } = true;

    void OnCollisionEnter2D(Collision2D other)
    {
        IsFalling = false;
        
        if (!other.collider.CompareTag("Fruits")
            || !other.collider.TryGetComponent<Fruits>(out var otherFruits))
        {
            return;
        }

        if (!other.gameObject.activeSelf)
        {
            return;
        }

        onFruitsEnter?.Invoke(otherFruits);
    }
}
