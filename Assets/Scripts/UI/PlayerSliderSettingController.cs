using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// This script update the number above the slider controlling the number of players in game.
/// </summary>
public class PlayerSliderSettingController : MonoBehaviour
{
    [SerializeField] TMP_Text number;
    public void updateNumber(float num)
    {
        number.text = ((int)num).ToString();
    }
}
