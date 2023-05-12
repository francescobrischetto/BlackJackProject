using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] Button StopDealer;
    [SerializeField] Button ShuffleDeck;
    [SerializeField] Button UserListInfo;
    [SerializeField] GameObject UserListInfoPanel;

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
        UserListInfoPanel.SetActive(false);
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

    public void SpawnPlayerPanel(PlayerController player)
    {
        GameObject spawnedPlayerPanel = Instantiate(PlayerInfoPanel, Vector3.zero, Quaternion.identity, PlayerInfoGridContentPanel.transform);
        PlayerInfoPanelDisplayController playerInfoPanelDisplayController = spawnedPlayerPanel.GetComponent<PlayerInfoPanelDisplayController>();
        playerInfoPanelDisplayController.PlayerController = player;
        playerInfoPanelDisplayController.SetupPlayerInfo();

    }
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
        Time.timeScale = 1 - Time.timeScale;
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
        RoundPhase.text = "No Cards!";
    }

}
