using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    private Animator animator;
    private int isAskingCardsHash = Animator.StringToHash("isAskingCards");
    private int isAskingToStopHash = Animator.StringToHash("isAskingToStop");
    private int isWinningHash = Animator.StringToHash("isWinning");
    private int isRoundEndHash = Animator.StringToHash("isRoundEnd");

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void resetAnimations()
    {
        if (animator != null)
        {
            animator.SetBool(isAskingCardsHash, false);
            animator.SetBool(isAskingToStopHash, false);
            animator.SetBool(isWinningHash, false);
            animator.SetBool(isRoundEndHash, false);
        }
    }

    public void AskCards()
    {
        if (animator != null)
        {
            animator.SetBool(isAskingCardsHash, true);
        }
    }

    public void StopAskingCards()
    {
        if (animator != null)
        {
            animator.SetBool(isAskingCardsHash, false);
            animator.SetBool(isAskingToStopHash, true);
        }
    }

    public void PlayerIsBust()
    {
        if (animator != null)
        {
            animator.SetBool(isAskingCardsHash, false);
            animator.SetBool(isWinningHash, false);
        }
    }

    public void RoundEndWon()
    {
        if (animator != null)
        {
            animator.SetBool(isAskingCardsHash, false);
            animator.SetBool(isWinningHash, true);
            animator.SetBool(isRoundEndHash, true);
        }
    }

    public void RoundEndLost()
    {
        if (animator != null)
        {
            animator.SetBool(isAskingCardsHash, false);
            animator.SetBool(isWinningHash, false);
            animator.SetBool(isRoundEndHash, true);
        }
    }


}
