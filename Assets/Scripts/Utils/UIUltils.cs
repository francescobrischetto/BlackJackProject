using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UIUtils
{
    public static string GetStatusString(PlayerState playerState)
    {
        string result = "";
        switch (playerState)
        {
            case PlayerState.NOTPLAYERTURN:
                result = "Not My Turn";
                break;
            case PlayerState.ONEMORECARD:
                result = "More Cards";
                break;
            case PlayerState.BUST:
                result = "Busted";
                break;
            case PlayerState.STOP:
                result = "Stop";
                break;
            case PlayerState.WON:
                result = "Winner";
                break;
            case PlayerState.LOST:
                result = "Looser";
                break;
            default:
                break;

        }
        return result;
    }

    public static string GetRoundStatusString(RoundState roundState)
    {
        string result = "";
        switch (roundState)
        {
            case RoundState.START:
                result = "Starting";
                break;
            case RoundState.PLAYERTURN:
                result = "Players Turn";
                break;
            case RoundState.DEALERTURN:
                result = "Dealer Turn";
                break;
            case RoundState.END:
                result = "Ending";
                break;


        }
        return result;
    }
}
