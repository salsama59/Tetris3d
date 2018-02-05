using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGroundColiderManager : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {

        PieceMovement pieceMovementScript  = other.GetComponent<PieceMovement>();
        pieceMovementScript.IsMoving = false;
        Rigidbody objectColidingRigidBody = other.GetComponent<Rigidbody>();
        objectColidingRigidBody.isKinematic = true;

    }
}
