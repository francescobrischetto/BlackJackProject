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
            GameObject newCardOnTable = Instantiate(goCard, cardPositions[indexFreeSlot].transform.position + new Vector3(0,0.3f,0), Quaternion.identity, cardPositions[indexFreeSlot].transform);
            //Debug.Log(newCardOnTable);
            //EditorGUIUtility.PingObject(newCardOnTable);

            newCardOnTable.transform.localScale = new Vector3(1, 1, 1);
            //Rigidbody cardRigidBody = newCardOnTable.GetComponent<Rigidbody>();
            //Destroy(cardRigidBody);
            indexFreeSlot++;
        }
    }
}
