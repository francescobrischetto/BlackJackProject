using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class Deck : MonoBehaviour
{
    //This represents the prefab that should spawn whenever a card is launched
    [SerializeField] List<Card> cards;

    [field: Header("Card Launching settings")]
    [SerializeField] float cardSpawningOffset;
    [SerializeField] float lowestDragSpeed;
    [SerializeField] float dragAttenuation;


    //Those variables controls the mouse dragging mechanism
    private bool isDragging = false;
    private Vector3 dragStartPosition;
    private float dragStartTime;
    
    public void Shuffle()
    {
        cards.Shuffle();
    }

    public Card ThrowCard()
    {
        if(cards.Count > 0)
        {
            Card returnCard = cards[0];
            cards.RemoveAt(0);
            return returnCard;
        }
        else
        {
            Debug.Log("No more cards!");
            return null;
        }
        
    }
    private void Start()
    {
        Shuffle();
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
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
                        isDragging = true;
                        dragStartPosition = Input.mousePosition;
                        dragStartTime = Time.time;
                    }
                }
            }
            
        }

        else if (Input.GetMouseButtonUp(0))
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
}