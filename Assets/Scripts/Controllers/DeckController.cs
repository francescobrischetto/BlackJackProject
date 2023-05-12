using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// This class is responsible of controlling a deck, allowing the user to spawn and throw cards.
/// </summary>
public class DeckController : MonoBehaviour
{
    //A list of Scriptable Objects "Card" composing the discarded cards
    private List<Card> discardedCards = new List<Card>();

    //Those variables controls the mouse dragging mechanism
    private bool isDragging = false;
    private Vector3 dragStartPosition;
    private float dragStartTime;

    //Those variables controls when the player is allowed to drag or click on the mouse
    private bool draggableControlsEnabled = false;
    private bool clickControlsEnabled = false;

    //Singleton Instance
    public static DeckController Instance { get; private set; }

    [field: Header("Deck Settings")]
    //A list of Scriptable Objects "Card" composing the deck
    [SerializeField] List<Card> deckCards;
    [SerializeField] GameObject shuffleParticleEffect;

    [field: Header("Card Launching Settings")]
    //this field determines a Y-axis offset to shift the card spawning point
    [SerializeField] float cardSpawningOffset;
    //this field is a threshold to determine if a player have done a drag powerful enough to throw a card
    [SerializeField] float lowestDragSpeed;
    //this field is an attenuation value that lowers the force to apply to the throwed card
    [SerializeField] float dragAttenuation;

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

    //TODO: Remove the Shuffle on Start
    private void Start()
    {
        Shuffle();
    }


    private void Update()
    {
        //I clicked and i'm allowed to click or drag on the deck
        if (Input.GetMouseButtonDown(0) && (draggableControlsEnabled || clickControlsEnabled))
        {
            //Check if I clicked on the deck
            RaycastHit raycastHit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out raycastHit, 100f))
            {
                if (raycastHit.transform != null)
                {
                    if (raycastHit.transform.gameObject.tag == "Deck")
                    {
                        //Different behaviour if I can drag or click on the deck (depending on Turn phase)
                        if (draggableControlsEnabled)
                        {
                            isDragging = true;
                            dragStartPosition = Input.mousePosition;
                            dragStartTime = Time.time;
                        }
                        if (clickControlsEnabled)
                        {
                            //Clicking on the deck will give the top card to the dealer
                            Card topCard = ThrowCard();
                            if (topCard != null)
                            {
                                DealerController.Instance.ReceiveCard(topCard);
                            }
                        }

                    }
                }
            }

        }
        //I'm releasing the mouse click and i'm allowed to drag on the deck
        else if (Input.GetMouseButtonUp(0) && draggableControlsEnabled)
        {
            //Check if I was dragging from previous frame
            if (isDragging)
            {
                //Calculating the drag speed based on the mouse distance and duration of the drag
                Vector3 dragEndPosition = Input.mousePosition;
                float dragDuration = Time.time - dragStartTime;
                float dragDistance = Vector3.Distance(dragEndPosition, dragStartPosition);
                float dragSpeed = dragDistance / dragDuration;
                // Check if drag speed is fast enough to launch the card
                if (dragSpeed > lowestDragSpeed)
                {
                    Vector3 cardPosition = transform.position + transform.up * cardSpawningOffset;
                    Card topCard = ThrowCard();
                    if (topCard != null)
                    {
                        GameObject newCard = Instantiate(topCard.cardAsset, cardPosition, Quaternion.identity);
                        //Localscale of the card should be the same as the deck localscale to spawn a card of the same "size"
                        newCard.transform.localScale = transform.localScale;
                        //Remember to destroy the card in future
                        StartCoroutine(CO_DestroyCardAfterSeconds(newCard));
                        //Launching the card in the given direction
                        Rigidbody rb = newCard.GetComponent<Rigidbody>();
                        Vector3 launchDirectionInCameraSpace = (dragEndPosition - dragStartPosition).normalized;
                        Vector3 launchDirectionInWorldSpace = Camera.main.transform.TransformDirection(launchDirectionInCameraSpace);
                        Vector3 forceDirection = launchDirectionInWorldSpace * (dragSpeed / dragAttenuation);
                        rb.AddForce(forceDirection, ForceMode.Force);
                    }
                }
                //Resetting the drag behaviour
                isDragging = false;
            }
        }
    }

    private void DisableControls()
    {
        draggableControlsEnabled = false;
        clickControlsEnabled = false;
    }

    private void AppendDiscardedCards()
    {
        deckCards.AddRange(discardedCards);
        discardedCards.Clear();
    }

    /// <summary>
    /// This method returns the Scriptable Objects "Card" giving the game object of a card. It can return null.
    /// </summary>
    /// <param name="goCard"></param>
    /// <returns></returns>
    public Card GetCardFromObject(GameObject goCard)
    {
        foreach (Card card in discardedCards)
        {
            //Checking the gameobject name to understand if it is a Clone (instance) of one of the discarded cards
            if (goCard.name.Contains(card.cardAsset.name))
            {
                return card;
            }
        }
        return null;
    }

    /// <summary>
    /// This coroutine destroys the spawned card after 3 seconds (if it still exists in the game).
    /// </summary>
    /// <param name="newCard"></param>
    /// <returns></returns>
    private IEnumerator CO_DestroyCardAfterSeconds(GameObject newCard)
    {
        yield return new WaitForSeconds(3);
        if (newCard != null)
        {
            Destroy(newCard);
        }

    }

    public void Shuffle()
    {
        deckCards.Shuffle();
        Vector3 particlePosition = transform.position + transform.up * 0.7f;
        GameObject particleSpawned = Instantiate(shuffleParticleEffect, particlePosition, Quaternion.identity);
        Destroy(particleSpawned, 1f);
    }

    /// <summary>
    /// This method returns the Scriptable Object "Card" that is on top of the deck. It may returns null.
    /// </summary>
    /// <returns></returns>
    public Card ThrowCard()
    {
        if(deckCards.Count > 0)
        {
            Card returnCard = deckCards[0];
            deckCards.RemoveAt(0);
            //Adding discarded card to the discarded list
            discardedCards.Add(returnCard);
            return returnCard;
        }
        else
        {
            //TODO: Trigger an event?
            Debug.Log("No more cards!");
            return null;
        }
        
    }
    
    /// <summary>
    /// This method allows the deck to react to any state change of the round
    /// </summary>
    /// <param name="state"></param>
    public void reactToRoundStateChanges(RoundState state)
    {
        switch (state)
        {
            case RoundState.START:
                DisableControls();
                AppendDiscardedCards();
                break;

            case RoundState.PLAYERTURN:
                draggableControlsEnabled = true;
                break;

            case RoundState.DEALERTURN:
                draggableControlsEnabled = false;
                clickControlsEnabled = true;
                break;

            case RoundState.END:
                clickControlsEnabled = false;
                break;
        }
    }
}