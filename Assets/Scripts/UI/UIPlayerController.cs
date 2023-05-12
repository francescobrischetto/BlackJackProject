using TMPro;
using UnityEngine;

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
    public void SetUIPlayerStatus(PlayerState playerState)
    {
        SetUIPlayerName(GetComponent<PlayerController>().playerName);
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
