using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

using FruitsType = Fruits.FruitType;
using Random = UnityEngine.Random;

public interface IFruitsObjectPool
{
    void SpawnNextFruits(Fruits fruits, Vector2 posA, Vector2 posB);
    void ReleaseFruits(Fruits fruits);
}

[Serializable]
public struct PoolState
{
    public int CountActive;
    public int CountInactive;
    public int CountAll;
}

public class FruitsCreator : MonoBehaviour, IFruitsObjectPool
{
    ObjectPool<Fruits> poolCherry;
    ObjectPool<Fruits> poolStrawberry;
    ObjectPool<Fruits> poolGrape;
    ObjectPool<Fruits> poolDekopon;
    ObjectPool<Fruits> poolPersimmon;
    ObjectPool<Fruits> poolApple;
    ObjectPool<Fruits> poolPear;
    ObjectPool<Fruits> poolPeach;
    ObjectPool<Fruits> poolPineapple;
    ObjectPool<Fruits> poolMelon;
    ObjectPool<Fruits> poolWatermelon;
    
    [SerializeField] Fruits[] prefabs;

    [SerializeField] bool collectionChecked;
    [SerializeField] int defaultCapacity = 12;
    [SerializeField] int maxSize = 36;
    
    [SerializeField] PoolState poolCherryStates;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip spawnNextClip;
    
    public void SpawnNextFruits(Fruits fruits, Vector2 posA, Vector2 posB)
    {
        Debug.Log($"Called FruitsObjectPool.SpawnNextFruits");

        var type = fruits.Type;
        var position = Vector2.Lerp(posA, posB, 0.5f);
        var rotation = fruits.transform.localRotation;

        // 引数のフルーツの種類がスイカの場合は何もしない
        if (type == FruitsType.Watermelon)
        {
            return;
        }

        StartCoroutine(SpawnCoroutine(++type, position, rotation));
        return;

        IEnumerator SpawnCoroutine(FruitsType newType, Vector2 pos, Quaternion localRotation)
        {
            yield return null;

            var next = GetFruits(newType);
            next.Rigidbody.simulated = false;
            next.transform.position = pos;
            next.transform.localRotation = localRotation;
            next.Rigidbody.simulated = true;
            audioSource.PlayOneShot(spawnNextClip);
            
            yield return new WaitForSeconds(0.06f);
        }
    }

    public void ReleaseFruits(Fruits fruits)
    {
        var releasePool = fruits.Type switch
        {
            FruitsType.Cherry => poolCherry,
            FruitsType.Strawberry => poolStrawberry,
            FruitsType.Grape => poolGrape,
            FruitsType.Dekopon => poolDekopon,
            FruitsType.Persimmon => poolPersimmon,
            FruitsType.Apple => poolApple,
            FruitsType.Pear => poolPear,
            FruitsType.Peach => poolPeach,
            FruitsType.Pineapple => poolPineapple,
            FruitsType.Melon => poolMelon,
            FruitsType.Watermelon => poolWatermelon,
            _ => throw new IndexOutOfRangeException()
        };
        releasePool.Release(fruits);
        Debug.Log($"Returned Fruits. Type:{fruits.Type}, Pool: {releasePool.CountActive}");
    }

    public Fruits GetFruits(FruitsType type)
    {
        Fruits fruits;
        var disposable = type switch
        {
            FruitsType.Cherry => poolCherry.Get(out fruits),
            FruitsType.Strawberry => poolStrawberry.Get(out fruits),
            FruitsType.Grape => poolGrape.Get(out fruits),
            FruitsType.Dekopon => poolDekopon.Get(out fruits),
            FruitsType.Persimmon => poolPersimmon.Get(out fruits),
            FruitsType.Apple => poolApple.Get(out fruits),
            FruitsType.Pear => poolPear.Get(out fruits),
            FruitsType.Peach => poolPeach.Get(out fruits),
            FruitsType.Pineapple => poolPineapple.Get(out fruits),
            FruitsType.Melon => poolMelon.Get(out fruits),
            FruitsType.Watermelon => poolWatermelon.Get(out fruits),
            _ => throw new IndexOutOfRangeException()
        };
        
        fruits.Initialize(this, disposable);
        return fruits;
    }

    void Awake()
    {
        poolCherry = CreatePool(FruitsType.Cherry);
        poolStrawberry = CreatePool(FruitsType.Strawberry);
        poolGrape = CreatePool(FruitsType.Grape);
        poolDekopon = CreatePool(FruitsType.Dekopon);
        poolPersimmon = CreatePool(FruitsType.Persimmon);
        poolApple = CreatePool(FruitsType.Apple);
        poolPear = CreatePool(FruitsType.Pear);
        poolPeach = CreatePool(FruitsType.Peach);
        poolPineapple = CreatePool(FruitsType.Pineapple);
        poolMelon = CreatePool(FruitsType.Melon);
        poolWatermelon = CreatePool(FruitsType.Watermelon);
    }

    void Update()
    {
        poolCherryStates.CountActive = poolCherry.CountActive;
        poolCherryStates.CountInactive = poolCherry.CountInactive;
        poolCherryStates.CountAll = poolCherry.CountAll;
    }

    ObjectPool<Fruits> CreatePool(FruitsType type)
        => new(() => OnCreateFruits(type),
            OnGetFruits,
            OnReleaseFruits,
            OnDestroyFruits,
            collectionChecked,
            defaultCapacity,
            maxSize);

    Fruits OnCreateFruits(FruitsType type) => Instantiate(prefabs.FirstOrDefault(x => x.Type == type), transform);
    static void OnGetFruits(Fruits obj) => obj.gameObject.SetActive(true);
    static void OnReleaseFruits(Fruits obj) => obj.gameObject.SetActive(false);
    static void OnDestroyFruits(Fruits obj) => Destroy(obj.gameObject);
}