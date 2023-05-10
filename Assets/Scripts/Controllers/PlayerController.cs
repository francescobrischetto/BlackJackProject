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
    
    public PlayerState State { get; private set; }
    //A list with all the possible scores of the player (considering that some cards may have more than one value E.G. Ace 1,11)
    public List<int> PlayerScore { get; private set; } = new List<int>();

    [field: Header("Player Behaviour settings")]
    [SerializeField] int thresholdScore = 17;
    [SerializeField] float percentageToRequestAboveThreshold = 0.0f;
    //Observer pattern to notify other objects when the player state change
    public UnityEvent onPlayerStateChanged;

    private void OnEnable()
    {
        //Reacting to Round changes
        GameController.Instance.onRoundStateChange.AddListener(reactToRoundStateChanges);
        if(cardPositionController == null)
        {
            //Getting the visual card position controller from sibilings
            cardPositionController = transform.parent.GetComponentInChildren<VisualCardPositionController>();
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
            if(bestScore >= thresholdScore && bestScore < 21)
            {
                //above this thresholdScore the AI player has x % of wanting another card
                //So, if the percentage <= 1-x% he will stop asking a card because he is above the threshold
                if (Random.value <= 1 - percentageToRequestAboveThreshold)
                {
                    //TODO: Remove Mesh Rendered visual reacting!
                    transform.GetComponent<MeshRenderer>().material.color = Color.yellow;
                    //Update player state and notify
                    State = PlayerState.STOP;
                    onPlayerStateChanged.Invoke();
                }
            }
            //my score goes beyond 21, so the function returned 0
            else if (bestScore == 0)
            {
                //TODO: Remove Mesh Rendered visual reacting!
                transform.GetComponent<MeshRenderer>().material.color = Color.black;
                //Update player state and notify
                State = PlayerState.BUST;
                onPlayerStateChanged.Invoke();
            }
            //my score is exactly 21, so there is no point to continue. Stopping the player to request cards
            else if (bestScore == 21)
            {
                //TODO: Remove Mesh Rendered visual reacting!
                transform.GetComponent<MeshRenderer>().material.color = Color.yellow;
                //Update player state and notify
                State = PlayerState.STOP;
                onPlayerStateChanged.Invoke();
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
        //Clearing the board visually
        cardPositionController.ResetSlots();
        State = PlayerState.NOTPLAYERTURN;
        //TODO: Remove Mesh Rendered visual reacting!
        transform.GetComponent<MeshRenderer>().material.color = Color.blue;
        
    }

    private void ResetPlayerScore()
    {
        PlayerScore.Clear();
        PlayerScore.Add(0);
    }
    public void setThresholdScore(int newThreshold)
    {
        thresholdScore = newThreshold;
    }

    public void setPercentageToRequest(float newPercentage)
    {
        percentageToRequestAboveThreshold = newPercentage;
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
                //TODO: Remove Mesh Rendered visual reacting!
                transform.GetComponent<MeshRenderer>().material.color = Color.green;
                //The player will start to ask cards
                State = PlayerState.ONEMORECARD;
                break;

            case RoundState.DEALERTURN:
                break;

            case RoundState.END:
                break;
        }
    }
}