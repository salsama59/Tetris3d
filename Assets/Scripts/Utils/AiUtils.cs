using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//Class pour gérer l'intelligence artificielle
public class AiUtils : MonoBehaviour {

    //Vérifie pour une pièce donnée si le placement donnera lieu à des trou dans des lignes
    //FIXME les trous de la piece en elle meme ne sont pas pris en compte dans le calcul, il faut le faire dans un second temps
    //la liste de position correspond à l'intervale minimum testé qui sera déterminé en fonction de la largeur de la pièce actuelle (pièce parent)
	public static bool IsLineGapPossible(GameObject pieceParent, List<Vector3> piecePositions)
    {
        
        List<Transform> referencesTransforms = new List<Transform>();
        Dictionary<int, Transform> transformDictionnary = new Dictionary<int, Transform>();
        List<float> distanceList = new List<float>();

        //Récupérer tout les enfants de la pièce triés par hauteur sur z decroissant
        Transform[] childrenTransform = pieceParent
            .GetComponentsInChildren<Transform>()
            .Where(childTransform => childTransform.gameObject != pieceParent)
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

        //Gouper les enfants par colonnes et ne garder que les enfants dont la hauteur sur z est la moins élevée
        foreach (Transform transform in referencesTransforms)
        {
            int transformDictionaryKey = Mathf.RoundToInt(transform.position.x - 0.5f);

            if (transformDictionnary.ContainsKey(transformDictionaryKey))
            {
                transformDictionnary[transformDictionaryKey] = transform;
            }
            else
            {
                transformDictionnary.Add(Mathf.RoundToInt(transform.position.x - 0.5f), transform);
            }
            
        }

        //Si il n'existe pas de pieces posées alors on simule le sol de l'air de jeu, ainsi les calcul seront fais sur cette base
        if (piecePositions == null)
        {
            piecePositions = new List<Vector3>();
            int[] xPositionIndexes = transformDictionnary.Keys.OrderBy(key => key).ToArray();

            foreach (int xPositionIndex in xPositionIndexes)
            {
                piecePositions.Add(new Vector3(xPositionIndex + 0.5f, 0.5f, -0.5f));
            }

        }

        //Calculer la distances avec la pièce du bas pour chacun des enfants et définir la plus petite des distances
        foreach (Vector3 position in piecePositions)
        {
            int synchro = Mathf.RoundToInt(position.x - 0.5f);
            Transform currentChild = transformDictionnary[synchro];

            float distance = Mathf.Abs(position.z - currentChild.position.z - 1f);

            distanceList.Add(distance);

            //Debug.Log(distance);

        }

        return distanceList.Any(distance => distance != distanceList.First());

    }

    public static List<Vector3> GetBottomPiecePositions(int playerId, GameObject parentPiece)
    {
        Debug.Log("**** GetBottomPiecePositions begin ****");
        List<Vector3> piecePositions = new List<Vector3>();

        Debug.Log("parentPiece.name = " + parentPiece.name);
        Debug.Log("parentPiece.transform.position = " + parentPiece.transform.position);
        Transform[] childrenTransform = parentPiece
            .GetComponentsInChildren<Transform>()
            .Where(childTransform => childTransform.gameObject != parentPiece)
            .OrderBy(childTransform => childTransform.position.x)
            .ToArray();

        PositionMapElement[,] playerPositionMapElement = GameUtils.FetchPlayerPositionMap(playerId);
        GameObject highestPiece = GameFieldUtils.FetchHighestPieceInPlayerField(playerId);

        if (highestPiece == null)
        {
            Debug.Log("**** GetBottomPiecePositions end ****");
            return null;
        }

        GameObject playerField = GameUtils.FetchPlayerField(playerId);

        GameObject playerForSeeWindow = GameUtils.FetchPlayerForSeeWindow(playerId);

        int mapVerticalStartPosition = Mathf.RoundToInt(highestPiece.transform.position.z - 0.5f);

        if (childrenTransform.Length == 1)
        {
            Debug.Log("**** GetBottomPiecePositions end ****");
            return GetPositionListForSingleChild(playerId, piecePositions, childrenTransform, playerPositionMapElement, playerField, playerForSeeWindow, mapVerticalStartPosition);
        }
        else
        {
            Debug.Log("**** GetBottomPiecePositions end ****");
            return GetPositionListForMultipleChild(playerId, piecePositions, childrenTransform, playerPositionMapElement, playerField, playerForSeeWindow, mapVerticalStartPosition);
        }
        
    }

    public static IaData CalculateAction(GameObject currentSimulatedObject, int sideId)
    {
        Debug.Log("**** CalculateAction start ****");
        GameObject simulatedObjectClone = PieceUtils.ClonePieceObject(currentSimulatedObject);

        IaData iaInformations = new IaData();

        Transform transform;
        //position map elements => [lines, collumns]
        Debug.Log("Trying to simulate right");
        transform = SimulateMovement(Vector3.right, simulatedObjectClone, sideId);

        if(transform != null)
        {
            iaInformations.TargetPosition = transform.position;
            iaInformations.TargetRotation = transform.rotation;
            Destroy(simulatedObjectClone);
            Debug.Log("**** CalculateAction end ****");
            return iaInformations;
        }

        simulatedObjectClone.transform.SetPositionAndRotation(currentSimulatedObject.transform.position, currentSimulatedObject.transform.rotation);

        Debug.Log("Trying to simulate left");
        transform = SimulateMovement(Vector3.left, simulatedObjectClone, sideId);

        if (transform == null)
        {
            iaInformations.TargetPosition = currentSimulatedObject.transform.position;
            iaInformations.TargetRotation = currentSimulatedObject.transform.rotation;
        }
        else
        {
            iaInformations.TargetPosition = transform.position;
            iaInformations.TargetRotation = transform.rotation;
        }

        Destroy(simulatedObjectClone);
        Debug.Log("**** CalculateAction end ****");
        return iaInformations;
    }

    private static Transform SimulateMovement(Vector3 direction, GameObject objectClone, int playerSide)
    {
        Transform pieceCloneTransform = null;
        bool isCurrentSimulationInProgress = true;
        while (isCurrentSimulationInProgress)
        {
            bool possible = IsLineGapPossible(objectClone, GetBottomPiecePositions(playerSide, objectClone));
            Debug.Log("Gap is possible ??? => " + possible);
            if (possible)
            {
                bool isMovePossible = MovementUtils.IsMovementPossible(direction, objectClone);
                if (isMovePossible)
                {
                    Debug.Log("IsMovementPossible => " + isMovePossible);
                    MovementGeneratorUtils.SimulateNextTranslation(objectClone, direction);
                    Debug.Log("Move to  => " + objectClone.transform.position);
                }
                else
                {
                    Debug.Log("Did not find a rigth location");
                    return null;
                }
                
            }
            else
            {
                return objectClone.transform;
            }
            
        }

        return pieceCloneTransform;
    }

    private static List<Vector3> GetPositionListForMultipleChild(int playerId, List<Vector3> piecePositions, Transform[] childrenTransform, PositionMapElement[,] playerPositionMapElement, GameObject playerField, GameObject playerForSeeWindow, int mapVerticalStartPosition)
    {
        Debug.Log("**** GetPositionListForMultipleChild start ****");
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
                Debug.Log("i = " + i + " j = " + j);
                if (playerPositionMapElement[j, i].IsOccupied)
                {
                    piecePositions.Add(playerPositionMapElement[j, i].CurrentMapElement.transform.position);
                    break;
                }

                if (j == 0)
                {
                    piecePositions.Add(new Vector3(i + 0.5f, 0.5f, -0.5f));
                }
            }
        }
        Debug.Log("**** GetPositionListForMultipleChild end ****");
        return piecePositions;
    }

    private static List<Vector3> GetPositionListForSingleChild(int playerId, List<Vector3> piecePositions, Transform[] childrenTransform, PositionMapElement[,] playerPositionMapElement, GameObject playerField, GameObject playerForSeeWindow, int mapVerticalStartPosition)
    {
        Debug.Log("**** GetPositionListForSingleChild start ****");
        int mapSingleHorizontalPosition;

        if (ApplicationUtils.IsInMultiPlayerMode())
        {
            mapSingleHorizontalPosition = GameFieldUtils.PositionToMapValue(childrenTransform.First().position.x, playerId, playerField, playerForSeeWindow);
        }
        else
        {
            mapSingleHorizontalPosition = (int)Mathf.Round(childrenTransform.First().position.x - 0.5f);
        }

        for (int j = mapVerticalStartPosition; j > 0; j--)
        {
            Debug.Log("i = " + mapSingleHorizontalPosition + " j = " + j);
            if (playerPositionMapElement[j, mapSingleHorizontalPosition].IsOccupied)
            {
                piecePositions.Add(playerPositionMapElement[j, mapSingleHorizontalPosition].CurrentMapElement.transform.position);
                break;
            }
        }
        Debug.Log("**** GetPositionListForSingleChild end ****");
        return piecePositions;
    }

}
