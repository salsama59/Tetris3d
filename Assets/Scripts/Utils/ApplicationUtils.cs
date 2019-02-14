using System.Collections.Generic;
using UnityEngine;

public class ApplicationUtils {
    public static int playerNumber = 0;
    public static Dictionary<int, string> playerAffectation = new Dictionary<int, string>();
    public static Dictionary<int, Color> playerPanelColor = new Dictionary<int, Color>(){
            { (int)PlayerEnum.PlayerId.PLAYER_1, new Color(60f/255f, 109f/255f, 37f/255f, 255f/255f) },
            { (int)PlayerEnum.PlayerId.PLAYER_2, new Color(19f/255f, 86f/255f, 120f/255f, 255f/255f) }
        };

    public static bool IsInMultiPlayerMode()
    {
        return playerNumber > 1;
    }

    public static string GetPlayerAffectation(int playerId)
    {
        return playerAffectation[playerId];
    }

    public static bool IsTypeComputer(int playerId)
    {
        string affectation = playerAffectation[playerId];

        return affectation == PlayerTypeConstants.PLAYER_COMPUTER;
    }

    public static void AffectPlayer(int playerId, string affectation)
    {
        if (playerAffectation.ContainsKey(playerId))
        {
            playerAffectation[playerId] = affectation;
        }
        else
        {
            playerAffectation.Add(playerId, affectation);
        }
    }

    public static Color GetPlayerColor(int playerId)
    {
        return playerPanelColor[playerId];
    }
    
}
