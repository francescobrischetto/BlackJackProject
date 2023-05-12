using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

public class PlayerInfoPanelDisplayController : MonoBehaviour
{
    [SerializeField] TMP_Text PlayerName;
    [SerializeField] TMP_Text Status;
    [SerializeField] TMP_Text Threshold;
    [SerializeField] TMP_Text Percentage;
    [SerializeField] TMP_Text Score;

    public PlayerController PlayerController { private get; set; }

    public void ScoreUpdated(int newScore)
    {
        Score.text = newScore.ToString();
    }
    public void PlayerStatusChanged(PlayerState playerState)
    {
        Status.text = UIUtils.GetStatusString(playerState);
    }

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
