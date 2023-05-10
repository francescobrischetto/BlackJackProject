using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="New Card", menuName = "Card")]
public class Card : ScriptableObject
{
    public new string name;
    //Some cards can have more than one value (E.g. Ace => 1, 11)
    public List<int> values;
    //GameObject to spawn
    public GameObject cardAsset;
}
    