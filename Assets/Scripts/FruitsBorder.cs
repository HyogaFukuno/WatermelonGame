using System;
using UnityEngine;

public sealed class FruitsBorder : MonoBehaviour
{
    Action onFruitsEnter;

    public event Action OnFruitsEnter
    {
        add => onFruitsEnter += value;
        remove => onFruitsEnter -= value;
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Fruits")
            || !other.gameObject.TryGetComponent<Fruits>(out var fruits))
        {
            return;
        }
        
        if (fruits.IsFalling)
        {
            return;
        }

        Debug.Log("AAA");
        onFruitsEnter?.Invoke();
    }
}