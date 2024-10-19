using System;
using System.Collections;
using UnityEngine;

public sealed class Fruits : MonoBehaviour
{
    public enum FruitType
    {
        Cherry,
        Strawberry,
        Grape,
        Dekopon,
        Persimmon,
        Apple,
        Pear,
        Peach,
        Pineapple,
        Melon,
        Watermelon,
    }

    IFruitsObjectPool fruitsObjectPool;
    IDisposable releaseDisposable;
    Action<Fruits> onSpawnNextFruits;
    
    [SerializeField] FruitType type;
    [SerializeField] new Rigidbody2D rigidbody;
    [SerializeField] SpriteMask spriteMask;
    bool isDisappear;
    
    public FruitType Type => type;
    public Rigidbody2D Rigidbody => rigidbody;

    public bool IsFalling => this.GetFruitsCallback().IsFalling;
    
    public event Action<Fruits> OnSpawnNextFruits
    {
        add => onSpawnNextFruits += value;
        remove => onSpawnNextFruits -= value;
    }

    public void Initialize(IFruitsObjectPool pool, IDisposable disposable)
    {
        fruitsObjectPool = pool;
        releaseDisposable = disposable;

        isDisappear = false;
        this.GetFruitsCallback().IsFalling = true;
    }

    void OnFruitsEnter(Fruits other)
    {
        if (type != other.type)
        {
            return;
        }

        if (isDisappear || other.isDisappear)
        {
            return;
        }
        
        onSpawnNextFruits?.Invoke(this);
        fruitsObjectPool.SpawnNextFruits(this, transform.position, other.transform.position);
        
        StartCoroutine(OnDisappearCoroutine());
        other.StartCoroutine(other.OnDisappearCoroutine());
    }

    IEnumerator OnDisappearCoroutine()
    {
        isDisappear = true;
        Rigidbody.simulated = false;
        
        var target = Vector3.one * 1.2f;
        var time = 0.0f;
        
        spriteMask.enabled = true;
        while (spriteMask.transform.localScale.x < target.x)
        {
            spriteMask.transform.localScale = Vector3.Lerp(spriteMask.transform.localScale, target, time);
            time += Time.deltaTime * 4.0f;
            yield return null;
        }

        spriteMask.enabled = false;
        spriteMask.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        releaseDisposable?.Dispose();
    }

    void OnEnable()
    {
        this.GetFruitsCallback().OnFruitsEnter += OnFruitsEnter;
    }

    void OnDisable()
    {
        this.GetFruitsCallback().OnFruitsEnter -= OnFruitsEnter;
        onSpawnNextFruits = null;
    }
}

public static class FruitsExtension
{
    public static FruitsCallback GetFruitsCallback(this Fruits fruits)
    {
        if (fruits.TryGetComponent<FruitsCallback>(out var callback))
        {
            return callback;
        }

        return fruits.gameObject.AddComponent<FruitsCallback>();
    }
}