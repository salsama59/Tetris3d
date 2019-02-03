using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceController : MonoBehaviour {

    protected Rigidbody gameObJectRigidBody;
    protected Transform gameObjectTransform;
    protected bool isMoving;
    protected GameObject field;
    protected float pieceRotationSpeed;
    protected int ownerId;

    protected float elapsedTime;
    public float targetElapsedtime;
    public float timeToMoveToward;
    public float maxRotateAmplitude;
    public GameObject pieceSwingEffect;

    protected void MoveObjectToNewPosition(Vector3 newPosition)
    {
        if (elapsedTime >= targetElapsedtime)
        {
            elapsedTime = 0;
            this.gameObjectTransform.position = Vector3.Lerp(this.gameObjectTransform.position, newPosition, timeToMoveToward);
        }
    }

    public bool IsMoving
    {
        get
        {
            return isMoving;
        }

        set
        {
            isMoving = value;
        }
    }

    public GameObject Field
    {
        get
        {
            return field;
        }

        set
        {
            field = value;
        }
    }

    public float PieceRotationSpeed
    {
        get
        {
            return pieceRotationSpeed;
        }

        set
        {
            pieceRotationSpeed = value;
        }
    }

    public int OwnerId
    {
        get
        {
            return ownerId;
        }

        set
        {
            ownerId = value;
        }
    }

}
