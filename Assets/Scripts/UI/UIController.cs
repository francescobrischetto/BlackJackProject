using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// This class is responsible of controlling the screen space in-game UI.
/// </summary>
public class UIController : MonoBehaviour
{
    [SerializeField] Button StopDealer;
    [SerializeField] Button ShuffleDeck;
    [SerializeField] Button UserListInfo;
    [SerializeField] GameObject UserListInfoPanel;
    [SerializeField] GameObject PauseMenuPanel;

    [field: Header("PlayerInfo Panel spawning settings")]
    [SerializeField] GameObject PlayerInfoGridContentPanel;
    [SerializeField] GameObject PlayerInfoPanel;

    [SerializeField] TMP_Text RoundPhase;
    [SerializeField] TMP_Text DealerScore;

    public UnityEvent onStopDealerClick;
    public UnityEvent onShuffleDeckClick;

    //Singleton Instance
    public static UIController Instance { get; private set; }


    private void Awake()
    {
        //Closing the panels on awake
        UserListInfoPanel.SetActive(false);
        PauseMenuPanel.SetActive(false);
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

    //This method spawn a dedicated player panel for each player in the info panel. It will listen to the player change in state.
    public void SpawnPlayerPanel(PlayerController player)
    {
        GameObject spawnedPlayerPanel = Instantiate(PlayerInfoPanel, Vector3.zero, Quaternion.identity, PlayerInfoGridContentPanel.transform);
        PlayerInfoPanelDisplayController playerInfoPanelDisplayController = spawnedPlayerPanel.GetComponent<PlayerInfoPanelDisplayController>();
        playerInfoPanelDisplayController.PlayerController = player;
        playerInfoPanelDisplayController.SetupPlayerInfo();

    }

    //In this way i have controls on what to do in the top level script when the button are pressed
    public void OnStopDealerClick()
    {
        onStopDealerClick.Invoke();
    }

    public void OnShuffleDeckClick()
    {
        onShuffleDeckClick.Invoke();
    }

    public void OnUserListClick()
    {
        UserListInfoPanel.SetActive(!UserListInfoPanel.activeInHierarchy);
        //Pauses the game
        Time.timeScale = 1 - Time.timeScale;
    }

    public void TogglePauseMenu()
    {
        PauseMenuPanel.SetActive(!PauseMenuPanel.activeInHierarchy);
        //Pauses the game
        Time.timeScale = 1 - Time.timeScale;
    }

    public void BackToMainMenu()
    {
        //Resetting the game time before leaving the scene
        Time.timeScale = 1 - Time.timeScale;
        SceneManager.LoadScene("MenuScene");
    }

    public void reactToRoundStateChanges(RoundState state)
    {
        RoundPhase.text = UIUtils.GetRoundStatusString(state);
        switch (state)
        {
            case RoundState.START:
                ShuffleDeck.gameObject.SetActive(true);
                break;

            case RoundState.PLAYERTURN:
                ShuffleDeck.gameObject.SetActive(false);
                break;

            case RoundState.DEALERTURN:
                ShuffleDeck.gameObject.SetActive(false);
                break;

            case RoundState.END:
                ShuffleDeck.gameObject.SetActive(false);
                break;
        }
    }

    public void reactToDealerScoreUpdate(int newScore)
    {
        DealerScore.text = newScore.ToString();
    }

    public void NoMoreCardsInDeck()
    {
        RoundPhase.text = UIUtils.GetNoMoreCardsString();
    }

}
