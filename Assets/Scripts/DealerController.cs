using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DealerController : MonoBehaviour
{
    //Singleton Class
    public static DealerController Instance { get; private set; }
    public List<int> DealerScore { get; private set; } = new List<int>();
    [SerializeField] List<GameObject> dealerCardsPosition;
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

    public void ResetDealer()
    {
        ResetDealerScore();
        ClearDealerCardPosition();
    }

    private void ClearDealerCardPosition()
    {
        //Clearing dealer cards
    }

    private void ResetDealerScore()
    {
        DealerScore.Clear();
        DealerScore.Add(0);
    }

    public void ReceiveDealerCard(Card card)
    {
        Debug.Log($"DEALER Received {card.name} and value {card.values[0]}");
        List<int> newScores = BlackJackUtils.CalculateNewScores(DealerScore, card);
        DealerScore.Clear();
        DealerScore.AddRange(newScores);
    }
}
