using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreSystem : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private float scoreMultiplier = 10f;
    
    private float score;
    
    void Update()
    {
        score += Time.deltaTime * scoreMultiplier;

        scoreText.text = "Score: " + Mathf.FloorToInt(score).ToString();
    }
}
