using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PieceMovement : MonoBehaviour {
    private const float rotationMaxValue = 360f;
    private const float rotationAmount = 90f;
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

                if (!this.IsRotateForbiden())
                {
                    RotateObject(isClockwise);
                }
            }
            else if(Input.GetKeyDown(DetectPlayerRotation(Direction.LEFT)))
            {
                bool isClockwise = false;
                if (!this.IsRotateForbiden())
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
        float yAxeRotation = rotationAmount;
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
        List<bool> rayCastHits = new List<bool>(); 
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

        float spherePositionAdjustment = movementDirection.x * 0.5f;

        Transform[] childrenTransform = this.gameObject.GetComponentsInChildren<Transform>();

        foreach (Transform childTransform in childrenTransform)
        {
            RaycastHit infos;
            Vector3 sphereOrigin = new Vector3(childTransform.position.x - spherePositionAdjustment, childTransform.position.y, childTransform.position.z);
            bool hasHitten = Physics.SphereCast(sphereOrigin, 0.5f, movementDirection, out infos, 1f, LayerMask.GetMask(LayerConstants.LAYER_NAME_DESTROYABLE_PIECE, LayerConstants.LAYER_NAME_ARENA_WALL));
            rayCastHits.Add(hasHitten);
        }

        //Check if all raycast hit are false (return true if all hit are false but return false otherwise) 
        bool movementAllowed = rayCastHits.ToArray().All(hit => hit == false);
        
        return !movementAllowed;

    }

    private bool IsRotateForbiden()
    {
        List<Vector3> nodes = this.CalculatePoints();
        return this.SweepHasHit(nodes);
    }

    private List<Vector3> CalculatePoints()
    {
        float radius = maxRotateAmplitude;
        List<Vector3> nodes = new List<Vector3>();
        float calcAngle = 0;
        int segments = 12;
        float curveAmount = rotationMaxValue;

        // Calculate Arc on X-Z    
        for (int i = 0; i < segments + 1; i++)
        {
            float posX = Mathf.Cos(calcAngle * Mathf.Deg2Rad) * radius;
            float posZ = Mathf.Sin(calcAngle * Mathf.Deg2Rad) * radius;
            nodes.Add(transform.position + (transform.right * posX) + (transform.forward * posZ));
            calcAngle += curveAmount / (float)segments;
        }

        return nodes;
    }

    private bool SweepHasHit(List<Vector3> nodes)
    {
        RaycastHit hit;

        for (int i = 0; i < nodes.Count - 1; i++)
        {
            if(Physics.Linecast(nodes[i], nodes[i + 1], out hit, LayerMask.GetMask(LayerConstants.LAYER_NAME_DESTROYABLE_PIECE, LayerConstants.LAYER_NAME_ARENA_WALL), QueryTriggerInteraction.Ignore))
            {
                return true;
            }
            
        }

        return false;
    }

    private void DrawLines(List<Vector3> nodes)
    {
        for (int i = 0; i < nodes.Count - 1; i++)
        {
            Debug.DrawLine(nodes[i], nodes[i + 1], Color.red, 1.5f);
        }
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