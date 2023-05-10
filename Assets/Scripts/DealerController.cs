using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DealerController : MonoBehaviour
{
    //Singleton Class
    public static DealerController Instance { get; private set; }
    public List<int> DealerScore { get; private set; } = new List<int>();
    [SerializeField] VisualCardPositionController cardPositionController;

    public PlayerState DealerState { get; private set; }
    public UnityEvent onDealerStateChanged;


    private void Awake()
    {
        //Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void ResetDealer()
    {
        ResetDealerScore();
        ClearDealerCardPosition();
        DealerState = PlayerState.NOTPLAYERTURN;
    }

    private void ClearDealerCardPosition()
    {
        cardPositionController.ResetSlots();
    }

    private void ResetDealerScore()
    {
        DealerScore.Clear();
        DealerScore.Add(0);
    }

    public void ReceiveDealerCard(Card card)
    {
        if(DealerState == PlayerState.ONEMORECARD)
        {
            cardPositionController.setCardOnTable(card.cardAsset);
            UpdateDealerScore(card);
            int bestScore = BlackJackUtils.CalculateBestScore(DealerScore);
            if (bestScore == 0)
            {
                DealerState = PlayerState.BUST;
                onDealerStateChanged.Invoke();
            }
            else if (bestScore == 21)
            {
                DealerState = PlayerState.STOP;
                onDealerStateChanged.Invoke();
            }
        }
        
    }

    public void DealerStop()
    {
        DealerState = PlayerState.STOP;
        onDealerStateChanged.Invoke();
    }

    private void UpdateDealerScore(Card card)
    {
        List<int> newScores = BlackJackUtils.CalculateNewScores(DealerScore, card);
        DealerScore.Clear();
        DealerScore.AddRange(newScores);
    }

    public void reactToRoundStateChanges(RoundState state)
    {
        switch (state)
        {
            case RoundState.START:
                ResetDealer();
                break;

            case RoundState.PLAYERTURN:
                break;

            case RoundState.DEALERTURN:
                DealerState = PlayerState.ONEMORECARD;
                break;

            case RoundState.END:
                break;
        }
    }
}
