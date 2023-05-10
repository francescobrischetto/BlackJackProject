using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum PlayerState { NOTPLAYERTURN, ONEMORECARD, BUST, STOP }

public class PlayerController : MonoBehaviour
{
    public PlayerState State { get; private set; }
    public List<int> PlayerScore { get; private set; } = new List<int>();

    private VisualCardPositionController cardPositionController;

    public UnityEvent onPlayerStateChanged;

    public int RequestedScore = 17;
    public float RandomPercentage = 0.0f;

    private void OnEnable()
    {
        GameController.Instance.onRoundStateChange.AddListener(reactToRoundStateChanges);
        if(cardPositionController == null)
        {
            cardPositionController = transform.parent.GetComponentInChildren<VisualCardPositionController>();
        }

    }

    private void OnDisable()
    {
        GameController.Instance.onRoundStateChange.RemoveListener(reactToRoundStateChanges);
    }


    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Card" && State == PlayerState.ONEMORECARD && cardPositionController.IsThereSpace())
        {

            //Add that card to my table
            cardPositionController.setCardOnTable(collision.gameObject);
            //Update PlayerScore
            UpdatePlayerScore(collision.gameObject);
            //Destroy the card
            Destroy(collision.gameObject);
            //Check if we can change state
            int bestScore = BlackJackUtils.CalculateBestScore(PlayerScore);
            if(bestScore >= RequestedScore && bestScore < 21)
            {
                //above this X points the AI player has x % of wanting another card
                //So, if it is <= than 1-x% he will stop asking a card
                if (Random.value <= 1 - RandomPercentage)
                {
                    transform.GetComponent<MeshRenderer>().material.color = Color.yellow;
                    State = PlayerState.STOP;
                    onPlayerStateChanged.Invoke();
                }
            }
            else if(bestScore == 0)
            {
                transform.GetComponent<MeshRenderer>().material.color = Color.black;
                State = PlayerState.BUST;
                onPlayerStateChanged.Invoke();
            }
            else if(bestScore == 21)
            {
                transform.GetComponent<MeshRenderer>().material.color = Color.yellow;
                State = PlayerState.STOP;
                onPlayerStateChanged.Invoke();
            }

        }
    }

    private void UpdatePlayerScore(GameObject goCard)
    {
        
        Card card = DeckController.Instance.GetCardFromObject(goCard);
        if(card != null)
        {
            ReceiveDealerCard(card);
        }
        else
        {
            Debug.Log("WARNING! PLAYER RECEIVED A NULL CARD FROM DECK");
        }
        
    }
    private void ReceiveDealerCard(Card card)
    {
        List<int> newScores = BlackJackUtils.CalculateNewScores(PlayerScore, card);
        PlayerScore.Clear();
        PlayerScore.AddRange(newScores);
    }

    //TODO: Remove this visual display
    private IEnumerator CO_ReceivedCardDisplay()
    {
        Color initialColor = transform.GetComponent<MeshRenderer>().material.color;
        transform.GetComponent<MeshRenderer>().material.color = Color.red;
        yield return new WaitForSeconds(0.25f);
        transform.GetComponent<MeshRenderer>().material.color = initialColor;
    }


    private void ResetPlayer()
    {
        PlayerScore.Clear();
        PlayerScore.Add(0);
        State = PlayerState.NOTPLAYERTURN;
        //TODO: Remove
        transform.GetComponent<MeshRenderer>().material.color = Color.blue;
        cardPositionController.ResetSlots();
    }

    public void reactToRoundStateChanges(RoundState state)
    {
        switch (state)
        {
            case RoundState.START:
                ResetPlayer();
                break;

            case RoundState.PLAYERTURN:
                //Change player state
                //TODO: Remove
                transform.GetComponent<MeshRenderer>().material.color = Color.green;
                State = PlayerState.ONEMORECARD;
                break;

            case RoundState.DEALERTURN:
                Debug.Log("GO DEALER!");
                break;

            case RoundState.END:
                break;
        }
    }
}