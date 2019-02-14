using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceController : MonoBehaviour {

    protected Rigidbody gameObJectRigidBody;
    protected bool isMoving;
    protected GameObject field;
    protected float pieceRotationSpeed;
    protected int ownerId;

    protected float elapsedTime;
    public float targetElapsedtime;
    public float timeToMoveToward;
    public float maxRotateAmplitude;
    public GameObject pieceSwingEffect;

    public virtual void Awake()
    {
        this.enabled = false;
    }

    protected void MoveObjectToNewPosition(Vector3 newPosition)
    {
        if (elapsedTime >= targetElapsedtime)
        {
            elapsedTime = 0;
            this.transform.position = Vector3.Lerp(this.transform.position, newPosition, timeToMoveToward);
        }
    }

    protected KeyCode DetectPlayerMovement(DirectionEnum.Direction direction)
    {

        KeyCode expectedKey = 0;

        switch (this.OwnerId)
        {
            case (int)PlayerEnum.PlayerId.PLAYER_1:

                if (direction == DirectionEnum.Direction.LEFT)
                {
                    expectedKey = KeyCode.Q;
                }
                else if (direction == DirectionEnum.Direction.RIGHT)
                {
                    expectedKey = KeyCode.D;
                }
                else if (direction == DirectionEnum.Direction.DOWN)
                {
                    expectedKey = KeyCode.W;
                }

                break;

            case (int)PlayerEnum.PlayerId.PLAYER_2:

                if (direction == DirectionEnum.Direction.LEFT)
                {
                    expectedKey = KeyCode.LeftArrow;
                }
                else if (direction == DirectionEnum.Direction.RIGHT)
                {
                    expectedKey = KeyCode.RightArrow;
                }
                else if (direction == DirectionEnum.Direction.DOWN)
                {
                    expectedKey = KeyCode.DownArrow;
                }

                break;

            default:
                break;
        }

        return expectedKey;
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
