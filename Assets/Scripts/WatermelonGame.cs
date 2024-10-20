using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using FruitsType = Fruits.FruitType;
using Random = UnityEngine.Random;

[Serializable]
public struct FruitsSprite
{
    public FruitsType type;
    public Sprite sprite;
}

public sealed class WatermelonGame : MonoBehaviour
{
    [Header("Users")]
    [SerializeField] UserScore userScore;
    
    [Header("Inputs")]
    [SerializeField] PlayerInput playerInput;
    [SerializeField] InputActionReference moveAction;
    [SerializeField] InputActionReference enterAction;
    
    [Header("Fruits")]
    [SerializeField] FruitsCreator pool;
    [SerializeField] FruitsType maxSpawnedFruitType = FruitsType.Apple;
    [SerializeField] Transform spawnedFruitsParent;
    [SerializeField] FruitsSprite[] fruitsSprites;
    [SerializeField] FruitsBorder border;
    Fruits currentFruits;
    FruitsType nextFruitType;
    bool isFinish;

    [Header("Cursor")]
    [SerializeField] float cursorSpeed = 3.0f;
    Vector2 rawDirection;
    
    [Header("Sounds")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip dropClip;

    [Header("UI")]
    [SerializeField] TouchButton moveLeftButton;
    [SerializeField] TouchButton moveRightButton;
    [SerializeField] Button enterButton;
    [SerializeField] Button reloadButton;
    [SerializeField] Image nextFruitImage;
    [SerializeField] RectTransform gameOverView;
    
    void Awake()
    {
        Application.targetFrameRate = 120;
        
        StartCoroutine(SpawnFruitsCoroutine());
    }

    void OnEnable()
    {
        playerInput.onActionTriggered += OnMove;
        playerInput.onActionTriggered += OnEnter;
        
        border.OnFruitsEnter += OnFruitsEnterBorder;
        
        moveLeftButton.onPointerDown += () => OnMove(Vector2.left);
        moveRightButton.onPointerDown += () => OnMove(Vector2.right);
        moveLeftButton.onPointerUp += () => OnMove(Vector2.zero);
        moveRightButton.onPointerUp += () => OnMove(Vector2.zero);
        enterButton.onClick.AddListener(OnDropFruits);
        reloadButton.onClick.AddListener(() => SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name));
    }
    
    void OnDisable()
    {
        playerInput.onActionTriggered -= OnMove;
        playerInput.onActionTriggered -= OnEnter;
        
        border.OnFruitsEnter -= OnFruitsEnterBorder;

        moveLeftButton.onPointerDown = null;
        moveLeftButton.onPointerUp = null;
        moveRightButton.onPointerDown = null;
        moveLeftButton.onPointerUp = null;
        enterButton.onClick.RemoveListener(OnDropFruits);
    }

    void FixedUpdate()
    {
        if (isFinish)
        {
            return;
        }

        var current = spawnedFruitsParent.localPosition;
        
        current += new Vector3(rawDirection.x, 0.0f) * (cursorSpeed * Time.deltaTime);
        current.x = Mathf.Min(2.6f, Mathf.Max(-2.6f, current.x));
        spawnedFruitsParent.localPosition = current;

        if (currentFruits is not null)
        {
            currentFruits.transform.localPosition = spawnedFruitsParent.localPosition;
        }
    }

    IEnumerator SpawnFruitsCoroutine()
    {
        nextFruitType = (FruitsType)Random.Range(0, (int)maxSpawnedFruitType);
        nextFruitImage.enabled = false;

        yield return null;
        
        while (gameObject.activeInHierarchy)
        {
            if (isFinish)
            {
                break;
            }

            currentFruits = pool.GetFruits(nextFruitType);
            currentFruits.transform.localPosition = spawnedFruitsParent.localPosition;
            currentFruits.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, Random.Range(-60.0f, 60.0f));
            currentFruits.OnSpawnNextFruits += userScore.OnAddScore;
            
            currentFruits.Rigidbody.simulated = false;
            
            nextFruitType = (FruitsType)Random.Range(0, (int)maxSpawnedFruitType);
            nextFruitImage.sprite = fruitsSprites.FirstOrDefault(x => x.type == nextFruitType).sprite;
            nextFruitImage.enabled = true;

            yield return new WaitUntil(() => currentFruits.Rigidbody.simulated);
            
            currentFruits = null;
            
            if (isFinish)
            {
                break;
            }
            
            yield return new WaitForSeconds(1.0f);
        }
    }

    void OnMove(InputAction.CallbackContext context)
    {
        if (context.action != moveAction.action)
        {
            return;
        }

        OnMove(context.ReadValue<Vector2>());
    }

    void OnEnter(InputAction.CallbackContext context)
    {
        if (context.action != enterAction.action)
        {
            return;
        }

        OnDropFruits();
    }

    void OnMove(Vector2 value)
    {
        rawDirection = value;
    }
    
    void OnDropFruits()
    {
        if (currentFruits is null)
        {
            return;
        }

        currentFruits.Rigidbody.simulated = true;
        audioSource.PlayOneShot(dropClip);
    }

    void OnFruitsEnterBorder()
    {
        Debug.Log($"IsFinish");
        
        isFinish = true;
        gameOverView.gameObject.SetActive(true);
    }
}