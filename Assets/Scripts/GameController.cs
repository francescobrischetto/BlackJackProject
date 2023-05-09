using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public enum RoundState { START, PLAYERTURN, DEALERTURN, END} 
public class GameController : MonoBehaviour
{
    private RoundState roundState;
    //Observer pattern to notify other objects when the round state change
    [SerializeField] UnityEvent<RoundState> onRoundStateChange;
    [SerializeField] int numberOfPlayers = 7;
    [SerializeField] List<GameObject> playerPositions;
    public static GameController Instance { get; private set; }
    private List<PlayerController> playerInstances;


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

    // Start is called before the first frame update
    void Start()
    {
        GameStart();
    }

    void SpawnPlayers()
    {
        //Spawning the players in Place
    }

    void GameStart()
    {
        SpawnPlayers();
        StartRound();
    }
    void StartRound()
    {
        //TODO FIX
        roundState = RoundState.PLAYERTURN;
        onRoundStateChange.Invoke(roundState);
        //Initialization when starting a new Round
        DealerScore.Clear();    
        DealerScore.Add(0);
        ClearDealerCardPosition();
        StartPlayerTurn();
    }

    void ClearDealerCardPosition()
    {
        //Clearing dealer cards
    }

    void StartPlayerTurn()
    {
        roundState = RoundState.PLAYERTURN;
        onRoundStateChange.Invoke(roundState);
    }

    bool CheckIfPlayersTurnIsFinished()
    {
        foreach(PlayerController player in playerInstances)
        {
            if (player.State == PlayerState.ONEMORECARD)
            {
                return false;
            }
        }
        return true;
    }

    public void PlayerStatusChanged()
    {
        if (CheckIfPlayersTurnIsFinished())
        {
            roundState = RoundState.DEALERTURN;
            onRoundStateChange.Invoke(roundState);
        }
    }

    public void ReceiveDealerCard(Card card)
    {
        for(int i = 0; i < DealerScore.Count; i++)
        {
            DealerScore[i] = DealerScore[i] + card.values[0];
        }
        //I received an Ace
        if (card.values.Count > 1)
        {
            List<int> dealerScoreCopy = DealerScore.GetRange(0, DealerScore.Count);
            for (int i = 0; i < dealerScoreCopy.Count; i++)
            {
                dealerScoreCopy[i] = dealerScoreCopy[i] - card.values[0] + card.values[1];
            }
            DealerScore.AddRange(dealerScoreCopy);
        }
    }

    public void EndDealerTurn()
    {
        roundState = RoundState.END;
        onRoundStateChange.Invoke(roundState);
        //Checking winning conditions
        //Waiting some seconds
        StartRound();
    }
}
