﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PieceUtils : MonoBehaviour {

	public static PieceMetadatas FetchPieceMetadataScript(GameObject parentPiece)
    {
        return parentPiece.GetComponent<PieceMetadatas>();
    }

    public static ComputerPieceController FetchPieceComputerPieceControllerScript(GameObject parentPiece)
    {
        return parentPiece.GetComponent<ComputerPieceController>();
    }

    public static PlayerPieceController FetchPiecePlayerPieceControllerScript(GameObject parentPiece)
    {
        return parentPiece.GetComponent<PlayerPieceController>();
    }

    public static PieceController FetchCurrentlyActivatedPieceControllerScript(GameObject parentPiece)
    {
        PieceController[] monoBehaviourScripts = parentPiece.GetComponents<PieceController>()
            .Where(script =>  script.enabled)
            .ToArray();

        if (monoBehaviourScripts.Length == 0)
        {
            return null;
        }
        else
        {
            return monoBehaviourScripts.First() as PieceController;
        }
    }
    
    public static GameObject ClonePieceObject(GameObject parentPieceObjectToBeCloned)
    {
        PieceMetadatas parentPieceObjectToBeClonedMetaDataScript =  FetchPieceMetadataScript(parentPieceObjectToBeCloned);

        Transform[] childrenTransform = parentPieceObjectToBeCloned.transform
            .GetComponentsInChildren<Transform>()
            .Where(child => child.gameObject != parentPieceObjectToBeCloned)
            .ToArray();

        GameObject finalParent = new GameObject();
        finalParent.transform.SetPositionAndRotation(parentPieceObjectToBeCloned.transform.position, parentPieceObjectToBeCloned.transform.rotation);

        foreach (Transform childTransform in childrenTransform)
        {
            GameObject child = new GameObject();

            child.transform.SetPositionAndRotation(childTransform.position, childTransform.rotation);
            child.transform.parent = finalParent.transform;

        }

        PieceMetadatas finalParentMetaDatas =  finalParent.AddComponent<PieceMetadatas>();
        finalParentMetaDatas.IsExcentered = parentPieceObjectToBeClonedMetaDataScript.IsExcentered;
        finalParentMetaDatas.HasSpecificRotationBehaviour = parentPieceObjectToBeClonedMetaDataScript.HasSpecificRotationBehaviour;
        finalParentMetaDatas.MaxRotateAmplitude = parentPieceObjectToBeClonedMetaDataScript.MaxRotateAmplitude;

        finalParent.name = "Simulated => " + parentPieceObjectToBeCloned.name;
        return finalParent;
    }

    public static EasyModeBehaviour FetchEasyModeBehaviourScript(GameObject parentPiece)
    {
        return parentPiece.GetComponent<EasyModeBehaviour>();
    }

    public static NormalModeBehaviour FetchNormalModeBehaviourScript(GameObject parentPiece)
    {
        return parentPiece.GetComponent<NormalModeBehaviour>();
    }

    public static ComputerPlayerBehaviour FetchCorrespondingPlayerBehaviourScript(GameObject parentPiece, int playerId)
    {
        string difficulty = ApplicationUtils.GetPlayerDifficultyMode(playerId);

        if (difficulty == DifficultyConstant.EASY_MODE)
        {
            return FetchEasyModeBehaviourScript(parentPiece);
        }
        else if (difficulty == DifficultyConstant.NORMAL_MODE)
        {
            return FetchNormalModeBehaviourScript(parentPiece);
        }
        else
        {
            return null;
        }

    }
}
