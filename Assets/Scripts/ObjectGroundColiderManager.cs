using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ObjectGroundColiderManager : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        bool isOtherColliderHasPieceChildTag = other.collider.CompareTag(TagConstants.TAG_NAME_PLAYER_1_PIECE_CHILD) || other.collider.CompareTag(TagConstants.TAG_NAME_PLAYER_2_PIECE_CHILD);
        //if a piece child collide with sommething other than the background
        if(isOtherColliderHasPieceChildTag && this.IsCollisionAccepted())
        {
            bool isThisColliderHasPieceChildTag = this.CompareTag(TagConstants.TAG_NAME_PLAYER_1_PIECE_CHILD) || this.CompareTag(TagConstants.TAG_NAME_PLAYER_2_PIECE_CHILD);
            PieceMovement parentPieceMovementScript = null;

            if (other.collider.transform.parent == null && isOtherColliderHasPieceChildTag && this.transform.parent != null && isThisColliderHasPieceChildTag)
            {
                parentPieceMovementScript = this.GetComponentInParent<PieceMovement>();
            }
            else
            {
                parentPieceMovementScript = other.collider.GetComponentInParent<PieceMovement>();
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
                    GameObject gameManagerObject = GameObject.FindGameObjectWithTag(TagConstants.TAG_NAME_GAME_MANAGER);
                    GameManager gameManagerScript = gameManagerObject.GetComponent<GameManager>();
                    parentPieceMovementScript.IsMoving = false;
                    Rigidbody objectColidingParentRigidBody = other.collider.GetComponentInParent<Rigidbody>();
                    objectColidingParentRigidBody.velocity = Vector3.zero;
                    objectColidingParentRigidBody.isKinematic = true;
                    this.CorrectObjectAngles(objectColidingParentRigidBody.gameObject);
                    this.CorrectObjectPosition(objectColidingParentRigidBody.gameObject, parentPieceMovementScript.OwnerId, gameManagerScript);
                    this.UpdateMapDatasForObject(objectColidingParentRigidBody.gameObject, gameManagerScript, parentPieceMovementScript.OwnerId);
                    gameManagerScript.CleanUpPieceObject(objectColidingParentRigidBody.gameObject, parentPieceMovementScript.OwnerId);
                    gameManagerScript.DestroyObjectLines(parentPieceMovementScript.OwnerId);
                    //test for the game over requirements
                    if (gameManagerScript.IsGameOver(parentPieceMovementScript.OwnerId))
                    {
                        gameManagerScript.GameOver(parentPieceMovementScript.OwnerId);

                        int winnerId = parentPieceMovementScript.OwnerId == (int)GameManager.PlayerId.PLAYER_1 ? (int)GameManager.PlayerId.PLAYER_2 : (int)GameManager.PlayerId.PLAYER_1;
                        gameManagerScript.DeclareWinner(winnerId);
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

    private void CorrectObjectPosition(GameObject objectColliding, int playerId, GameManager gameManager)
    {
        PositionMapElement[,] positionMap = gameManager.PlayersPositionMap[playerId];

        for (int i = 0; i < positionMap.GetLength(0); i++)
        {
            for(int j = 0; j < positionMap.GetLength(1); j++)
            {
                this.CorrectPlayerPiecePosition(objectColliding, playerId, gameManager, positionMap, i, j);
            }
        }
    }

    private void CorrectPlayerPiecePosition(GameObject objectColliding, int playerId, GameManager gameManager, PositionMapElement[,] positionMap, int i, int j)
    {
        float positionAdjustment = 0;
        float posX = objectColliding.transform.position.x;

        if (ApplicationData.IsInMultiPlayerMode())
        {
            this.CorrectPlayerPiecePositionForTwoPlayerMode(objectColliding, playerId, gameManager, positionMap, i, j, positionAdjustment, posX);
        }
        else
        {
            this.CorrectPlayerPiecePositionForOnePlayerMode(objectColliding, playerId, gameManager, positionMap, i, j, positionAdjustment, posX);
        }
    }

    private void CorrectPlayerPiecePositionForOnePlayerMode(GameObject objectColliding, int playerId, GameManager gameManager, PositionMapElement[,] positionMap, int i, int j, float positionAdjustment, float posX)
    {
        if (playerId == (int)GameManager.PlayerId.PLAYER_1)
        {
            if (posX < j + 1 && posX > j)
            {
                if (objectColliding.transform.position.z < i + 1 && objectColliding.transform.position.z > i)
                {
                    objectColliding.transform.position = positionMap[i, j].Position;
                    return;
                }
            }
        }
    }

    private void CorrectPlayerPiecePositionForTwoPlayerMode(GameObject objectColliding, int playerId, GameManager gameManager, PositionMapElement[,] positionMap, int i, int j, float positionAdjustment, float posX)
    {
        positionAdjustment = -0.5f;
        if (posX < (gameManager.MapValueToPosition(j + 1, playerId) + positionAdjustment) && posX > (gameManager.MapValueToPosition(j, playerId) + positionAdjustment))
        {
            if (objectColliding.transform.position.z < i + 1 && objectColliding.transform.position.z > i)
            {
                objectColliding.transform.position = positionMap[i, j].Position;
                return;
            }
        }
    }

    private void UpdateMapDatasForObject(GameObject parentObject, GameManager gameManagerScript, int playerId)
    {

        Transform[] childrenTransform = parentObject.GetComponentsInChildren<Transform>();

        foreach (Transform childTransform in childrenTransform)
        {
            int linePosition = (int)Math.Round(childTransform.position.z - 0.5f);
            int columnPosition = 0;

            if (ApplicationData.playerNumber > 1)
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

    private void CorrectObjectAngles(GameObject gameObject)
    {
        PieceMetadatas objectScriptPieceMetadatas = gameObject.GetComponent<PieceMetadatas>();
        //The last rotation known is stored in the piece metadatas
        Quaternion objectQuaternion = objectScriptPieceMetadatas.CurrentRotation;
        float xAngle = objectQuaternion.eulerAngles.x;
        float yAngle = objectQuaternion.eulerAngles.y;
        float zAngle = objectQuaternion.eulerAngles.z;
        
        gameObject.transform.localEulerAngles = new Vector3 (xAngle, yAngle, zAngle);
    }

    private bool IsContactFromBelow(Collision otherCollision)
    {

        Transform[] childrenTransform = otherCollision.gameObject.GetComponentsInChildren<Transform>();

        RaycastHit hitInfo;
        
        foreach (Transform childTransform in childrenTransform)
        {
            
            if (Physics.Raycast(childTransform.position, new Vector3(0f, 0f, -1f), out hitInfo, 3f, LayerMask.GetMask(LayerConstants.LAYER_NAME_DESTROYABLE_PIECE, LayerConstants.LAYER_NAME_ARENA_WALL), QueryTriggerInteraction.Ignore))
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
