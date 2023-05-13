using System;
using TMPro;
using UnityEngine;

/// <summary>
/// This class is responsible of controlling the screen space panel of the player on the UI
/// </summary>
public class PlayerInfoPanelDisplayController : MonoBehaviour
{
    [SerializeField] TMP_Text PlayerName;
    [SerializeField] TMP_Text Status;
    [SerializeField] TMP_Text Threshold;
    [SerializeField] TMP_Text Percentage;
    [SerializeField] TMP_Text Score;

    //The public setter is necessary. The right player to listen is setted when this gameObject is spawned.
    public PlayerController PlayerController { private get; set; }

    /// <summary>
    /// This method allows the class to react to any state change of the player's score
    /// </summary>
    /// <param name="newScore"></param>
    public void ScoreUpdated(int newScore)
    {
        Score.text = newScore.ToString();
    }

    /// <summary>
    /// This method allows the class to react to any state change of the player's state
    /// </summary>
    /// <param name="playerState"></param>
    public void PlayerStatusChanged(PlayerState playerState, string playerName)
    {
        Status.text = UIUtils.GetStatusString(playerState);
    }

    /// <summary>
    /// This method will setup the component and its initial values
    /// </summary>
    public void SetupPlayerInfo()
    {
        PlayerController.onPlayerStateChanged.AddListener(PlayerStatusChanged);
        PlayerController.onScoreChanged.AddListener(ScoreUpdated);
        PlayerName.text = PlayerController.playerName;
        Threshold.text = PlayerController.thresholdScore.ToString();
        Percentage.text = ((int)Math.Ceiling(PlayerController.percentageToRequestAboveThreshold * 100)).ToString();
        ScoreUpdated(0);

    }

}
