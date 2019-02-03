using UnityEngine;

public class PlayerPieceController : PieceController {

    // Use this for initialization
    void Start ()
    {
        PieceRotationSpeed = 0.4f;
        IsMoving = true;
        this.gameObjectTransform = this.gameObject.GetComponent<Transform>();
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
                    newPosition = this.gameObjectTransform.position + Vector3.right;
                    this.MoveObjectToNewPosition(newPosition);
                }
            }
            else if (Input.GetKey(DetectPlayerMovement(DirectionEnum.Direction.LEFT)))
            {
                if (!this.IsMoveForbiden(KeyCode.LeftArrow))
                {
                    newPosition = this.gameObjectTransform.position + Vector3.left;
                    this.MoveObjectToNewPosition(newPosition);
                }
            }

            if (Input.GetKeyDown(DetectPlayerRotation(DirectionEnum.Direction.RIGHT)))
            {
                bool isClockwise = true;
                if (MovementUtils.IsRotationPossible(this.maxRotateAmplitude, this.gameObject))
                {
                    RotateObject(isClockwise);
                }
            }
            else if(Input.GetKeyDown(DetectPlayerRotation(DirectionEnum.Direction.LEFT)))
            {
                bool isClockwise = false;
                if (MovementUtils.IsRotationPossible(this.maxRotateAmplitude, this.gameObject))
                {
                    this.RotateObject(isClockwise);
                }
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


    private KeyCode DetectPlayerMovement(DirectionEnum.Direction direction)
    {

        KeyCode expectedKey = 0;

        switch (this.OwnerId)
        {
            case (int)PlayerEnum.PlayerId.PLAYER_1 :

                if(direction == DirectionEnum.Direction.LEFT)
                {
                    expectedKey = KeyCode.Q;
                }
                else if(direction == DirectionEnum.Direction.RIGHT)
                {
                    expectedKey = KeyCode.D;
                }
                else if(direction == DirectionEnum.Direction.DOWN)
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
        PieceMetadatas pieceMetadatas = this.GetComponent<PieceMetadatas>();

        if (!isClockwise)
        {
            yAxeRotation *= -1;
        }

        //Wanted rotation calculation 
        Quaternion newrotation = Quaternion.Euler(new Vector3(Quaternion.identity.x, yAxeRotation, Quaternion.identity.z));
        //The from rotation
        Quaternion originRotation = this.gameObjectTransform.rotation;
        //The to rotation
        Quaternion destinationRotation = originRotation * newrotation;
        //Rotate smoothly
        this.gameObjectTransform.rotation = Quaternion.Slerp(originRotation, destinationRotation, Mathf.Clamp(Time.time * PieceRotationSpeed, 0f, 1f));

        if (pieceMetadatas.HasSpecificRotationBehaviour)
        {
            float currentYRotationValue = this.gameObjectTransform.rotation.eulerAngles.y;

            if (currentYRotationValue == 90f || currentYRotationValue == 270f)
            {
                this.gameObjectTransform.position = this.gameObjectTransform.position + (Vector3.right / 2);
            }
            else
            {
                this.gameObjectTransform.position = this.gameObjectTransform.position + (Vector3.left / 2);
            }
        }

        Instantiate(pieceSwingEffect, this.gameObjectTransform.position, Quaternion.identity);
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