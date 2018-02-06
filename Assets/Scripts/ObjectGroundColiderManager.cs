using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGroundColiderManager : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Piece"))
        {
            PieceMovement pieceMovementScript = other.GetComponent<PieceMovement>();
            pieceMovementScript.IsMoving = false;

            Rigidbody objectColidingRigidBody = other.GetComponent<Rigidbody>();
            objectColidingRigidBody.isKinematic = true;
        }

        GameObject gameManagerObject = GameObject.FindGameObjectWithTag("GameManager");
        GameManager gameManager = gameManagerObject.GetComponent<GameManager>();

        gameManager.IsReadyToSpawnObject = true;

    }
}
