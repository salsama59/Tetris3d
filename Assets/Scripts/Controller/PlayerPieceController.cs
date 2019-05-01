using UnityEngine;

public class PlayerPieceController : PieceController {

    public override void Awake()
    {
        base.Awake();
    }

    // Use this for initialization
    void Start ()
    {
        PieceRotationSpeed = 0.4f;
        IsMoving = true;
        this.gameObJectRigidBody = this.gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate ()
    {
        if (IsMoving)
        {
            elapsedTime += Time.deltaTime;
            float verticalMove = -1f;

            Vector3 newGameObjectVelocity = new Vector3();

            Vector3 newPosition = new Vector3();

            if (Input.GetKey(DetectPlayerMovement(DirectionEnum.Direction.RIGHT)))
            {
                if(!this.IsMoveForbiden(KeyCode.RightArrow))
                {
                    newPosition = this.transform.position + Vector3.right;
                    this.MoveObjectToNewPosition(newPosition);
                }
            }
            else if (Input.GetKey(DetectPlayerMovement(DirectionEnum.Direction.LEFT)))
            {
                if (!this.IsMoveForbiden(KeyCode.LeftArrow))
                {
                    newPosition = this.transform.position + Vector3.left;
                    this.MoveObjectToNewPosition(newPosition);
                }
            }

            if (Input.GetKey(DetectPlayerRotation(DirectionEnum.Direction.RIGHT)) && !this.IsRotationLocked)
            {
                bool isClockwise = true;
                if (MovementUtils.IsRotationPossible(this.gameObject))
                {
                    this.RotateObject(isClockwise);
                }

                this.IsRotationLocked = true;
            }
            else if(Input.GetKey(DetectPlayerRotation(DirectionEnum.Direction.LEFT)) && !this.IsRotationLocked)
            {
                bool isClockwise = false;
                if (MovementUtils.IsRotationPossible(this.gameObject))
                {
                    this.RotateObject(isClockwise);
                }

                this.IsRotationLocked = true;
            }

            if ((Input.GetKeyUp(DetectPlayerRotation(DirectionEnum.Direction.LEFT)) || Input.GetKeyUp(DetectPlayerRotation(DirectionEnum.Direction.RIGHT))) && this.IsRotationLocked)
            {
                this.IsRotationLocked = false;
            }

            if (Input.GetKey(DetectPlayerMovement(DirectionEnum.Direction.DOWN)))
            {
                newGameObjectVelocity = new Vector3(gameObJectRigidBody.velocity.x, gameObJectRigidBody.velocity.y, verticalMove * GameUtils.FetchPlayerPieceSpeed(OwnerId));
            }
            else
            {
                newGameObjectVelocity = new Vector3(this.gameObJectRigidBody.velocity.x, this.gameObJectRigidBody.velocity.y, -0.5f * GameUtils.FetchPlayerPieceSpeed(OwnerId));
            }

            this.gameObJectRigidBody.velocity = newGameObjectVelocity;
        }
    }

    private KeyCode DetectPlayerRotation(DirectionEnum.Direction direction)
    {

        KeyCode expectedKey = 0;

        switch (this.OwnerId)
        {
            case (int)PlayerEnum.PlayerId.PLAYER_1:

                if (direction == DirectionEnum.Direction.LEFT)
                {
                    expectedKey = KeyCode.LeftAlt;
                }
                else if (direction == DirectionEnum.Direction.RIGHT)
                {
                    expectedKey = KeyCode.Space;
                }
                break;

            case (int)PlayerEnum.PlayerId.PLAYER_2:

                if (direction == DirectionEnum.Direction.LEFT)
                {
                    expectedKey = KeyCode.RightAlt;
                }
                else if (direction == DirectionEnum.Direction.RIGHT)
                {
                    expectedKey = KeyCode.RightControl;
                }
                break;

            default:
                break;
        }

        return expectedKey;
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

        //Rotate
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

    private bool IsMoveForbiden(KeyCode keyPushed)
    {
        Vector3 movementDirection = new Vector3();

        switch (keyPushed)
        {
            case KeyCode.RightArrow:
                movementDirection = Vector3.right;
                break;
            case KeyCode.LeftArrow:
                movementDirection = Vector3.left;
                break;
            default:
                break;
        }

        return !MovementUtils.IsMovementPossible(movementDirection, this.gameObject);
    }
}