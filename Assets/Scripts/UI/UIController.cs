using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] Button StopDealer;
    [SerializeField] Button ShuffleDeck;
    [SerializeField] Button UserListInfo;
    [SerializeField] GameObject UserListInfoPanel;

    public UnityEvent onStopDealerClick;
    public UnityEvent onShuffleDeckClick;

    private void Awake()
    {
        UserListInfoPanel.SetActive(false);
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
    }

}
