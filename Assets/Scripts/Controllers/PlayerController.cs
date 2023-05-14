using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// This class is responsible of controlling a single player.
/// </summary>
public class PlayerController : MonoBehaviour
{
    //Player internal state
    private PlayerState state;
    //A list with all the possible scores of the player (considering that some cards may have more than one value E.G. Ace 1,11)
    public List<int> PlayerScore { get; private set; } = new List<int>();

    [field: Header("Player Behaviour settings")]
    public string playerName;
    public int thresholdScore = 17;
    public float percentageToRequestAboveThreshold = 0.0f;

    //Event that notifies the change of player's state
    public UnityEvent<PlayerState,string> onPlayerStateChanged;
    //Event that notifies the change of player's best score
    public UnityEvent<int> onScoreChanged;
    //Event that notifies when the player receives a card
    public UnityEvent<GameObject> onCardReceived;
    
    private void OnCollisionEnter(Collision collision)
    {
        //Reacting to card collision only if the player is requesting cards
        if(collision.gameObject.tag == "Card" && state == PlayerState.ONEMORECARD)
        {
            //Handling the card receiving
            ReceiveCard(collision.gameObject);
            //Calculating the new best score and notify
            int bestScore = BlackJackUtils.CalculateBestScore(PlayerScore);
            onScoreChanged.Invoke(bestScore);
            //Checking if the player can change state
            if (bestScore >= thresholdScore && bestScore < 21)
            {
                //above this thresholdScore the AI player has x % of wanting another card
                //So, if the percentage <= 1-x% he will stop asking a card because he is above the threshold
                if (Random.value <= 1 - percentageToRequestAboveThreshold)
                {
                    //Update player state and notify
                    state = PlayerState.STOP;
                    onPlayerStateChanged.Invoke(state,playerName);
                }
            }
            //Player score goes beyond 21, so the function returned 0
            else if (bestScore == 0)
            {
                //Update player state and notify
                state = PlayerState.BUST;
                onPlayerStateChanged.Invoke(state, playerName);
            }
            //Player score is exactly 21, so there is no point to continue. Stopping the player to request cards
            else if (bestScore == 21)
            {
                //Update player state and notify
                state = PlayerState.STOP;
                onPlayerStateChanged.Invoke(state, playerName);
            }
        }
    }

    private void UpdatePlayerScoreList(Card card)
    {
        List<int> newScores = BlackJackUtils.CalculateNewScores(PlayerScore, card);
        PlayerScore.Clear();
        PlayerScore.AddRange(newScores);
    }

    //Handling the card Reception
    private void ReceiveCard(GameObject goCard)
    {
        //We need the Scriptable Object "Card" but we only have its game object, we need to ask to the DeckController
        Card card = DeckController.Instance.GetCardFromObject(goCard);
        if (card != null)
        {
            //Notify the card reception
            onCardReceived.Invoke(goCard);
            UpdatePlayerScoreList(card);
        }
        //Destroy the card
        Destroy(goCard);
    }

    private void ResetPlayerScore()
    {
        PlayerScore.Clear();
        PlayerScore.Add(0);
        //Notify the resetting score
        onScoreChanged.Invoke(0);
    }

    private void ResetPlayer()
    {
        ResetPlayerScore();
        //Update player state and notify
        state = PlayerState.NOTPLAYERTURN;
        onPlayerStateChanged.Invoke(state, playerName);


    }

    private void AskCards()
    {
        //Update player state and notify
        state = PlayerState.ONEMORECARD;
        onPlayerStateChanged.Invoke(state, playerName);
    }

    public void PlayerWon()
    {
        //Update player state and notify
        state = PlayerState.WON;
        onPlayerStateChanged.Invoke(state, playerName);
    }

    public void PlayerLost()
    {
        //Update player state and notify
        state = PlayerState.LOST;
        onPlayerStateChanged.Invoke(state, playerName);
    }

    /// <summary>
    /// This method allows the player to react to any change of the round's state
    /// </summary>
    /// <param name="state"></param>
    public void reactToRoundStateChanges(RoundState state)
    {
        switch (state)
        {
            case RoundState.START:
                ResetPlayer();
                break;

            case RoundState.PLAYERTURN:
                AskCards();
                break;

            case RoundState.DEALERTURN:
                break;

            case RoundState.END:
                break;
        }
    }
}