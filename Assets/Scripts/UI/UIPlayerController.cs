using TMPro;
using UnityEngine;

/// <summary>
/// This class is responsible of controlling the World Space UI of the player.
/// </summary>
public class UIPlayerController : MonoBehaviour
{
    [SerializeField]
    private TMP_Text score;
    [SerializeField]
    private TMP_Text status;
    [SerializeField]
    private TMP_Text playerName;

    private void Awake()
    {
        PlayerController playerController = GetComponent<PlayerController>();
        playerController.onPlayerStateChanged.AddListener(SetUIPlayerStatus);
        playerController.onScoreChanged.AddListener(SetUIPlayerScore);
        

    }
    public void SetUIPlayerStatus(PlayerState playerState, string playerName)
    {
        //Name is set when the status change because (when the players spawn) it has no name. This is a workaround solution.
        SetUIPlayerName(playerName);
        status.text = UIUtils.GetStatusString(playerState);
    }

    public void SetUIPlayerScore(int scoreNum)
    {
        score.text = $"Score: {scoreNum.ToString()}";
    }

    public void SetUIPlayerName(string pName)
    {
        playerName.text = pName;
    }
}
