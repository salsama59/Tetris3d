using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUtils : MonoBehaviour {

	public static GameManager FetchGameManagerScript()
    {
        GameObject gameManagerObject = GameObject.FindGameObjectWithTag(TagConstants.TAG_NAME_GAME_MANAGER);
        GameManager gameManagerScript = gameManagerObject.GetComponent<GameManager>();

        return gameManagerScript;
    }

    public static ScoreManager FetchScoreManagerScript()
    {
        GameObject scoreManagerObject = GameObject.FindGameObjectWithTag(TagConstants.TAG_NAME_SCORE_MANAGER);
        ScoreManager scoreManagerScript = scoreManagerObject.GetComponent<ScoreManager>();

        return scoreManagerScript;
    }

    public static GameObject FetchPlayerField(int playerId)
    {
        return FetchGameManagerScript().PlayersField[playerId];
    }

    public static GameObject FetchPlayerForSeeWindow(int playerId)
    {
        return FetchGameManagerScript().PlayersForeSeeWindow[playerId];
    }

    public static PositionMapElement[,] FetchPlayerPositionMap(int playerId)
    {
        return FetchGameManagerScript().PlayersPositionMap[playerId];
    }

    public static float FetchPlayerPieceSpeed(int playerId)
    {
        return FetchGameManagerScript().PlayersPiecesMovementSpeed[playerId];
    }
}
