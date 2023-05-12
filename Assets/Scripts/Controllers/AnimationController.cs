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
        GetComponent<PlayerController>().onPlayerStateChanged.AddListener(reactPlayerStatusChange);
    }

    private void resetAnimations()
    {
        if (animator != null)
        {
            animator.SetBool(isAskingCardsHash, false);
            animator.SetBool(isAskingToStopHash, false);
            animator.SetBool(isWinningHash, false);
            animator.SetBool(isRoundEndHash, false);
        }
    }

    private void AskCards()
    {
        if (animator != null)
        {
            animator.SetBool(isAskingCardsHash, true);
        }
    }

    private void StopAskingCards()
    {
        if (animator != null)
        {
            animator.SetBool(isAskingCardsHash, false);
            animator.SetBool(isAskingToStopHash, true);
        }
    }

    private void PlayerIsBust()
    {
        if (animator != null)
        {
            animator.SetBool(isAskingCardsHash, false);
            animator.SetBool(isWinningHash, false);
        }
    }

    private void RoundEndWon()
    {
        if (animator != null)
        {
            animator.SetBool(isAskingCardsHash, false);
            animator.SetBool(isWinningHash, true);
            animator.SetBool(isRoundEndHash, true);
        }
    }

    private void RoundEndLost()
    {
        if (animator != null)
        {
            animator.SetBool(isAskingCardsHash, false);
            animator.SetBool(isWinningHash, false);
            animator.SetBool(isRoundEndHash, true);
        }
    }

    public void reactPlayerStatusChange(PlayerState playerState)
    {
        switch (playerState)
        {
            case PlayerState.NOTPLAYERTURN:
                resetAnimations();
                break;
            case PlayerState.ONEMORECARD:
                AskCards();
                break;
            case PlayerState.BUST:
                PlayerIsBust();
                break;
            case PlayerState.STOP:
                StopAskingCards();
                break;
            case PlayerState.WON:
                RoundEndWon();
                break;
            case PlayerState.LOST:
                RoundEndLost();
                break;
        }
    }

}
