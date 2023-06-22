using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private TMP_Text highScoreText;
    [SerializeField] private TMP_Text energyText;
    [SerializeField] private TMP_Text rechargingText;
    [SerializeField] private int maxEnergy;
    [SerializeField] private int energyRechargeDuration;
    [SerializeField] private Button playButton;
    [SerializeField] private AndroidNotificationsHandler androidNotificationsHandler;

    private const string EnergyKey = "Energy";
    private const string EnergyReadyKey = "EnergyReady";

    private int energy;
    private DateTime energyReadyTime;
    private Coroutine countdownCoroutine;

    private void Start()
    {
        OnApplicationFocus(true);
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus) return;

        CancelInvoke();
        LoadHighScore();
        LoadEnergy();
    }

    private void LoadEnergy()
    {
        energy = PlayerPrefs.GetInt(EnergyKey, maxEnergy);

        if (energy == 0)
        {
            string energyReadyString = PlayerPrefs.GetString(EnergyReadyKey, string.Empty);

            if (energyReadyString == string.Empty) return;

            energyReadyTime = DateTime.Parse(energyReadyString);

            if (DateTime.Now > energyReadyTime)
            {
                energy = maxEnergy;
                PlayerPrefs.SetInt(EnergyKey, energy);
            }
            else
            {
                TimeSpan remainingTime = energyReadyTime - DateTime.Now;
                StartCountdown(remainingTime);
                rechargingText.gameObject.SetActive(true);
                playButton.interactable = false;
            }
        }

        UpdateEnergyText();
    }

    private void StartCountdown(TimeSpan remainingTime)
    {
        if (countdownCoroutine != null)
            StopCoroutine(countdownCoroutine);

        countdownCoroutine = StartCoroutine(CountdownCoroutine(remainingTime));
    }

    private System.Collections.IEnumerator CountdownCoroutine(TimeSpan remainingTime)
    {
        while (remainingTime.TotalSeconds > 0)
        {
            rechargingText.text = $"Recharging: {remainingTime.Seconds}s";
            yield return new WaitForSeconds(1f);
            remainingTime = energyReadyTime - DateTime.Now;
        }

        EnergyRecharged();
    }

    private void EnergyRecharged()
    {
        rechargingText.gameObject.SetActive(false);
        playButton.interactable = true;
        energy = maxEnergy;
        PlayerPrefs.SetInt(EnergyKey, energy);
        PlayerPrefs.SetString(EnergyReadyKey, string.Empty);
        UpdateEnergyText();
    }

    private void LoadHighScore()
    {
        int highScore = PlayerPrefs.GetInt(ScoreSystem.HighScoreKey, 0);
        highScoreText.text = $"High Score: {highScore}";
    }

    private void UpdateEnergyText()
    {
        energyText.text = $"Play ({energy})";
    }

    public void Play()
    {
        if (energy < 1) return;

        energy--;
        PlayerPrefs.SetInt(EnergyKey, energy);

        if (energy == 0)
        {
            energyReadyTime = DateTime.Now.AddMinutes(energyRechargeDuration);
            PlayerPrefs.SetString(EnergyReadyKey, energyReadyTime.ToString());

#if UNITY_ANDROID
            androidNotificationsHandler.ScheduleNotification(energyReadyTime);
#endif
        }

        SceneManager.LoadScene(1);
    }
}
