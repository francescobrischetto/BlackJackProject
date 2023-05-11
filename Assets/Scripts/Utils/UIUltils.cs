using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerStateString { NOTPLAYERTURN, ONEMORECARD, BUST, STOP, WON, LOST}
public static class UIUtils
{
    public static string GetStatusString(PlayerStateString playerStateString)
    {
        string result = "";
        switch (playerStateString)
        {
            case PlayerStateString.NOTPLAYERTURN:
                result = "Not My Turn";
                break;
            case PlayerStateString.ONEMORECARD:
                result = "More Cards";
                break;
            case PlayerStateString.BUST:
                result = "Busted";
                break;
            case PlayerStateString.STOP:
                result = "Stop";
                break;
            case PlayerStateString.WON:
                result = "Winner";
                break;
            case PlayerStateString.LOST:
                result = "Looser";
                break;
            default:
                break;

        }
        return result;
    }
}
