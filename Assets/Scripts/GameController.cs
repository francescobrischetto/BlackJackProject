using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using GD.MinMaxSlider;

public struct PlayerInfo
{
    public string Name;
    public int Score;
    public bool Won;

}

//This struct will be generated at the end of the round and will be passed to the UI to display the round informations
public struct GameState
{
    public int DealerScore;
    public List<PlayerInfo> PlayerInfos;
}
public enum RoundState { START, PLAYERTURN, DEALERTURN, END} 

public class GameController : MonoBehaviour
{
    
    //Game Controller keeps the state of the current round
    private RoundState roundState;
    private List<PlayerController> playerInstances = new List<PlayerController>();

    //Observer pattern to notify other objects when the round state change
    public UnityEvent<RoundState> onRoundStateChange;

    [field: Header("Player Spawning settings")]
    //Players settings (limited to max 7 players (worst case when the deck can potentially end!)
    [SerializeField] int maxPlayers = 7;
    [SerializeField] int numberOfPlayers = 7;
    [SerializeField] List<GameObject> playerPositions;
    [SerializeField] List<GameObject> playerPrefabs;
    [MinMaxSlider(0, 21)]
    [SerializeField] Vector2Int RequestedScore = new Vector2Int(0, 21);
    [MinMaxSlider(0, 1)]
    [SerializeField] Vector2 RandomPercentage = new Vector2(0f, 1f);

    //Singleton Class
    public static GameController Instance { get; private set; }

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
            int randomPlayerIndex = Random.Range(0, playerIndexes.Count);
            int randomIndex = playerIndexes[randomPlayerIndex];
            GameObject newPlayer = Instantiate(playerPrefabs[0], playerPositions[randomIndex].transform.position, Quaternion.identity, playerPositions[randomIndex].transform);
            playerIndexes.RemoveAt(randomPlayerIndex);
            newPlayer.name = "Player " + i;
            PlayerController newPlayerController = newPlayer.GetComponent<PlayerController>();
            newPlayerController.RandomPercentage = Random.Range(RandomPercentage.x, RandomPercentage.y);
            newPlayerController.RequestedScore = Random.Range(RequestedScore.x, RequestedScore.y+1);
            newPlayerController.onPlayerStateChanged.AddListener(PlayerStatusChanged);
            playerInstances.Add(newPlayerController);
            Debug.Log($"Spawned player {newPlayer.name} with requested score {newPlayerController.RequestedScore} and random percentage {newPlayerController.RandomPercentage}");

        }
    }

    private void GameStart()
    {
        SpawnPlayers();
        StartRound();
    }
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

    private bool CheckIfPlayersTurnIsFinished()
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

    //Coroutine that ends the current round and starts a new one, after some delay and UI adjustments
    private IEnumerator CO_StartNewRound()
    {
        yield return new WaitForSeconds(3.0f);
        //Update UI
        yield return new WaitForSeconds(2.0f);
        StartRound();
    }

    //This function constructs the game states, storing who wons the current round
    private GameState checkGameState()
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


    //This function will listen any player when its status change
    public void PlayerStatusChanged()
    {
        //If no player is asking for cards, we can go to dealer turn
        if (CheckIfPlayersTurnIsFinished())
        {
            roundState = RoundState.DEALERTURN;
            onRoundStateChange.Invoke(roundState);
        }
    }

    //TODO: remove in final build
    public void FinishPlayerTurn()
    {
        roundState = RoundState.DEALERTURN;
        onRoundStateChange.Invoke(roundState);
    }

    
    //TODO: React to dealer changes
    //This function will be called by the UI or if dealer score > 21 to go to the end state of the round
    public void EndDealerTurn()
    {
        //Update round state and notify
        roundState = RoundState.END;
        onRoundStateChange.Invoke(roundState);
        //Checking winning conditions
        GameState gameState = checkGameState();
        Debug.Log("GAME FINISHED!");
        Debug.Log("GAME STATE:");
        Debug.Log($"Dealer Score: {gameState.DealerScore}");
        foreach(PlayerInfo playerInfo in gameState.PlayerInfos)
        {
            Debug.Log("---PlayerINFO");
            Debug.Log($"Player Name: {playerInfo.Name}");
            Debug.Log($"Player Score: {playerInfo.Score}");
            Debug.Log($"Player Won?: {playerInfo.Won}");
        }
        //Passing gameState to UI for displaying
        //Waiting some seconds
        StartCoroutine(CO_StartNewRound());   
    }

}
