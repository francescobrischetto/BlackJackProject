using System.Collections.Generic;
using System.Linq;

//This struct contains the round informations
public struct GameState
{
    public int DealerScore;
    public List<PlayerInfo> PlayerInfos;
}

//This struct contains the round information of the player
public struct PlayerInfo
{
    public string Name;
    public int Score;
    public bool Won;

}

//Enums determining each Round Phase and Player State
public enum RoundState { START, PLAYERTURN, DEALERTURN, END }
public enum PlayerState { NOTPLAYERTURN, ONEMORECARD, BUST, STOP }

public static class BlackJackUtils
{

    /// <summary>
    /// This method calculate the new score of a blackjack player receiving a card.
    /// </summary>
    /// <param name="scores">Previous possible score list (considering Ace that has two possible values).</param>
    /// <param name="card">The new card received by the player.</param>
    /// <returns></returns>
    public static List<int> CalculateNewScores(List<int> scores, Card card)
    {
        List<int> newScores = scores.GetRange(0, scores.Count);
        for (int i = 0; i < scores.Count; i++)
        {
            newScores[i] = scores[i] + card.values[0];
        }
        //I received an Ace
        if (card.values.Count > 1)
        {
            List<int> scoreCopy = scores.GetRange(0, scores.Count);
            for (int i = 0; i < scoreCopy.Count; i++)
            {
                scoreCopy[i] = scoreCopy[i] + card.values[1];
            }
            newScores.AddRange(scoreCopy);
        }
        return newScores;
    }

    /// <summary>
    /// This method returns the best score of a blackjack player or 0 if exceeds the score 21.
    /// </summary>
    /// <param name="scores">List of possible scores (Ace has two possible values)</param>
    /// <returns></returns>
    public static int CalculateBestScore(List<int> scores)
    {
        List<int> bestScores = scores.Where(x => x <= 21).OrderByDescending(x => x).ToList();
        if(bestScores.Count > 0)
        {
            return bestScores[0];
        }
        else
        {
            return 0;
        }
    }
}
