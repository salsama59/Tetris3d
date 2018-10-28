using UnityEngine;

public class PieceMovement : MonoBehaviour {
    Rigidbody gameObJectRigidBody;
    Transform gameObjectTransform;
    private GameObject field;
    private bool isMoving;
    private float elapsedTime;
    public float targetElapsedtime;
    public float timeToMoveToward;
    public float maxRotateAmplitude;
    private float pieceRotationSpeed;
    private GameManager gameManagerInstance;
    public int ownerId;
    public GameObject pieceSwingEffect;
    public enum Direction {RIGHT, LEFT, DOWN};

    // Use this for initialization
    void Start ()
    {
        PieceRotationSpeed = 0.4f;
        gameManagerInstance = FindObjectOfType<GameManager>();
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

            if (Input.GetKey(DetectPlayerMovement(Direction.RIGHT)))
            {
                if(!this.IsMoveForbiden(KeyCode.RightArrow))
                {
                    newPosition = this.gameObjectTransform.position + Vector3.right;
                    this.MoveObjectToNewPosition(newPosition);
                }
            }
            else if (Input.GetKey(DetectPlayerMovement(Direction.LEFT)))
            {
                if (!this.IsMoveForbiden(KeyCode.LeftArrow))
                {
                    newPosition = this.gameObjectTransform.position + Vector3.left;
                    this.MoveObjectToNewPosition(newPosition);
                }
            }

            if (Input.GetKeyDown(DetectPlayerRotation(Direction.RIGHT)))
            {
                bool isClockwise = true;
                if (MovementUtils.IsRotationPossible(this.maxRotateAmplitude, this.gameObject))
                {
                    RotateObject(isClockwise);
                }
            }
            else if(Input.GetKeyDown(DetectPlayerRotation(Direction.LEFT)))
            {
                bool isClockwise = false;
                if (MovementUtils.IsRotationPossible(this.maxRotateAmplitude, this.gameObject))
                {
                    this.RotateObject(isClockwise);
                }
            }

            if (Input.GetKey(DetectPlayerMovement(Direction.DOWN)))
            {
                newGameObjectVelocity = new Vector3(gameObJectRigidBody.velocity.x, gameObJectRigidBody.velocity.y, verticalMove * this.GetPieceMovementSpeed());
            }
            else
            {
                newGameObjectVelocity = new Vector3(this.gameObJectRigidBody.velocity.x, this.gameObJectRigidBody.velocity.y, -0.5f * this.GetPieceMovementSpeed());
            }

            this.gameObJectRigidBody.velocity = newGameObjectVelocity;
        }
    }


    private KeyCode DetectPlayerMovement(Direction direction)
    {

        KeyCode expectedKey = 0;

        switch (this.OwnerId)
        {
            case (int)GameManager.PlayerId.PLAYER_1 :

                if(direction == Direction.LEFT)
                {
                    expectedKey = KeyCode.Q;
                }
                else if(direction == Direction.RIGHT)
                {
                    expectedKey = KeyCode.D;
                }
                else if(direction == Direction.DOWN)
                {
                    expectedKey = KeyCode.W;
                }
                
                break;

            case (int)GameManager.PlayerId.PLAYER_2:

                if (direction == Direction.LEFT)
                {
                    expectedKey = KeyCode.LeftArrow;
                }
                else if (direction == Direction.RIGHT)
                {
                    expectedKey = KeyCode.RightArrow;
                }
                else if (direction == Direction.DOWN)
                {
                    expectedKey = KeyCode.DownArrow;
                }

                break;

            default:
                break;
        }

        return expectedKey;
    }

    private KeyCode DetectPlayerRotation(Direction direction)
    {

        KeyCode expectedKey = 0;

        switch (this.OwnerId)
        {
            case (int)GameManager.PlayerId.PLAYER_1:

                if (direction == Direction.LEFT)
                {
                    expectedKey = KeyCode.LeftAlt;
                }
                else if (direction == Direction.RIGHT)
                {
                    expectedKey = KeyCode.Space;
                }
                break;

            case (int)GameManager.PlayerId.PLAYER_2:

                if (direction == Direction.LEFT)
                {
                    expectedKey = KeyCode.RightAlt;
                }
                else if (direction == Direction.RIGHT)
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

    private void MoveObjectToNewPosition(Vector3 newPosition)
    {
        if (elapsedTime >= targetElapsedtime)
        {
            elapsedTime = 0;
            this.gameObjectTransform.position = Vector3.Lerp(this.gameObjectTransform.position, newPosition, timeToMoveToward);
        }
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

    private float GetPieceMovementSpeed()
    {
        return gameManagerInstance.PlayersPiecesMovementSpeed[this.OwnerId];
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