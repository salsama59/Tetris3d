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

                Vector3 objectColligingCurrentRotation = other.collider.gameObject.transform.eulerAngles;

                Debug.Log("So " + other.gameObject.name + " rotation was" + objectColligingCurrentRotation);

                Debug.Log("So pieceMovement variable Ismoving was : " + pieceMovementScript.IsMoving);
                pieceMovementScript.IsMoving = false;

                Debug.Log("So pieceMovement variable Ismoving is now : " + pieceMovementScript.IsMoving);

                Rigidbody objectColidingRigidBody = other.collider.GetComponent<Rigidbody>();
                
                Debug.Log("So " + other.gameObject.name + " rotation is now" + objectColligingCurrentRotation);
                Debug.Log("velocity was : " + objectColidingRigidBody.velocity);
                objectColidingRigidBody.velocity = new Vector3(0, 0, 0);
                Debug.Log("velocity is now : " + objectColidingRigidBody.velocity);

                other.collider.gameObject.transform.eulerAngles = objectColligingCurrentRotation;

                objectColidingRigidBody.isKinematic = true;

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
