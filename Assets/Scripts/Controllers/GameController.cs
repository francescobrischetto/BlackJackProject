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

    //Observer pattern to notify other objects when the round state change
    public UnityEvent<RoundState> onRoundStateChange;

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

    //TODO: Remove the game start on start
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
            GameObject newPlayer = Instantiate(playerPrefabs[0], playerPositions[randomIndex].transform.position, Quaternion.identity, playerPositions[randomIndex].transform);
            playerIndexes.RemoveAt(randomPlayerIndex);
            //Setup player name and parameters
            newPlayer.name = "Player " + i;
            PlayerController newPlayerController = newPlayer.GetComponent<PlayerController>();
            newPlayerController.setPercentageToRequest(Random.Range(RandomPercentage.x, RandomPercentage.y));
            //Random range works differently for ints -> max Exclusive
            newPlayerController.setThresholdScore(Random.Range(RequestedScore.x, RequestedScore.y + 1));
            //Subscribing to player event when they change state
            newPlayerController.onPlayerStateChanged.AddListener(PlayerStatusChanged);
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

        //Next round state
        StartPlayerTurn();
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
        //Create game state structure to store who won the current round
        GameState gameState = CreateGameStateStruct();
        //Passing gameState to UI for displaying
        //TODO: Fix
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

    /// <summary>
    /// Coroutine that ends the current round and starts a new one, after some delay and UI adjustments
    /// </summary>
    /// <returns></returns>
    private IEnumerator CO_StartNewRound()
    {
        yield return new WaitForSeconds(3.0f);
        //Update UI
        yield return new WaitForSeconds(2.0f);
        StartRound();
    }

    /// <summary>
    /// This function constructs populate the game state structure, storing who wons the current round
    /// </summary>
    /// <returns></returns>
    private GameState CreateGameStateStruct()
    {
        GameState gameState = new GameState();
        gameState.DealerScore = BlackJackUtils.CalculateBestScore(DealerController.Instance.DealerScore);
        gameState.PlayerInfos = new List<PlayerInfo>();
        for(int i=0; i<playerInstances.Count; i++)
        {
            PlayerInfo playerInfo = new PlayerInfo();
            playerInfo.Name = "Player " + (i + 1);
            playerInfo.Score = BlackJackUtils.CalculateBestScore(playerInstances[i].PlayerScore);
            playerInfo.Won = playerInfo.Score > gameState.DealerScore;
            gameState.PlayerInfos.Add(playerInfo);
        }
        return gameState;
    }


    /// <summary>
    /// This function will listen any player when its status change
    /// </summary>
    public void PlayerStatusChanged()
    {
        //If no player is asking for cards, we can go to dealer turn
        if (IsPlayerTurnFinished())
        {
            roundState = RoundState.DEALERTURN;
            onRoundStateChange.Invoke(roundState);
        }
    }

    /// <summary>
    /// This function will listen any change in dealer status
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

}
