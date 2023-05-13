using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerSliderSettingController : MonoBehaviour
{
    [SerializeField] TMP_Text number;
    public void updateNumber(float num)
    {
        number.text = ((int)num).ToString();
    }
}
