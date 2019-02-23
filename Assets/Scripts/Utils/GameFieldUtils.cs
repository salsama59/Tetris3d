using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameFieldUtils : MonoBehaviour {

    public static float FIELD_MARGIN = 2f;
    public static float maxAllowedPlayableLine = 19.5f;

    public static int PositionToMapValue(float position, int playerId, GameObject fieldGameObject, GameObject fieldForeSeeWindowGameObject)
    {
        int collumn = 0;
        if (playerId == (int)PlayerEnum.PlayerId.PLAYER_1)
        {
            collumn = (int)((ElementType.CalculateGameObjectMaxRange(fieldGameObject.transform.GetChild(0).gameObject).x + FIELD_MARGIN - 0.5f) - (position * -1));
        }
        else if (playerId == (int)PlayerEnum.PlayerId.PLAYER_2)
        {
            collumn = (int)(position - (ElementType.CalculateGameObjectMaxRange(fieldForeSeeWindowGameObject.transform.GetChild(0).gameObject).x + FIELD_MARGIN + 0.5f));
        }
        
        return collumn;
    }

    public static float MapValueToPosition(int mapValue, int playerId, GameObject fieldGameObject, GameObject fieldForeSeeWindowGameObject)
    {
        float position = 0;
        if (playerId == (int)PlayerEnum.PlayerId.PLAYER_1)
        {
            position = -1 * (ElementType.CalculateGameObjectMaxRange(fieldGameObject.transform.GetChild(0).gameObject).x - mapValue + FIELD_MARGIN - 0.5f);
        }
        else if (playerId == (int)PlayerEnum.PlayerId.PLAYER_2)
        {
            position = mapValue + ElementType.CalculateGameObjectMaxRange(fieldForeSeeWindowGameObject.transform.GetChild(0).gameObject).x + FIELD_MARGIN + 0.5f;
        }

        return position;
    }

    public static GameObject FetchHighestPieceInPlayerField(int playerId)
    {
        string tag = "";

        if ((int)PlayerEnum.PlayerId.PLAYER_1 == playerId)
        {
            tag = TagConstants.TAG_NAME_PLAYER_1_PIECE_CHILD;
        }
        else if ((int)PlayerEnum.PlayerId.PLAYER_2 == playerId)
        {
            tag = TagConstants.TAG_NAME_PLAYER_2_PIECE_CHILD;
        }

        GameObject[] pieceChildList = GameObject.FindGameObjectsWithTag(tag)
            .Where(childObject => childObject.transform.parent == null /*&& childObject.layer == LayerMask.NameToLayer(LayerConstants.LAYER_NAME_DESTROYABLE_PIECE)*/)
            .OrderByDescending(childObject => childObject.transform.position.z)
            .ToArray();

        return pieceChildList.Length > 0 ? pieceChildList.First() : null;
    }

}