using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGroundColiderManager : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("now the piece : " + other.name + " is passing");
        Debug.Log("tag is : " + other.tag);

        if (other.CompareTag("Piece"))
        {
            Debug.Log("After comparing tags : " + other.tag + " is then " + "Piece?");
            
            PieceMovement pieceMovementScript = other.GetComponent<PieceMovement>();

            if(pieceMovementScript.IsMoving)
            {
                Debug.Log("So pieceMovement variable Ismoving was : " + pieceMovementScript.IsMoving);
                pieceMovementScript.IsMoving = false;

                Debug.Log("So pieceMovement variable Ismoving is now : " + pieceMovementScript.IsMoving);

                /*Rigidbody objectColidingRigidBody = other.GetComponent<Rigidbody>();
                //objectColidingRigidBody.isKinematic = true;
                Debug.Log("velocity was : " + objectColidingRigidBody.velocity);
                objectColidingRigidBody.velocity = new Vector3(0, 0, 0);
                Debug.Log("velocity is now : " + objectColidingRigidBody.velocity);*/
            }

        }
        else if(other.CompareTag("Ground"))
        {
            return;
        }

        GameObject gameManagerObject = GameObject.FindGameObjectWithTag("GameManager");
        GameManager gameManager = gameManagerObject.GetComponent<GameManager>();

        gameManager.IsReadyToSpawnObject = true;

    }
}
