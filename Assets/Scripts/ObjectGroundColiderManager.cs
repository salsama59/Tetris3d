using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGroundColiderManager : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        Debug.Log("now the piece : " + other.collider.name + " is passing");
        Debug.Log("tag is : " + other.collider.tag);

        if (other.collider.CompareTag("Piece"))
        {
            Debug.Log("After comparing tags : " + other.collider.tag + " is then " + "Piece?");
            
            PieceMovement pieceMovementScript = other.collider.GetComponent<PieceMovement>();


            Debug.Log("piece is moving : " + pieceMovementScript.IsMoving);
            Debug.Log(this.gameObject.name + " is below " + other.gameObject.name + " : " + this.IsContactFromBelow(other));
            if (pieceMovementScript.IsMoving && this.IsContactFromBelow(other))
            {
                Debug.Log("So pieceMovement variable Ismoving was : " + pieceMovementScript.IsMoving);
                pieceMovementScript.IsMoving = false;

                Debug.Log("So pieceMovement variable Ismoving is now : " + pieceMovementScript.IsMoving);

                Rigidbody objectColidingRigidBody = other.collider.GetComponent<Rigidbody>();
                objectColidingRigidBody.isKinematic = true;
                Debug.Log("velocity was : " + objectColidingRigidBody.velocity);
                objectColidingRigidBody.velocity = new Vector3(0, 0, 0);
                Debug.Log("velocity is now : " + objectColidingRigidBody.velocity);
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

        Collider meshRenderer = this.gameObject.GetComponent<Collider>();
        Vector3 objectHalfSize = meshRenderer.bounds.size * 0.5f;
        Vector3 objectScale = this.gameObject.transform.localScale;

        float objectMinSidePosition = this.gameObject.transform.position.x - (objectHalfSize.x * objectScale.x);
        float objectMaxSidePosition = this.gameObject.transform.position.x + (objectHalfSize.x * objectScale.x);

        Debug.Log("there is : " + otherCollision.contacts.Length + " colision contact");

        foreach (ContactPoint contact in otherCollision.contacts)
        {
            if(objectMinSidePosition < contact.point.x && objectMaxSidePosition > contact.point.x && contact.point.z > this.gameObject.transform.position.z)
            {
                return true;
            }
            
        }

        

        /*Debug.Log(this.gameObject.name + " size is : " + meshRenderer.bounds.size);

        MeshRenderer otherMeshRenderer = otherObjectColiding.GetComponent<MeshRenderer>();
        Vector3 otherObjectHalfSize = otherMeshRenderer.bounds.size * 0.5f;
        Vector3 otherObjectScale = otherObjectColiding.transform.localScale;

        Debug.Log(otherObjectColiding.name + " size is : " + otherMeshRenderer.bounds.size);

        float otherObjectMinSidePosition = otherObjectColiding.transform.position.x - (otherObjectHalfSize.x * otherObjectScale.x);
        float otherObjectMaxSidePosition = otherObjectColiding.transform.position.x + (otherObjectHalfSize.x * otherObjectScale.x);

       

        if (objectMinSidePosition <= otherObjectMinSidePosition || objectMaxSidePosition >= otherObjectMaxSidePosition)
        {
            below = true;
        }*/

        return below;

    }
}
