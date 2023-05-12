using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// This class is responsible of controlling a single player.
/// </summary>
public class PlayerController : MonoBehaviour
{
    //This object is responsible of placing the cards on the table visually
    private VisualCardPositionController cardPositionController;
    private AnimationController animationController;
    private UIPlayerController uiPlayerController;
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

    private void OnEnable()
    {
        //Reacting to Round changes
        GameController.Instance.onRoundStateChange.AddListener(reactToRoundStateChanges);
        if(cardPositionController == null)
        {
            //Getting the visual card position controller from sibilings
            cardPositionController = transform.parent.GetComponentInChildren<VisualCardPositionController>();
        }
        if (animationController == null)
        {
            animationController = GetComponent<AnimationController>();
        }
        if(uiPlayerController == null)
        {
            uiPlayerController = GetComponent<UIPlayerController>();
        }
    }

    private void OnDisable()
    {
        GameController.Instance.onRoundStateChange.RemoveListener(reactToRoundStateChanges);
    }


    private void OnCollisionEnter(Collision collision)
    {
        //Reacting to card collision only if there is space in board and the player is requesting cards
        if(collision.gameObject.tag == "Card" && State == PlayerState.ONEMORECARD && cardPositionController.IsThereSpace())
        {
            //Add that card to my table
            cardPositionController.setCardOnTable(collision.gameObject);
            ReceiveCard(collision.gameObject);
            //Destroy the card
            Destroy(collision.gameObject);
            //Check if we can change state
            int bestScore = BlackJackUtils.CalculateBestScore(PlayerScore);
            uiPlayerController.SetUIPlayerScore(bestScore);
            onScoreChanged.Invoke(bestScore);
            if (bestScore >= thresholdScore && bestScore < 21)
            {
                //above this thresholdScore the AI player has x % of wanting another card
                //So, if the percentage <= 1-x% he will stop asking a card because he is above the threshold
                if (Random.value <= 1 - percentageToRequestAboveThreshold)
                {
                    //Animation to stop asking cards
                    animationController.StopAskingCards();
                    //Update player state and notify
                    State = PlayerState.STOP;
                    uiPlayerController.SetUIPlayerStatus(State);
                    onPlayerStateChanged.Invoke(State);
                }
            }
            //my score goes beyond 21, so the function returned 0
            else if (bestScore == 0)
            {
                //Animation to tell that the player is bust
                animationController.PlayerIsBust();
                //Update player state and notify
                State = PlayerState.BUST;
                uiPlayerController.SetUIPlayerStatus(State);
                onPlayerStateChanged.Invoke(State);
            }
            //my score is exactly 21, so there is no point to continue. Stopping the player to request cards
            else if (bestScore == 21)
            {
                //Animation to stop asking cards
                animationController.StopAskingCards();
                //Update player state and notify
                State = PlayerState.STOP;
                uiPlayerController.SetUIPlayerStatus(State);
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
            UpdatePlayerScore(card);
        }
        else
        {
            //TODO: Trigger an event to handle?
            Debug.Log("WARNING! PLAYER RECEIVED A NULL CARD FROM DECK");
        }   
    }

    private void ResetPlayer()
    {
        ResetPlayerScore();
        uiPlayerController.SetUIPlayerScore(0);
        //Clearing the board visually
        cardPositionController.ResetSlots();
        State = PlayerState.NOTPLAYERTURN;
        onPlayerStateChanged.Invoke(State);
        uiPlayerController.SetUIPlayerName(playerName);
        uiPlayerController.SetUIPlayerStatus(State);
        //Resetting the Animation States
        animationController.resetAnimations();


    }

    private void ResetPlayerScore()
    {
        PlayerScore.Clear();
        PlayerScore.Add(0);
    }

    public void PlayerWon()
    {
        animationController.RoundEndWon();
        State = PlayerState.WON;
        onPlayerStateChanged.Invoke(State);
        uiPlayerController.SetUIPlayerStatus(State);
    }

    public void PlayerLost()
    {
        animationController.RoundEndLost();
        State = PlayerState.LOST;
        onPlayerStateChanged.Invoke(State);
        uiPlayerController.SetUIPlayerStatus(State);
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
                //Animation to keep asking cards
                animationController.AskCards();
                //The player will start to ask cards
                State = PlayerState.ONEMORECARD;
                onPlayerStateChanged.Invoke(State);
                uiPlayerController.SetUIPlayerStatus(State);
                break;

            case RoundState.DEALERTURN:
                break;

            case RoundState.END:
                break;
        }
    }
}