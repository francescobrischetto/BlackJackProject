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

    [SerializeField] UnityEvent onCardReceived;

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
        if(collision.gameObject.tag == "Card")
        {
            //Add that card to my table
            //Update PlayerScore
            UpdatePlayerScore(collision.gameObject);
            //Destroy the card
            Destroy(collision.gameObject);
            onCardReceived.Invoke();
        }
    }

    private void UpdatePlayerScore(GameObject goCard)
    {
        cardPositionController.setCardOnTable(goCard);
        Card card = DeckController.Instance.GetCardFromObject(goCard);
        if(card != null)
        {
            Debug.Log($"PLAYER Received {card.name} and value {card.values[0]}");
            ReceiveDealerCard(card);
        }
        else
        {
            Debug.Log("WARNING! PLAYER RECEIVED A NULL CARD FROM DECK");
        }
        
    }
    private void ReceiveDealerCard(Card card)
    {
        StartCoroutine(CO_ReceivedCardDisplay());
        List<int> newScores = BlackJackUtils.CalculateNewScores(PlayerScore, card);
        PlayerScore.Clear();
        PlayerScore.AddRange(newScores);
    }

    private IEnumerator CO_ReceivedCardDisplay()
    {
        transform.GetComponent<MeshRenderer>().material.color = Color.red;
        yield return new WaitForSeconds(0.5f);
        transform.GetComponent<MeshRenderer>().material.color = Color.white;
    }


    private void ResetPlayer()
    {
        PlayerScore.Clear();
        PlayerScore.Add(0);
    }

    public void reactToRoundStateChanges(RoundState state)
    {
        switch (state)
        {
            case RoundState.START:
                ResetPlayer();
                cardPositionController.ResetSlots();
                break;

            case RoundState.PLAYERTURN:
                break;

            case RoundState.DEALERTURN:
                Debug.Log("GO DEALER!");
                break;

            case RoundState.END:
                Debug.Log("Goodbye from player!");
                break;
        }
    }
}