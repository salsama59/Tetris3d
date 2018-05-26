using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ObjectGroundColiderManager : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        
        //if a piece child collide with sommething other than the background
        if (other.collider.CompareTag("PieceChild") && this.IsCollisionAccepted())
        {

            PieceMovement parentPieceMovementScript = null;

            if (other.collider.transform.parent == null && other.collider.CompareTag("PieceChild") && this.transform.parent != null && this.CompareTag("PieceChild"))
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
                if(this.IsContactFromBelow(other))
                {
                    GameObject gameManagerObject = GameObject.FindGameObjectWithTag("GameManager");
                    GameManager gameManagerScript = gameManagerObject.GetComponent<GameManager>();
                    parentPieceMovementScript.IsMoving = false;
                    Rigidbody objectColidingParentRigidBody = other.collider.GetComponentInParent<Rigidbody>();
                    objectColidingParentRigidBody.velocity = new Vector3(0, 0, 0);
                    objectColidingParentRigidBody.isKinematic = true;
                    this.CorrectObjectAngles(objectColidingParentRigidBody.gameObject);
                    this.CorrectObjectPosition(objectColidingParentRigidBody.gameObject);
                    this.UpdateMapDatasForObject(objectColidingParentRigidBody.gameObject, gameManagerScript.GameMap);
                    gameManagerScript.CleanUpPieceObject(objectColidingParentRigidBody.gameObject);
                    gameManagerScript.DestroyObjectLines();
                    //test for the game over requirements
                    if (gameManagerScript.IsGameOver())
                    {
                        gameManagerScript.GameOver();
                        return;
                    }
                    gameManagerScript.IsReadyToSpawnObject = true;
                }
            }
        }

    }

    private bool IsCollisionAccepted()
    {
        return !this.gameObject.CompareTag("Background");
    }

    private void CorrectObjectPosition(GameObject objectColliding)
    {
        GameObject gameManagerObject = GameObject.FindGameObjectWithTag("GameManager");
        GameManager gameManager = gameManagerObject.GetComponent<GameManager>();
        PositionMapElement[,] positionMap = gameManager.GameMap;

        for (int i = 0; i < positionMap.GetLength(0); i++)
        {
            for(int j = 0; j  < positionMap.GetLength(1); j++)
            {
                if(objectColliding.transform.position.x < j + 1 && objectColliding.transform.position.x > j)
                {
                    if(objectColliding.transform.position.z < i + 1 && objectColliding.transform.position.z > i)
                    {
                        objectColliding.transform.position = positionMap[i, j].Position;
                    }
                }
            }
        }
    }

    private void UpdateMapDatasForObject(GameObject parentObject, PositionMapElement[,] positionMap)
    {

        Transform[] childrenTransform = parentObject.GetComponentsInChildren<Transform>();

        foreach (Transform childTransform in childrenTransform)
        {
            int linePosition = (int)Math.Round(childTransform.position.z - 0.5f);
            int columnPosition = (int)Math.Round(childTransform.position.x - 0.5f);
            positionMap[linePosition, columnPosition].IsOccupied = true;
            positionMap[linePosition, columnPosition].CurrentMapElement = childTransform.gameObject;
            childTransform.gameObject.layer = LayerMask.NameToLayer("DestroyablePiece");
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
            
            if (Physics.Raycast(childTransform.position, new Vector3(0f, 0f, -1f), out hitInfo, 3f, LayerMask.GetMask("DestroyablePiece", "ArenaWall"), QueryTriggerInteraction.Ignore))
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
