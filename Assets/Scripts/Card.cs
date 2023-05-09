using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="New Card", menuName = "Card")]
public class Card : ScriptableObject
{
    public new string name;
    public List<int> values;
    public GameObject cardAsset;
}
    