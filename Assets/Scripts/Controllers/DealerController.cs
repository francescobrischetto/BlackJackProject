using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// This class is responsible of controlling the dealer.
/// </summary>
public class DealerController : MonoBehaviour
{
    private PlayerState dealerState;

    //Singleton Class
    public static DealerController Instance { get; private set; }
    //A list with all the possible scores of the dealer (considering that some cards may have more than one value E.G. Ace 1,11)
    public List<int> DealerScore { get; private set; } = new List<int>();
    //Observer pattern to notify other objects when the dealer state change
    public UnityEvent<PlayerState> onDealerStateChanged;
    public UnityEvent<int> onScoreChanged;
    public UnityEvent<GameObject> onCardReceived;

    private void Awake()
    {
        //Singleton Setup
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
        dealerState = PlayerState.NOTPLAYERTURN;
    }

    private void ResetDealerScore()
    {
        DealerScore.Clear();
        DealerScore.Add(0);
        onScoreChanged.Invoke(0);
    }
    private void UpdateDealerScore(Card card)
    {
        List<int> newScores = BlackJackUtils.CalculateNewScores(DealerScore, card);
        DealerScore.Clear();
        DealerScore.AddRange(newScores);
    }

    public void ReceiveCard(Card card)
    {
        if(dealerState == PlayerState.ONEMORECARD)
        {
            //visually display the card on the table
            onCardReceived.Invoke(card.cardAsset);
            //calculate new score
            UpdateDealerScore(card);
            int bestScore = BlackJackUtils.CalculateBestScore(DealerScore);
            onScoreChanged.Invoke(bestScore);
            //my score goes beyond 21, so the function returned 0
            if (bestScore == 0)
            {
                //Update dealer state and notify
                dealerState = PlayerState.BUST;
                onDealerStateChanged.Invoke(dealerState);
            }
            //my score is exactly 21, so there is no point to continue. Stopping the dealer
            else if (bestScore == 21)
            {
                DealerStop();
            }
        }
        
    }

    //This function is called also from the UI, when the dealer decides to stop
    public void DealerStop()
    {
        if(dealerState == PlayerState.ONEMORECARD)
        {
            //Update dealer state and notify
            dealerState = PlayerState.STOP;
            onDealerStateChanged.Invoke(dealerState);
        }
        
    }


    /// <summary>
    /// This method allows the dealer to react to any state change of the round
    /// </summary>
    /// <param name="state"></param>
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
                dealerState = PlayerState.ONEMORECARD;
                break;

            case RoundState.END:
                break;
        }
    }
}
