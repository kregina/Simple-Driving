using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreSystem : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private float scoreMultiplier = 10f;

    public const string HighScoreKey = "HighScore";

    private float score;
    
    void Update()
    {
        score += Time.deltaTime * scoreMultiplier;

        scoreText.text = $"Score: {Mathf.FloorToInt(score)}";
    }

    private void OnDestroy()
    {
        int currentHighScore = PlayerPrefs.GetInt(HighScoreKey, 0);

        if (score > currentHighScore)
        {
            PlayerPrefs.SetInt(HighScoreKey, Mathf.FloorToInt(score));
        }
    }
}
