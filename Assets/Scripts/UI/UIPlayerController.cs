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

    public void SetUIPlayerStatus(PlayerState playerState)
    {
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
