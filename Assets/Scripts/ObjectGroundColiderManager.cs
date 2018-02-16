using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGroundColiderManager : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        Debug.Log("now the piece : " + other.collider.name + " is colliding");
        Debug.Log("tag is : " + other.collider.tag);

        if (other.collider.CompareTag("Piece"))
        {
            
            PieceMovement pieceMovementScript = other.collider.GetComponent<PieceMovement>();

            bool contactFromBelow = this.IsContactFromBelow(other);

            Debug.Log("piece is moving : " + pieceMovementScript.IsMoving);
            Debug.Log(this.gameObject.name + " is below " + other.gameObject.name + " : " + contactFromBelow);
            if (pieceMovementScript.IsMoving && contactFromBelow)
            {

                Debug.Log("So pieceMovement variable Ismoving was : " + pieceMovementScript.IsMoving);
                pieceMovementScript.IsMoving = false;

                Debug.Log("So pieceMovement variable Ismoving is now : " + pieceMovementScript.IsMoving);

                Rigidbody objectColidingRigidBody = other.collider.GetComponent<Rigidbody>();
                
                Debug.Log("velocity was : " + objectColidingRigidBody.velocity);
                objectColidingRigidBody.velocity = new Vector3(0, 0, 0);
                Debug.Log("velocity is now : " + objectColidingRigidBody.velocity);

                objectColidingRigidBody.isKinematic = true;

                this.CorrectObjectAngles(other.collider.gameObject);

                this.CorrectObjectPosition(pieceMovementScript, other.collider.gameObject);

            }

        }
        else if(other.collider.CompareTag("Ground"))
        {
            return;
        }

        GameObject gameManagerObject = GameObject.FindGameObjectWithTag("GameManager");
        GameManager gameManager = gameManagerObject.GetComponent<GameManager>();

        gameManager.IsReadyToSpawnObject = true;

    }

    private void CorrectObjectPosition(PieceMovement pieceMovementScript, GameObject objectColliding)
    {

        objectColliding.transform.position = pieceMovementScript.CurrentPosition;

    }

    private void CorrectObjectAngles(GameObject gameObject)
    {

        float xAngle = CorrectObjectAngleValue(gameObject.transform.rotation.eulerAngles.x);
        Debug.Log("angle de rotation y actuel : " + gameObject.transform.rotation.eulerAngles.y);
        float yAngle = CorrectObjectAngleValue(gameObject.transform.rotation.eulerAngles.y);
        Debug.Log("angle de rotation y après correction : " + yAngle);
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

        Debug.Log(otherCollision.gameObject.name + " size is : " + this.GetObjectSize(otherCollision.gameObject));

        float objectMinSidePosition = otherCollision.gameObject.transform.position.x - (objectHalfSize.x * objectScale.x);
        float objectMaxSidePosition = otherCollision.gameObject.transform.position.x + (objectHalfSize.x * objectScale.x);

        Debug.Log("there is : " + otherCollision.contacts.Length + " colision contact point");

        Debug.Log("this is the collisions positions : ");

        foreach (ContactPoint contact in otherCollision.contacts)
        {
            Debug.Log(contact.point);
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
