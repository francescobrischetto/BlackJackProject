using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This is responsible of placing the cards of the given player on the table
/// </summary>
public class VisualCardPositionController : MonoBehaviour
{
    private int indexFreeSlot = 0;
    //The list containing all the cards slots 
    [SerializeField] List<GameObject> cardPositions;

    private void ClearSlots()
    {
        //Remove all the childrens of all the card slots
        foreach (GameObject cardPosition in cardPositions)
        {
            cardPosition.transform.Clear();
        }
    }

    // This method resets the slots, clearing childrens and resetting index.
    private void ResetSlots()
    {
        ClearSlots();
        indexFreeSlot = 0;
    }

    /// <summary>
    /// This method allows the class to react to any state change of the round
    /// </summary>
    /// <param name="roundState"></param>
    public void reactToRoundChange(RoundState roundState)
    {
        switch (roundState)
        {
            case RoundState.START:
                ResetSlots();
                break;

            case RoundState.PLAYERTURN:
                break;

            case RoundState.DEALERTURN:
                break;

            case RoundState.END:
                break;
        }
    }

    /// <summary>
    /// This method allows the class to react to a card received to the player. It will put visually the card on the first available slot.
    /// </summary>
    /// <param name="goCard"></param>
    public void reactToCardReceived(GameObject goCard)
    {
        if(indexFreeSlot >=0 && indexFreeSlot < cardPositions.Count)
        {
            GameObject newCardOnTable = Instantiate(goCard, cardPositions[indexFreeSlot].transform.position, 
                Quaternion.identity, cardPositions[indexFreeSlot].transform);
            //The local scale of the card should be 1. It's size is controlled by the slot on the table (its parent)
            newCardOnTable.transform.localScale = new Vector3(1, 1, 1);
            //This is a fix necessary to rotate the card in the appropriate way
            newCardOnTable.transform.localRotation = Quaternion.Euler(0, 90, 0);
            //We do not need the rigid body for the cards on the table
            Rigidbody cardRigidBody = newCardOnTable.GetComponent<Rigidbody>();
            if(cardRigidBody != null)
            {
                Destroy(cardRigidBody);
            }
            indexFreeSlot++;
        }
    }
}
