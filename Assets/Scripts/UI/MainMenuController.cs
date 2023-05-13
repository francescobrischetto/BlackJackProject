using GD.MinMaxSlider;
using Min_Max_Slider;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    //Game Settings -> default
    [field: Header("Player Spawning settings")]
    [SerializeField] int playerNumber = 4;
    [MinMaxSlider(0, 21)]
    [SerializeField] Vector2Int RequestedScore = new Vector2Int(15, 19);
    [MinMaxSlider(0, 1)]    //External plugin just for visual clue
    [SerializeField] Vector2 RandomPercentage = new Vector2(0.2f, 0.4f);

    [SerializeField] GameObject SettingsPanel;
    [SerializeField] GameObject CreditsPanel;
    [SerializeField] Slider PlayerSlider;
    [SerializeField] MinMaxSlider ThresholdSlider;
    [SerializeField] MinMaxSlider PercentageSlider;

    private void Awake()
    {
        SettingsPanel.SetActive(false);
        CreditsPanel.SetActive(false);
        PlayerSlider.value = playerNumber;
        PlayerSlider.onValueChanged.Invoke(PlayerSlider.value);
        ThresholdSlider.SetValues(RequestedScore.x, RequestedScore.y,true);
        PercentageSlider.SetValues(RandomPercentage.x*100, RandomPercentage.y*100, true);
    }

    public void ToggleSettings()
    {
        SettingsPanel.SetActive(!SettingsPanel.activeSelf);
    }

    public void ToggleCredits()
    {
        CreditsPanel.SetActive(!CreditsPanel.activeSelf);
    }

    public void playerNumberUpdatedNumber(float num)
    {
        playerNumber = (int)num;
    }

    public void playerThresholdUpdated(float min, float max)
    {
        RequestedScore.x = (int)min;
        RequestedScore.y = (int)max;
    }

    public void playerChanceUpdated(float min, float max)
    {
        RandomPercentage.x = min / 100;
        RandomPercentage.y = max / 100;
    }

    public void QuitGame()
    {
        #if UNITY_STANDALONE
                Application.Quit();
        #endif
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    public void StartGame()
    {
        PlayerPrefs.SetInt("playerNumber", playerNumber);
        PlayerPrefs.SetFloat("RequestedScore-x", RequestedScore.x);
        PlayerPrefs.SetFloat("RequestedScore-y", RequestedScore.y);
        PlayerPrefs.SetFloat("RandomPercentage-x", RandomPercentage.x);
        PlayerPrefs.SetFloat("RandomPercentage-y", RandomPercentage.y);
        SceneManager.LoadScene("GameScene");
    }
}
