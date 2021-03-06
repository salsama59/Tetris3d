﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/**
 * Utility class to help Manage the AI a the game
 **/
public class AiUtils : MonoBehaviour {

    /**
     * Verify if a piece current placement will generate gaps given the piece itself, the current piece player id and the botton pieces positions list
     * FIXME the piece own gaps are not take into accounts in some cases.
     **/
    public static bool IsLineGapPossible(GameObject parentPiece, List<Vector3> bottomPiecesPositions, int playerId)
    {
        List<float> distanceList = CalculateDistanceList(parentPiece, bottomPiecesPositions);

        //if any of the distances calculated are different from the others this means there will be a gap
        return distanceList.Any(distance => distance != distanceList.First());

    }

    public static List<float> CalculateDistanceList(GameObject parentPiece, List<Vector3> bottomPiecesPositions)
    {
        List<Transform> referencesTransforms = new List<Transform>();
        Dictionary<int, Transform> transformDictionnary = new Dictionary<int, Transform>();
        List<float> distanceList = new List<float>();
        List<Vector3> newlyCalculatedBottomPiecesPositions = new List<Vector3>();

        //Gather the current parent piece child sorted by descending z positions
        Transform[] childrenTransform = parentPiece
            .GetComponentsInChildren<Transform>()
            .Where(childTransform => childTransform.gameObject != parentPiece)
            .OrderByDescending(childTransform => childTransform.position.z)
            .ToArray();

        switch (childrenTransform.Length)
        {
            case 1:
                referencesTransforms.Add(childrenTransform.First());
                break;
            default:
                foreach (Transform transform in childrenTransform)
                {
                    referencesTransforms.Add(transform);
                }
                break;
        }

        //Group the child per column and keep those with the less higher z position value
        foreach (Transform transform in referencesTransforms)
        {
            int transformDictionaryKey = 0;

            transformDictionaryKey = (int)Mathf.Round(transform.position.x - 0.5f);

            if (transformDictionnary.ContainsKey(transformDictionaryKey))
            {
                transformDictionnary[transformDictionaryKey] = transform;
            }
            else
            {
                transformDictionnary.Add(transformDictionaryKey, transform);
            }

        }

        //If there is no piece at the bottom we try to simulate the game field ground, this way all calcul will be made given this hypothesis 
        if (bottomPiecesPositions == null)
        {

            int[] xPositionIndexes = transformDictionnary.Keys.OrderBy(key => key).ToArray();

            foreach (int xPositionIndex in xPositionIndexes)
            {
                newlyCalculatedBottomPiecesPositions.Add(new Vector3(xPositionIndex + 0.5f, 0.5f, -0.5f));
            }

        }
        else
        {
            newlyCalculatedBottomPiecesPositions = bottomPiecesPositions;
        }

        //Calculate the distance with the pieces on the ground or the ground and the parent piece child
        foreach (Vector3 position in newlyCalculatedBottomPiecesPositions)
        {
            int synchro = (int)Mathf.Round(position.x - 0.5f);

            if (!transformDictionnary.ContainsKey(synchro))
            {
                Debug.Log("Did not find key : " + synchro);
            }

            Transform currentChild = transformDictionnary[synchro];

            float distance = Mathf.Abs(position.z - currentChild.position.z - 1f);

            distanceList.Add(distance);

        }

        return distanceList;
    }

    public static List<Vector3> GetBottomPiecePositions(int playerId, GameObject parentPiece)
    {
        List<Vector3> piecePositions = new List<Vector3>();
        Transform[] childrenTransform = parentPiece
            .GetComponentsInChildren<Transform>()
            .Where(childTransform => childTransform.gameObject != parentPiece)
            .OrderBy(childTransform => childTransform.position.x)
            .ToArray();

        PositionMapElement[,] playerPositionMapElement = GameUtils.FetchPlayerPositionMap(playerId);
        GameObject highestPiece = GameFieldUtils.FetchHighestPieceInPlayerField(playerId);

        if (highestPiece == null)
        {
            return null;
        }

        GameObject playerField = GameUtils.FetchPlayerField(playerId);

        GameObject playerForSeeWindow = GameUtils.FetchPlayerForSeeWindow(playerId);

        int mapVerticalStartPosition = Mathf.RoundToInt(highestPiece.transform.position.z - 0.5f);

        if (childrenTransform.Length == 1)
        {
            return GetPositionListForSingleChild(playerId, piecePositions, childrenTransform, playerPositionMapElement, playerField, playerForSeeWindow, mapVerticalStartPosition);
        }
        else
        {
            return GetPositionListForMultipleChild(playerId, piecePositions, childrenTransform, playerPositionMapElement, playerField, playerForSeeWindow, mapVerticalStartPosition);
        }
        
    }

    private static List<Vector3> GetPositionListForMultipleChild(int playerId, List<Vector3> piecePositions, Transform[] childrenTransform, PositionMapElement[,] playerPositionMapElement, GameObject playerField, GameObject playerForSeeWindow, int mapVerticalStartPosition)
    {

        int mapMinHorizontalPosition;
        int mapMaxHorizontalPosition;
        if (ApplicationUtils.IsInMultiPlayerMode())
        {
            mapMinHorizontalPosition = GameFieldUtils.PositionToMapValue(childrenTransform.First().position.x, playerId, playerField, playerForSeeWindow);
            mapMaxHorizontalPosition = GameFieldUtils.PositionToMapValue(childrenTransform.Last().position.x, playerId, playerField, playerForSeeWindow);
        }
        else
        {
            mapMinHorizontalPosition = (int)Mathf.Round(childrenTransform.First().position.x - 0.5f);
            mapMaxHorizontalPosition = (int)Mathf.Round(childrenTransform.Last().position.x - 0.5f);
        }

        for (int i = mapMinHorizontalPosition; i <= mapMaxHorizontalPosition; i++)
        {
            for (int j = mapVerticalStartPosition; j >= 0; j--)
            {
                if (playerPositionMapElement[j, i].IsOccupied)
                {
                    piecePositions.Add(playerPositionMapElement[j, i].CurrentMapElement.transform.position);
                    break;
                }

                if (j == 0)
                {
                    piecePositions.Add(new Vector3(GameFieldUtils.MapValueToPosition(i, playerId, playerField, playerForSeeWindow), 0.5f, -0.5f));
                }
            }
        }

        return piecePositions;
    }

    private static List<Vector3> GetPositionListForSingleChild(int playerId, List<Vector3> piecePositions, Transform[] childrenTransform, PositionMapElement[,] playerPositionMapElement, GameObject playerField, GameObject playerForSeeWindow, int mapVerticalStartPosition)
    {
        int mapSingleHorizontalPosition;

        if (ApplicationUtils.IsInMultiPlayerMode())
        {
            mapSingleHorizontalPosition = GameFieldUtils.PositionToMapValue(childrenTransform.First().position.x, playerId, playerField, playerForSeeWindow);
        }
        else
        {
            mapSingleHorizontalPosition = (int)Mathf.Round(childrenTransform.First().position.x - 0.5f);
        }

        for (int j = mapVerticalStartPosition; j >= 0; j--)
        {
            if (playerPositionMapElement[j, mapSingleHorizontalPosition].IsOccupied)
            {
                piecePositions.Add(playerPositionMapElement[j, mapSingleHorizontalPosition].CurrentMapElement.transform.position);
                break;
            }

            if (j == 0)
            {
                piecePositions.Add(new Vector3(GameFieldUtils.MapValueToPosition(mapSingleHorizontalPosition, playerId, playerField, playerForSeeWindow), 0.5f, -0.5f));
            }
        }

        return piecePositions;
    }

    public static float GetHighestDistanceBetweenPieces(int playerId, GameObject parentPiece)
    {
        float highestDistance = 0;

        //The two lists are ordered by ascending x position
        List<Vector3> bottomPiecesPositionList = GetBottomPiecePositions(playerId, parentPiece);

        if (bottomPiecesPositionList == null)
        {
            return highestDistance;
        }

        List<float> distanceList = CalculateDistanceList(parentPiece, bottomPiecesPositionList);

        for (int i = 0; i < distanceList.Count; i++)
        {

            float distance = distanceList[i];

            if (distance > highestDistance)
            {
                highestDistance = distance;
            }
        }
        
        return highestDistance;
    }

}
