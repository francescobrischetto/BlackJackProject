using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum PlayerState { NOTPLAYERTURN, ONEMORECARD, BUST, STOP }

public class PlayerController : MonoBehaviour
{
    public PlayerState State { get; private set; }
    public List<int> PlayerScore { get; private set; } = new List<int>();


    [SerializeField] UnityEvent onCardReceived;

    private void Start()
    {
        PlayerScore.Clear();
        PlayerScore.Add(0);

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
        Card card = Deck.Instance.GetCardFromObject(goCard);
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
        for (int i = 0; i < PlayerScore.Count; i++)
        {
            PlayerScore[i] = PlayerScore[i] + card.values[0];
        }
        //I received an Ace
        if (card.values.Count > 1)
        {
            List<int> playerScoreCopy = PlayerScore.GetRange(0, PlayerScore.Count);
            for (int i = 0; i < playerScoreCopy.Count; i++)
            {
                playerScoreCopy[i] = playerScoreCopy[i] - card.values[0] + card.values[1];
            }
            PlayerScore.AddRange(playerScoreCopy);
        }

        foreach(int score in PlayerScore)
        {
            Debug.Log($"{transform.gameObject.name} Player. Score Updated: {score}");
        }
    }

}