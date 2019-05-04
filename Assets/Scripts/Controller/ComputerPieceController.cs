using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComputerPieceController : PieceController
{
    private IaData iaData;
    private ComputerPlayerBehaviour computerPlayerBehaviour;

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
        this.ComputerPlayerBehaviour = PieceUtils.FetchCorrespondingPlayerBehaviourScript(this.gameObject, this.OwnerId);
        this.ComputerPlayerBehaviour.enabled = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (this.IsPieceValidForPlay())
        {

            if (this.HasNoTarget())
            {
                this.IaData = this.ComputerPlayerBehaviour.CalculateAction(this.gameObject, this.OwnerId);
            }

            this.elapsedTime += Time.deltaTime;

            Vector3 newGameObjectVelocity = new Vector3();

            if (!this.HasPieceReachedTargetRotation())
            {
                this.RotateTowardTargetRotation();
            }

            if (!this.HasPieceReachedTargetPosition())
            {
                this.MoveTowardTargetPosition();
            }

            if(this.HasPieceReachedTargetPosition() && this.HasPieceReachedTargetRotation())
            {
                this.FallDownOnTargetPosition();
            }
            else
            {
                newGameObjectVelocity = new Vector3(this.gameObJectRigidBody.velocity.x, this.gameObJectRigidBody.velocity.y, -0.5f * GameUtils.FetchPlayerPieceSpeed(OwnerId));

                this.gameObJectRigidBody.velocity = newGameObjectVelocity;
            }

        }
    }

    private bool IsPieceValidForPlay()
    {
        PieceMetadatas pieceMetadataScript =  this.gameObject.GetComponent<PieceMetadatas>();
        bool isNotForseeObject = !this.gameObject.CompareTag(TagConstants.TAG_NAME_PLAYER_1_FORESEE_PIECE) || !this.gameObject.CompareTag(TagConstants.TAG_NAME_PLAYER_2_FORESEE_PIECE);
        bool isPiecePlayable = pieceMetadataScript.IsPieceReady;
        bool isInDeletingState = GameUtils.FetchPlayersDeletingLinesState(this.OwnerId);
        return isNotForseeObject && isPiecePlayable && this.IsMoving && !this.gameObJectRigidBody.isKinematic && !isInDeletingState;
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

    private bool HasPieceReachedTargetRotation()
    {
        float currentObjectYrotation = this.gameObject.transform.rotation.eulerAngles.y;
        float targetYrotation = this.IaData.TargetRotation.eulerAngles.y;

        return currentObjectYrotation == targetYrotation;
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

    private void RotateTowardTargetRotation()
    {
        if (!this.IsRotateForbiden())
        {
            this.RotateObject(true);
        }
    }

    private void RotateObject(bool isClockwise)
    {
        float yAxeRotation = MovementUtils.rotationAmount;
        float maxRotationAmount = MovementUtils.rotationMaxValue;
        PieceMetadatas pieceMetadatas = this.GetComponent<PieceMetadatas>();

        if (!isClockwise)
        {
            yAxeRotation *= -1;
            maxRotationAmount *= -1;
        }
        
        yAxeRotation += Mathf.Round(this.transform.rotation.eulerAngles.y);

        yAxeRotation = Mathf.Clamp(yAxeRotation, MovementUtils.rotationMinValue, maxRotationAmount);

        if (yAxeRotation == 360f || yAxeRotation == -360f)
        {
            yAxeRotation = MovementUtils.rotationMinValue;
        }

        /*= Quaternion.Slerp(originRotation, destinationRotation, Mathf.Clamp(Time.time * PieceRotationSpeed, 0f, 1f));*/
        this.transform.rotation = Quaternion.AngleAxis(yAxeRotation, Vector3.up);

        if (pieceMetadatas.HasSpecificRotationBehaviour)
        {
            float currentYRotationValue = this.transform.rotation.eulerAngles.y;

            if (currentYRotationValue == 90f || currentYRotationValue == 270f)
            {
                this.transform.position = this.transform.position + (Vector3.right / 2);
            }
            else
            {
                this.transform.position = this.transform.position + (Vector3.left / 2);
            }
        }

        Instantiate(pieceSwingEffect, this.transform.position, Quaternion.identity);
    }

    private void FallDownOnTargetPosition()
    {
        float verticalMove = -1f;

        Vector3 newGameObjectVelocity = new Vector3();

        newGameObjectVelocity = new Vector3(gameObJectRigidBody.velocity.x, gameObJectRigidBody.velocity.y, verticalMove * GameUtils.FetchPlayerPieceSpeed(OwnerId));

        this.gameObJectRigidBody.velocity = newGameObjectVelocity;
    }

    private bool IsRotateForbiden()
    {
        return !MovementUtils.IsRotationPossible(this.gameObject);
    }

    private bool IsMoveForbiden(Vector3 movementDirection)
    {
        return !MovementUtils.IsMovementPossible(movementDirection, this.gameObject);
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

    public ComputerPlayerBehaviour ComputerPlayerBehaviour
    {
        get
        {
            return computerPlayerBehaviour;
        }

        set
        {
            computerPlayerBehaviour = value;
        }
    }
}
