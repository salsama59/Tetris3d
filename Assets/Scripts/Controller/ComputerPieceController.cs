using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputerPieceController : PieceController
{
    private Transform targetTransform;
    private IaData iaData;

    public override void Awake()
    {
        base.Awake();
    }

    // Use this for initialization
    void Start()
    {
        
        this.PieceRotationSpeed = 0.4f;
        this.IsMoving = true;
        this.gameObJectRigidBody = this.gameObject.GetComponent<Rigidbody>();

        if (IsPieceValidForPlay())
        {
            this.IaData = AiUtils.CalculateAction(this.gameObject, this.OwnerId);
        }

    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (IsMoving && this.IsPieceValidForPlay())
        {

            this.elapsedTime += Time.deltaTime;

            Vector3 newGameObjectVelocity = new Vector3();

            if (this.HasNoTarget())
            {
                this.IaData = AiUtils.CalculateAction(this.gameObject, this.OwnerId);
            }

            if (!this.HasPieceReachedTargetPosition())
            {
                this.MoveTowardTargetPosition();
            }
            else
            {
                this.FallDownOnTargetPosition();
            }

            newGameObjectVelocity = new Vector3(this.gameObJectRigidBody.velocity.x, this.gameObJectRigidBody.velocity.y, -0.5f * GameUtils.FetchPlayerPieceSpeed(OwnerId));
            

            this.gameObJectRigidBody.velocity = newGameObjectVelocity;


            /*if (Input.GetKeyDown(DetectPlayerRotation(DirectionEnum.Direction.RIGHT)))
            {
                bool isClockwise = true;
                if (MovementUtils.IsRotationPossible(this.maxRotateAmplitude, this.gameObject))
                {
                    RotateObject(isClockwise);
                }
            }
            else if (Input.GetKeyDown(DetectPlayerRotation(DirectionEnum.Direction.LEFT)))
            {
                bool isClockwise = false;
                if (MovementUtils.IsRotationPossible(this.maxRotateAmplitude, this.gameObject))
                {
                    this.RotateObject(isClockwise);
                }
            }*/


        }

    }

    private bool IsPieceValidForPlay()
    {
        PieceMetadatas pieceMetadataScript =  this.gameObject.GetComponent<PieceMetadatas>();
        bool isNotForseeObject = !this.gameObject.CompareTag(TagConstants.TAG_NAME_PLAYER_1_FORESEE_PIECE) || !this.gameObject.CompareTag(TagConstants.TAG_NAME_PLAYER_2_FORESEE_PIECE);
        bool isPiecePlayable = pieceMetadataScript.IsPieceReady;
        return isNotForseeObject && isPiecePlayable;
    }

    private bool HasNoTarget()
    {
        return this.IaData == null;
    }

    private bool HasPieceReachedTargetPosition()
    {
        float currentObjectXposition = this.gameObject.transform.position.x;
        float targetXposition = this.IaData.TargetPosition.x;

        return currentObjectXposition == targetXposition;
    }

    private Vector3 CalculateDirection()
    {
        if (this.gameObject.transform.position.x < this.IaData.TargetPosition.x)
        {
            return Vector3.right;
        }
        else if (this.gameObject.transform.position.x > this.IaData.TargetPosition.x)
        {
            return Vector3.left;
        }
        else
        {
            return Vector3.zero;
        }
    }

    private void MoveTowardTargetPosition()
    {
        Vector3 direction = this.CalculateDirection();
        Vector3 newPosition = new Vector3();
        if (!this.IsMoveForbiden(direction))
        {
            newPosition = this.transform.position + direction;
            this.MoveObjectToNewPosition(newPosition);
        }
    }

    private void FallDownOnTargetPosition()
    {
        float verticalMove = -1f;

        Vector3 newGameObjectVelocity = new Vector3();

        newGameObjectVelocity = new Vector3(gameObJectRigidBody.velocity.x, gameObJectRigidBody.velocity.y, verticalMove * GameUtils.FetchPlayerPieceSpeed(OwnerId));

        this.gameObJectRigidBody.velocity = newGameObjectVelocity;
    }

    private bool IsMoveForbiden(Vector3 movementDirection)
    {
        return !MovementUtils.IsMovementPossible(movementDirection, this.gameObject);
    }


    public Transform TargetTransform
    {
        get
        {
            return targetTransform;
        }

        set
        {
            targetTransform = value;
        }
    }

    public IaData IaData
    {
        get
        {
            return iaData;
        }

        set
        {
            iaData = value;
        }
    }
}
