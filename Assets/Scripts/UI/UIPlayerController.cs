using TMPro;
using UnityEngine;

public class UIPlayerController : MonoBehaviour
{
    [SerializeField]
    private TMP_Text score;
    [SerializeField]
    private TMP_Text status;

    public void SetUIPlayerStatus(PlayerStateString playerState)
    {
        status.text = UIUtils.GetStatusString(playerState);
    }

    public void SetUIPlayerScore(int scoreNum)
    {
        score.text = $"Score: {scoreNum.ToString()}";
    }
}
