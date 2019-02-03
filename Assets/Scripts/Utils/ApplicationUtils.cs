using System.Collections.Generic;

public class ApplicationUtils {
    public static int playerNumber = 0;
    public static Dictionary<int, string> playerAffectation = new Dictionary<int, string>();

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
        playerAffectation.Add(playerId, affectation);
    }
}
