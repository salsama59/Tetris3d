﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ObjectGroundColiderManager : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        bool isOtherColliderHasPieceChildTag = other.CompareTag(TagConstants.TAG_NAME_PLAYER_1_PIECE_CHILD) || other.CompareTag(TagConstants.TAG_NAME_PLAYER_2_PIECE_CHILD);
        //if a piece child collide with something other than the background
        if(isOtherColliderHasPieceChildTag && this.IsCollisionAccepted())
        {
            bool isThisColliderHasPieceChildTag = this.CompareTag(TagConstants.TAG_NAME_PLAYER_1_PIECE_CHILD) || this.CompareTag(TagConstants.TAG_NAME_PLAYER_2_PIECE_CHILD);
            PieceMovement parentPieceMovementScript = null;

            if (other.transform.parent == null && isOtherColliderHasPieceChildTag && this.transform.parent != null && isThisColliderHasPieceChildTag)
            {
                parentPieceMovementScript = this.GetComponentInParent<PieceMovement>();
            }
            else
            {
                parentPieceMovementScript = other.GetComponentInParent<PieceMovement>();
            }

            if(parentPieceMovementScript == null)
            {
                return;
            }

            //If the piece are moving
            if (parentPieceMovementScript.IsMoving)
            {
                if (this.IsContactFromBelow(other))
                {
                    Rigidbody objectColidingParentRigidBody = other.GetComponentInParent<Rigidbody>();
                    if(objectColidingParentRigidBody == null)
                    {
                        return;
                    }

                    GameObject gameManagerObject = GameObject.FindGameObjectWithTag(TagConstants.TAG_NAME_GAME_MANAGER);
                    GameManager gameManagerScript = gameManagerObject.GetComponent<GameManager>();
                    parentPieceMovementScript.IsMoving = false;
                    
                    objectColidingParentRigidBody.velocity = Vector3.zero;
                    objectColidingParentRigidBody.isKinematic = true;
                    this.CorrectPiecePosition(objectColidingParentRigidBody.gameObject);
                    this.UpdateMapDatasForObject(objectColidingParentRigidBody.gameObject, gameManagerScript, parentPieceMovementScript.OwnerId);
                    gameManagerScript.CleanUpPieceObject(objectColidingParentRigidBody.gameObject, parentPieceMovementScript.OwnerId);
                    gameManagerScript.DestroyObjectLines(parentPieceMovementScript.OwnerId);
                    //test for the game over requirements
                    if (gameManagerScript.IsGameOver(parentPieceMovementScript.OwnerId))
                    {
                        gameManagerScript.GameOver(parentPieceMovementScript.OwnerId);

                        int winnerId = parentPieceMovementScript.OwnerId == (int)GameManager.PlayerId.PLAYER_1 ? (int)GameManager.PlayerId.PLAYER_2 : (int)GameManager.PlayerId.PLAYER_1;

                        if (ApplicationData.IsInMultiPlayerMode())
                        {
                            gameManagerScript.DeclareWinner(winnerId);
                        }
                        return;
                    }

                    if(!gameManagerScript.Restart)
                    {
                        gameManagerScript.PlayersSpawnAuthorisation[parentPieceMovementScript.OwnerId] = true;
                    }
                    
                }
            }
        }
    }

    private bool IsCollisionAccepted()
    {
        return !this.gameObject.CompareTag(TagConstants.TAG_NAME_FIELD_BACKGROUND);
    }

    private void CorrectPiecePosition(GameObject objectColliding)
    {
        float fractionalLimit = 1f;
        float halfFractionalLimit = 0.5f;
        float calculatedZposition = 0f;
        float currentZPosition = objectColliding.transform.position.z;
        bool isNegative = currentZPosition < 0 ? true : false;

        float fractionnalPart = Mathf.Abs(currentZPosition) - Mathf.Abs((int)currentZPosition);

        if(fractionnalPart > 0 && fractionnalPart <= halfFractionalLimit)
        {
            calculatedZposition = Mathf.Abs((int)currentZPosition) + halfFractionalLimit;
        }
        else if(fractionnalPart > 0.5f && fractionnalPart <= fractionalLimit)
        {
            calculatedZposition = Mathf.Abs((int)currentZPosition) + fractionalLimit;
        }
        else
        {
            return;
        }

        if(isNegative)
        {
            calculatedZposition *= -1;
        }

        objectColliding.transform.position = new Vector3(objectColliding.transform.position.x, objectColliding.transform.position.y, calculatedZposition);
    }

    private void UpdateMapDatasForObject(GameObject parentObject, GameManager gameManagerScript, int playerId)
    {

        Transform[] childrenTransform = parentObject.GetComponentsInChildren<Transform>();

        foreach (Transform childTransform in childrenTransform)
        {
            int linePosition = (int)Math.Round(childTransform.position.z - 0.5f);
            int columnPosition = 0;

            if (ApplicationData.IsInMultiPlayerMode())
            {
                columnPosition = gameManagerScript.PositionToMapValue(childTransform.position.x, playerId);
            }
            else
            {
                columnPosition = (int)Math.Round(childTransform.position.x - 0.5f);
            }

            gameManagerScript.PlayersPositionMap[playerId][linePosition, columnPosition].IsOccupied = true;
            gameManagerScript.PlayersPositionMap[playerId][linePosition, columnPosition].CurrentMapElement = childTransform.gameObject;
            childTransform.gameObject.layer = LayerMask.NameToLayer(LayerConstants.LAYER_NAME_DESTROYABLE_PIECE);
        }
    }

    private bool IsContactFromBelow(Collider otherCollider)
    {

        Transform[] childrenTransform = otherCollider.gameObject.GetComponentsInChildren<Transform>();

        RaycastHit hitInfo;
        
        foreach (Transform childTransform in childrenTransform)
        {
            
            if (Physics.Raycast(childTransform.position, new Vector3(0f, 0f, -1f), out hitInfo, 3f, LayerMask.GetMask(LayerConstants.LAYER_NAME_DESTROYABLE_PIECE, LayerConstants.LAYER_NAME_ARENA_WALL), QueryTriggerInteraction.Collide))
            {
                if (hitInfo.collider != null)
                {
                    Vector3 hitNormal = hitInfo.normal;
                   
                    if (hitNormal == new Vector3(0f, 0f, 1f))
                    {
                        return true;
                    }
                }

            }
        }

        return false;

    }
}
