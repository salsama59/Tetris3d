using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGroundColiderManager : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {

        if (other.collider.CompareTag("Piece") && this.IsCollisionAccepted())
        {
           
            PieceMovement pieceMovementScript = other.collider.GetComponent<PieceMovement>();
            bool contactFromBelow = this.IsContactFromBelow(other);
            if (pieceMovementScript.IsMoving && contactFromBelow)
            {
                pieceMovementScript.IsMoving = false;
                Rigidbody objectColidingRigidBody = other.collider.GetComponent<Rigidbody>();
                objectColidingRigidBody.velocity = new Vector3(0, 0, 0);
                objectColidingRigidBody.isKinematic = true;
                this.CorrectObjectAngles(other.collider.gameObject);
                this.CorrectObjectPosition(other.collider.gameObject);
            }
        }
        else
        {
            return;
        }
        
        GameObject gameManagerObject = GameObject.FindGameObjectWithTag("GameManager");
        GameManager gameManager = gameManagerObject.GetComponent<GameManager>();
        gameManager.GameMap = this.UpdateOccupiedSpace(other.collider.gameObject, gameManager.GameMap);
        gameManager.IsReadyToSpawnObject = true;

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

    private PositionMapElement[,] UpdateOccupiedSpace(GameObject parentObject, PositionMapElement[,] positionMap)
    {
        Transform[] childrenTransform = parentObject.GetComponentsInChildren<Transform>();
        foreach (Transform transform in childrenTransform)
        {
            int linePosition = (int)(transform.position.z - 0.5f);
            int columnPosition = (int)(transform.position.x - 0.5f);
            positionMap[linePosition, columnPosition].IsOccupied = true;
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
        float roundedValue = Mathf.Round(eulerAngle);
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

        bool below = false;

        Vector3 objectHalfSize = this.GetObjectHalfsize(otherCollision.gameObject);
        Vector3 objectScale = otherCollision.gameObject.transform.localScale;

        float objectMinSidePosition = otherCollision.gameObject.transform.position.x - (objectHalfSize.x * objectScale.x);
        float objectMaxSidePosition = otherCollision.gameObject.transform.position.x + (objectHalfSize.x * objectScale.x);


        foreach (ContactPoint contact in otherCollision.contacts)
        {
            if (objectMinSidePosition < contact.point.x && objectMaxSidePosition > contact.point.x && contact.point.z > this.gameObject.transform.position.z)
            {
                return true;
            }
        }

        return below;

    }

    private Vector3 GetObjectSize(GameObject gameObject)
    {
        Vector3 objectSize = new Vector3();

        Collider[] colliders = gameObject.GetComponents<Collider>();


        foreach (var collider in colliders)
        {
            Vector3 colliderSize = collider.bounds.size;

            objectSize.x += colliderSize.x;
            objectSize.y += colliderSize.y;
            objectSize.z += colliderSize.z;
        }

        return objectSize;

    }


    private Vector3 GetObjectHalfsize(GameObject gameObject)
    {
        return this.GetObjectSize(gameObject) * 0.5f;
    }
}
