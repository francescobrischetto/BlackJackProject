using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    public static Deck Instance { get; private set; }

    //This represents the prefab that should spawn whenever a card is launched
    [SerializeField] List<Card> deckCards;

    [field: Header("Card Launching settings")]
    [SerializeField] float cardSpawningOffset;
    [SerializeField] float lowestDragSpeed;
    [SerializeField] float dragAttenuation;


    private List<Card> discardedCards = new List<Card>();

    //Those variables controls the mouse dragging mechanism
    private bool isDragging = false;
    private Vector3 dragStartPosition;
    private float dragStartTime;

    private bool draggableControlsEnabled = false;
    private bool clickControlsEnabled = false;


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
        Shuffle();
    }

    private void DisableControls()
    {
        draggableControlsEnabled = false;
        clickControlsEnabled = false;
    }

    public Card GetCardFromObject(GameObject goCard)
    {
        foreach(Card card in discardedCards)
        {
            if(goCard.name.Contains(card.cardAsset.name))
            {
                return card;
            }
        }
        return null;
    }

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


    public void Shuffle()
    {
        deckCards.Shuffle();
    }

    public void AppendDiscardedCards()
    {
        deckCards.AddRange(discardedCards);
        discardedCards.Clear();
    }

    public Card ThrowCard()
    {
        if(deckCards.Count > 0)
        {
            Card returnCard = deckCards[0];
            deckCards.RemoveAt(0);
            discardedCards.Add(returnCard);
            Debug.Log($"Returned card name {returnCard.name} and value {returnCard.values[0]}");
            return returnCard;
        }
        else
        {
            Debug.Log("No more cards!");
            return null;
        }
        
    }
    private void Update()
    {
        //DraggingMechanics
        if (Input.GetMouseButtonDown(0) && ( draggableControlsEnabled || clickControlsEnabled))
        {
            RaycastHit raycastHit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out raycastHit, 100f))
            {
                //If we started the drag onto the deck, the drag is considered valid and we will trhow a card
                if (raycastHit.transform != null)
                {
                    if (raycastHit.transform.gameObject.tag == "Deck")
                    {
                        if (draggableControlsEnabled)
                        {
                            isDragging = true;
                            dragStartPosition = Input.mousePosition;
                            dragStartTime = Time.time;
                        }
                        if(clickControlsEnabled)
                        {
                            Card topCard = ThrowCard();
                            GameController.Instance.ReceiveDealerCard(topCard);
                        }
                       
                    }
                }
            }
            
        }

        else if (Input.GetMouseButtonUp(0) && draggableControlsEnabled)
        {
            if (isDragging)
            {
                Vector3 dragEndPosition = Input.mousePosition;
                float dragDuration = Time.time - dragStartTime;

                float dragDistance = Vector3.Distance(dragEndPosition, dragStartPosition);
                float dragSpeed = dragDistance / dragDuration;

                if (dragSpeed > lowestDragSpeed) // Check if drag speed is fast enough to launch the card
                {
                    Vector3 cardPosition = transform.position + transform.up * cardSpawningOffset;
                    Card topCard = ThrowCard();
                    if(topCard != null)
                    {
                        //Instantiating the card
                        GameObject newCard = Instantiate(topCard.cardAsset, cardPosition, Quaternion.identity);
                        //Localscale of the card should be the same as the deck localscale
                        newCard.transform.localScale = transform.localScale;
                        StartCoroutine(DestroyCardAfterSeconds(newCard));
                        Rigidbody rb = newCard.GetComponent<Rigidbody>();
                        Vector3 launchDirectionInCameraSpace = (dragEndPosition - dragStartPosition).normalized;
                        Vector3 launchDirectionInWorldSpace = Camera.main.transform.TransformDirection(launchDirectionInCameraSpace);
                        //Attenuation of the drag speed
                        Vector3 forceDirection = launchDirectionInWorldSpace * (dragSpeed / dragAttenuation);
                        //Launching the card
                        rb.AddForce(forceDirection, ForceMode.Impulse);
                    }
                }
            }

            isDragging = false;
        }
    }

    IEnumerator DestroyCardAfterSeconds(GameObject newCard)
    {
        yield return new WaitForSeconds(3);
        if(newCard != null)
        {
            Destroy(newCard);
        }

    }
}