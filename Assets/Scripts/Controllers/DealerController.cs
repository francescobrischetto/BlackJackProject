using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// This class is responsible of controlling the dealer.
/// </summary>
public class DealerController : MonoBehaviour
{
    //Dealer internal state
    private PlayerState dealerState;

    //Singleton Class
    public static DealerController Instance { get; private set; }
    //A list with all the possible scores of the dealer (considering that some cards may have more than one value E.G. Ace 1,11)
    public List<int> DealerScore { get; private set; } = new List<int>();
    //Event that notifies the change of dealer's state
    public UnityEvent<PlayerState> onDealerStateChanged;
    //Event that notifies the change of dealer's best score
    public UnityEvent<int> onScoreChanged;
    //Event that notifies when the dealer receives a card
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
    private void ResetDealerScore()
    {
        DealerScore.Clear();
        DealerScore.Add(0);
        onScoreChanged.Invoke(0);
    }
    private void ResetDealer()
    {
        ResetDealerScore();
        dealerState = PlayerState.NOTPLAYERTURN;
        onDealerStateChanged.Invoke(dealerState);
    }

    private void UpdateDealerScoreList(Card card)
    {
        List<int> newScores = BlackJackUtils.CalculateNewScores(DealerScore, card);
        DealerScore.Clear();
        DealerScore.AddRange(newScores);
    }

    /// <summary>
    /// This function will give a card to the dealer.
    /// </summary>
    /// <param name="card"></param>
    public void ReceiveCard(Card card)
    {
        if(dealerState == PlayerState.ONEMORECARD)
        {
            //Notify that the dealer received a card
            onCardReceived.Invoke(card.cardAsset);
            //Calculate new score list
            UpdateDealerScoreList(card);
            //Calculate new best score
            int bestScore = BlackJackUtils.CalculateBestScore(DealerScore);
            //Notify that the best score of the dealer is updated
            onScoreChanged.Invoke(bestScore);
            //Dealer score goes beyond 21, so the function returned 0
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

    //This function is called also from the UI with used input when the dealer decides to stop
    public void DealerStop()
    {
        //We need to check if the dealer was receiving cards (UI button will not work otherwise)
        if(dealerState == PlayerState.ONEMORECARD)
        {
            //Update dealer state and notify
            dealerState = PlayerState.STOP;
            onDealerStateChanged.Invoke(dealerState);
        }
        
    }

    /// <summary>
    /// This method allows the dealer to react to any change of the round's state.
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
