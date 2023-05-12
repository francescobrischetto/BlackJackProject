using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using GD.MinMaxSlider;

/// <summary>
/// This class is responsible of controlling the whole blackjack game, changing round phases and determining who won each round.
/// </summary>
public class GameController : MonoBehaviour
{
    //Keep the state of the current round
    private RoundState roundState;
    //List of the controllers of the spawned players
    private List<PlayerController> playerInstances = new List<PlayerController>();

    //Singleton Instance
    public static GameController Instance { get; private set; }

    //Event that notifies when the round's state change
    public UnityEvent<RoundState> onRoundStateChange;
    //Event that notifies no more cards are available
    public UnityEvent onNoMoreCardsEvent;

    [field: Header("Player Spawning settings")]
    //Limited to max 7 players (worst case when the deck can potentially end!)
    [SerializeField] int maxPlayers = 7;
    //Number of players to spawn
    [SerializeField] int numberOfPlayers = 7;
    [SerializeField] List<GameObject> playerPositions;
    [SerializeField] List<GameObject> playerPrefabs;
    //Player behaviours (each parameter is determined randomly inside itss range)
    [MinMaxSlider(0, 21)]   
    [SerializeField] Vector2Int RequestedScore = new Vector2Int(0, 21);
    [MinMaxSlider(0, 1)]    //External plugin just for visual clue
    [SerializeField] Vector2 RandomPercentage = new Vector2(0f, 1f);

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

    private void Start()
    {
        GameStart();
    }

    private void SpawnPlayers()
    {
        List<int> playerIndexes = Enumerable.Range(0, maxPlayers).ToList();
        //Spawning the players in Place
        for (int i=0; i<numberOfPlayers; i++)
        {
            //Choosing a random place to spawn the player
            int randomPlayerIndex = Random.Range(0, playerIndexes.Count);
            int randomIndex = playerIndexes[randomPlayerIndex];
            int randomPlayerIndexToSpawn = Random.Range(0, playerPrefabs.Count);
            GameObject newPlayer = Instantiate(playerPrefabs[randomPlayerIndexToSpawn], 
                playerPositions[randomIndex].transform.position, Quaternion.identity, playerPositions[randomIndex].transform);
            newPlayer.transform.localRotation = Quaternion.identity;
            //No other players can spawn in this position
            playerIndexes.RemoveAt(randomPlayerIndex);
            //Setup player parameters
            newPlayer.name = "Player " + i;
            PlayerController newPlayerController = newPlayer.GetComponent<PlayerController>();
            //Setup card positioning controller script to react to player behaviours
            VisualCardPositionController visualCardPositionController = 
                playerPositions[randomIndex].transform.GetComponentInChildren<VisualCardPositionController>();
            newPlayerController.onCardReceived.AddListener(visualCardPositionController.reactToCardReceived);
            onRoundStateChange.AddListener(visualCardPositionController.reactToRoundChange);
            //Setup player name, percentage and threshold
            newPlayerController.playerName = newPlayer.name;
            newPlayerController.percentageToRequestAboveThreshold  = Random.Range(RandomPercentage.x, RandomPercentage.y);
            //Random range works differently for ints -> max Exclusive
            newPlayerController.thresholdScore = Random.Range(RequestedScore.x, RequestedScore.y + 1);
            //Subscribing to player event when they change state
            newPlayerController.onPlayerStateChanged.AddListener(PlayerStatusChanged);
            //Spawning the player panel on the GUI
            UIController.Instance.SpawnPlayerPanel(newPlayerController);
            //Adding the player to the list
            playerInstances.Add(newPlayerController);
        }
    }

    /// <summary>
    /// This function starts the whole game.
    /// </summary>
    private void GameStart()
    {
        SpawnPlayers();
        StartRound();
    }

    /// <summary>
    /// This function starts each new round.
    /// </summary>
    private void StartRound()
    {
        //Update round state and notify
        roundState = RoundState.START;
        onRoundStateChange.Invoke(roundState);

        //Next round state -> Started after a delay to be visible
        StartCoroutine(CO_StartPlayerTurn());
    }

    private void StartPlayerTurn()
    {
        //Update round state and notify
        roundState = RoundState.PLAYERTURN;
        onRoundStateChange.Invoke(roundState);
    }
    private void EndDealerTurn()
    {
        //Update round state and notify
        roundState = RoundState.END;
        onRoundStateChange.Invoke(roundState);
        //Calculating end game scores and detemining who wons the round
        EndGameCalculations();
        //Waiting some seconds to go to the new round
        StartCoroutine(CO_StartNewRound());
    }

    private bool IsPlayerTurnFinished()
    {
        foreach(PlayerController player in playerInstances)
        {
            if (player.State == PlayerState.ONEMORECARD)
            {
                return false;
            }
        }
        //No player is asking for cards, we can go to dealer turn
        return true;
    }

    private IEnumerator CO_StartNewRound()
    {
        yield return new WaitForSeconds(4.0f);
        StartRound();
    }

    private IEnumerator CO_StartPlayerTurn()
    {
        yield return new WaitForSeconds(2.0f);
        StartPlayerTurn();
    }


    /// <summary>
    /// This function calculates end game scores and determines who wons the round
    /// </summary>
    /// <returns></returns>
    private void EndGameCalculations()
    {
        int DealerBestScore = BlackJackUtils.CalculateBestScore(DealerController.Instance.DealerScore);
        for(int i=0; i<playerInstances.Count; i++)
        {
            int playerScore = BlackJackUtils.CalculateBestScore(playerInstances[i].PlayerScore);
            if (playerScore > DealerBestScore)
            {
                playerInstances[i].PlayerWon();
            }
            else
            {
                playerInstances[i].PlayerLost();
            }
        }
    }


    /// <summary>
    /// This function will listen any player when its status change
    /// </summary>
    public void PlayerStatusChanged(PlayerState playerState)
    {
        if (playerState != PlayerState.NOTPLAYERTURN && playerState != PlayerState.WON && playerState != PlayerState.LOST)
        {
            //If no player is asking for cards, we can go to dealer turn
            if (IsPlayerTurnFinished())
            {
                roundState = RoundState.DEALERTURN;
                onRoundStateChange.Invoke(roundState);
            }
        }
        
    }

    /// <summary>
    /// This function will listen any change in dealer state
    /// </summary>
    public void DealerStatusChanged(PlayerState dealerState)
    {
        switch (dealerState)
        {
            case PlayerState.NOTPLAYERTURN:
                break;
            case PlayerState.ONEMORECARD:
                break;
            case PlayerState.BUST:
                EndDealerTurn();
                break;
            case PlayerState.STOP:
                EndDealerTurn();
                break;
        }
    }

    /// <summary>
    /// This function is called when no more cards are avaiable -> The dealer will automatically lose and a new round will be started
    /// </summary>
    public void NoMoreCards()
    {
        //Update round state and notify
        roundState = RoundState.END;
        onRoundStateChange.Invoke(roundState);
        onNoMoreCardsEvent.Invoke();
        //All the players won
        for (int i = 0; i < playerInstances.Count; i++)
        {
            playerInstances[i].PlayerWon();
        }
        //Waiting some seconds to go to the new round
        StartCoroutine(CO_StartNewRound());
    }
}
