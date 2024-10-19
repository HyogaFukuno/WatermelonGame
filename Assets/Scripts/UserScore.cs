using System;
using System.Linq;
using TMPro;
using UnityEngine;

public sealed class UserScore : MonoBehaviour
{
    [Serializable]
    struct FruitsScoreTable
    {
        public Fruits.FruitType type;
        public int score;
    }

    [SerializeField] FruitsScoreTable[] fruitsScoreTable;
    [SerializeField] int currentScore;
    
    [Header("UI")]
    [SerializeField] TextMeshProUGUI scoreText;
    
    public void OnAddScore(Fruits fruits)
    {
        var addTable = fruitsScoreTable.FirstOrDefault(x => x.type == fruits.Type);
        currentScore += addTable.score;
        scoreText.SetText("{0}", currentScore);
    }
    
    void Awake() => scoreText.SetText("{0}", currentScore);
}