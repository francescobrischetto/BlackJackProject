using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// This class is responsible of controlling a single player.
/// </summary>
public class PlayerController : MonoBehaviour
{
    public PlayerState State { get; private set; }
    //A list with all the possible scores of the player (considering that some cards may have more than one value E.G. Ace 1,11)
    public List<int> PlayerScore { get; private set; } = new List<int>();

    [field: Header("Player Behaviour settings")]
    public int thresholdScore = 17;
    public float percentageToRequestAboveThreshold = 0.0f;
    public string playerName;
    //Observer pattern to notify other objects when the player state change
    public UnityEvent<PlayerState> onPlayerStateChanged;
    public UnityEvent<int> onScoreChanged;
    public UnityEvent<GameObject> onCardReceived;

    private void OnEnable()
    {
        //Reacting to Round changes
        GameController.Instance.onRoundStateChange.AddListener(reactToRoundStateChanges);
    }

    private void OnDisable()
    {
        GameController.Instance.onRoundStateChange.RemoveListener(reactToRoundStateChanges);
    }


    private void OnCollisionEnter(Collision collision)
    {
        //Reacting to card collision only if the player is requesting cards
        if(collision.gameObject.tag == "Card" && State == PlayerState.ONEMORECARD)
        {
            ReceiveCard(collision.gameObject);
            //Check if we can change state
            int bestScore = BlackJackUtils.CalculateBestScore(PlayerScore);
            onScoreChanged.Invoke(bestScore);
            if (bestScore >= thresholdScore && bestScore < 21)
            {
                //above this thresholdScore the AI player has x % of wanting another card
                //So, if the percentage <= 1-x% he will stop asking a card because he is above the threshold
                if (Random.value <= 1 - percentageToRequestAboveThreshold)
                {
                    //Update player state and notify
                    State = PlayerState.STOP;
                    onPlayerStateChanged.Invoke(State);
                }
            }
            //my score goes beyond 21, so the function returned 0
            else if (bestScore == 0)
            {
                //Update player state and notify
                State = PlayerState.BUST;
                onPlayerStateChanged.Invoke(State);
            }
            //my score is exactly 21, so there is no point to continue. Stopping the player to request cards
            else if (bestScore == 21)
            {
                //Update player state and notify
                State = PlayerState.STOP;
                onPlayerStateChanged.Invoke(State);
            }

        }
    }

    private void UpdatePlayerScore(Card card)
    {
        List<int> newScores = BlackJackUtils.CalculateNewScores(PlayerScore, card);
        PlayerScore.Clear();
        PlayerScore.AddRange(newScores);
    }
    private void ReceiveCard(GameObject goCard)
    {
        //We need the Scriptable Object "Card" but we only have its game object
        Card card = DeckController.Instance.GetCardFromObject(goCard);
        if (card != null)
        {
            onCardReceived.Invoke(goCard);
            UpdatePlayerScore(card);
        }
        //Destroy the card
        Destroy(goCard);
    }

    private void ResetPlayer()
    {
        ResetPlayerScore();
        State = PlayerState.NOTPLAYERTURN;
        onPlayerStateChanged.Invoke(State);


    }

    private void AskCards()
    {
        State = PlayerState.ONEMORECARD;
        onPlayerStateChanged.Invoke(State);
    }

    private void ResetPlayerScore()
    {
        PlayerScore.Clear();
        PlayerScore.Add(0);
        onScoreChanged.Invoke(0);
    }

    public void PlayerWon()
    {
        State = PlayerState.WON;
        onPlayerStateChanged.Invoke(State);
    }

    public void PlayerLost()
    {
        State = PlayerState.LOST;
        onPlayerStateChanged.Invoke(State);
    }

    /// <summary>
    /// This method allows the player to react to any state change of the round
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