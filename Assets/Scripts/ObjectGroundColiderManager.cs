using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ObjectGroundColiderManager : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {

        if (other.collider.CompareTag("Piece") && this.IsCollisionAccepted())
        {

            GameObject gameManagerObject = GameObject.FindGameObjectWithTag("GameManager");
            GameManager gameManager = gameManagerObject.GetComponent<GameManager>();

            PieceMovement pieceMovementScript = other.collider.GetComponentInParent<PieceMovement>();
            bool contactFromBelow = this.IsContactFromBelow(other);

            if (pieceMovementScript.IsMoving && contactFromBelow)
            {
                pieceMovementScript.IsMoving = false;
                Rigidbody objectColidingRigidBody = other.collider.GetComponentInParent<Rigidbody>();
                objectColidingRigidBody.velocity = new Vector3(0, 0, 0);
                objectColidingRigidBody.isKinematic = true;
                this.CorrectObjectAngles(objectColidingRigidBody.gameObject);
                this.CorrectObjectPosition(objectColidingRigidBody.gameObject);
                gameManager.GameMap = this.UpdateMapDatasForObject(objectColidingRigidBody.gameObject, gameManager.GameMap);
                gameManager.DestroyObjectLines();
                gameManager.IsReadyToSpawnObject = true;
            }
        }
        else
        {
            return;
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

    private PositionMapElement[,] UpdateMapDatasForObject(GameObject parentObject, PositionMapElement[,] positionMap)
    {

        Transform[] childrenTransform = parentObject.GetComponentsInChildren<Transform>();

        foreach (Transform transform in childrenTransform)
        {
            
            int linePosition = (int)Math.Round(transform.position.z - 0.5f);
            int columnPosition = (int)Math.Round(transform.position.x - 0.5f);
            positionMap[linePosition, columnPosition].IsOccupied = true;

            if (parentObject != transform.gameObject || (transform.parent == null))
            {
                positionMap[linePosition, columnPosition].CurrentMapElement = transform.gameObject;
            }
            
            transform.gameObject.layer = LayerMask.NameToLayer("DestroyablePiece");
        }

        return positionMap;
    }

    private void CorrectObjectAngles(GameObject gameObject)
    {
        float xAngle = CorrectObjectAngleValue(gameObject.transform.rotation.eulerAngles.x);
        float yAngle = CorrectObjectAngleValue(gameObject.transform.rotation.eulerAngles.y);
        float zAngle = CorrectObjectAngleValue(gameObject.transform.rotation.eulerAngles.z);
        gameObject.transform.localEulerAngles = new Vector3 (xAngle, yAngle, zAngle);
    }

    private float CorrectObjectAngleValue(float eulerAngle)
    {
        float roundedValue = Mathf.Floor(eulerAngle);
        bool isNegative = false;

        if(roundedValue < 0)
        {
            roundedValue *= -1;
            isNegative = true;
        }

        if(roundedValue <= 360f && roundedValue > 270f)
        {
            roundedValue = 360f;
        }
        else if(roundedValue <= 270f && roundedValue > 180f)
        {
            roundedValue = 270f;
        }
        else if(roundedValue <= 180f && roundedValue > 90f)
        {
            roundedValue = 180f;
        }
        else if (roundedValue <= 90f && roundedValue > 45f)
        {
            roundedValue = 90f;
        }
        else if(roundedValue <= 45f && roundedValue >= 0f)
        {
            roundedValue = 0f;
        }

        return isNegative ? roundedValue * -1 : roundedValue;
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

    public void WriteMapContentOnConsole(PositionMapElement[,] positionMap)
    {
       
        String line = "";

        for (int k = positionMap.GetLength(0) - 1; k >= 0; k--)
        {
            String lineNumber = "";

            if(k.ToString().Length == 1)
            {
                lineNumber = 0 + k.ToString() + ",";
            }
            else
            {
                lineNumber = k.ToString() + ",";
            }

            line += lineNumber;

            for (int l = 0; l < positionMap.GetLength(1); l++)
            {
                PositionMapElement currentElement = positionMap[k, l];

                if (currentElement.IsOccupied)
                {
                    line += "O";
                }
                else
                {
                    line += "X";
                }

                if (l == positionMap.GetLength(1) - 1)
                {
                    line += (";" + Environment.NewLine);
                }
                else
                {
                    line += ",";
                }
            }

        }

        Debug.Log(line);
       
    }
}
