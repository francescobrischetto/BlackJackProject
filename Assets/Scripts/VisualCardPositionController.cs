using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class VisualCardPositionController : MonoBehaviour
{
    [SerializeField] List<GameObject> cardPositions;
    private int indexFreeSlot = 0;

    public void ResetSlots()
    {
        ClearSlots();
        indexFreeSlot = 0;
    }

    private void ClearSlots()
    {
        foreach(GameObject cardPosition in cardPositions)
        {
            cardPosition.transform.Clear();
        }
    }

    public void setCardOnTable(GameObject goCard)
    {
        if(indexFreeSlot >=0 && indexFreeSlot < cardPositions.Count)
        {
            GameObject newCardOnTable = Instantiate(goCard, cardPositions[indexFreeSlot].transform.position, Quaternion.identity, cardPositions[indexFreeSlot].transform);
            newCardOnTable.transform.localScale = new Vector3(1, 1, 1);
            newCardOnTable.transform.localRotation = Quaternion.Euler(0, 90, 0);
            Rigidbody cardRigidBody = newCardOnTable.GetComponent<Rigidbody>();
            Destroy(cardRigidBody);
            indexFreeSlot++;
        }
    }
}
