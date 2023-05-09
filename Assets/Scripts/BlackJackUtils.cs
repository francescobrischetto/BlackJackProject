using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class BlackJackUtils
{
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
    
    public static int CalculateBestScore(List<int> scores)
    {
        scores.Add(0);
        return scores.Where(x => x <= 21).OrderByDescending(x => x).ToList()[0];
    }
}
