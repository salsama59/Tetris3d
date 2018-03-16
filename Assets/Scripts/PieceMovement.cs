using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PieceMovement : MonoBehaviour {

    Rigidbody gameObJectRigidBody;
    Transform gameObjectTransform;
    public GameObject field;
    private bool isMoving;
    public float speed;
    private float elapsedTime;
    public float targetElapsedtime;
    public float timeToMoveToward;
    public float maxRotateAmplitude;

    // Use this for initialization
    void Start ()
    {
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
            float verticalMove = Input.GetAxis("Vertical");

            Vector3 newGameObjectVelocity = new Vector3();

            Vector3 newPosition = new Vector3();

            if (Input.GetKey(KeyCode.RightArrow))
            {
                if(!this.IsMoveForbiden(KeyCode.RightArrow))
                {
                    newPosition = this.gameObjectTransform.position + Vector3.right;
                    this.MoveObjectToNewPosition(newPosition);
                }
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                if (!this.IsMoveForbiden(KeyCode.LeftArrow))
                {
                    newPosition = this.gameObjectTransform.position + Vector3.left;
                    this.MoveObjectToNewPosition(newPosition);
                }
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                bool isClockwise = true;

                if (!this.IsRotateForbiden())
                {
                    RotateObject(isClockwise);
                }
            }
            else if(Input.GetKeyDown(KeyCode.LeftAlt))
            {
                bool isClockwise = false;
                if (!this.IsRotateForbiden())
                {
                    this.RotateObject(isClockwise);
                }
            }

            if (Input.GetKey(KeyCode.DownArrow))
            {
                newGameObjectVelocity = new Vector3(this.gameObJectRigidBody.velocity.x, this.gameObJectRigidBody.velocity.y, verticalMove * this.speed);
            }
            else
            {
                newGameObjectVelocity = new Vector3(this.gameObJectRigidBody.velocity.x, this.gameObJectRigidBody.velocity.y, -0.5f * this.speed);
            }

            this.gameObJectRigidBody.velocity = newGameObjectVelocity;
        }
    }

    private void RotateObject(bool isClockwise)
    {
        Quaternion newrotation;
        Quaternion rotationPreview;

        if (isClockwise)
        {
            newrotation = Quaternion.Euler(0f, 90f, 0f);
            rotationPreview = this.gameObjectTransform.rotation * newrotation;
            if (rotationPreview.y == 360f)
            {
                newrotation = Quaternion.Euler(0f, 0f, 0f);
            }
        }
        else
        {
            newrotation = Quaternion.Euler(0f, -90f, 0f);
            rotationPreview = this.gameObjectTransform.rotation * newrotation;
            if (rotationPreview.y == -360f)
            {
                newrotation = Quaternion.Euler(0f, 0f, 0f);
            }
        }
        this.gameObjectTransform.rotation *= newrotation;
    }

    private void MoveObjectToNewPosition(Vector3 newPosition)
    {
        if (elapsedTime >= targetElapsedtime)
        {
            elapsedTime = 0;
            this.gameObjectTransform.position = Vector3.Lerp(this.gameObjectTransform.position, newPosition, timeToMoveToward);
        }
    }

    private Vector3 GetObjectSize(GameObject gameObject)
    {
        Vector3 objectSize = new Vector3();

        Collider[] colliders = gameObject.GetComponents<Collider>();


        foreach (var collider in colliders)
        {
            Vector3 colliderSize = collider.bounds.size;

            objectSize.x += colliderSize.x;
            objectSize.y += colliderSize.y;
            objectSize.z += colliderSize.z;
        }

        return objectSize;

    }

    private Vector3 GetObjectHalfsize(GameObject gameObject)
    {
        return this.GetObjectSize(gameObject) * 0.5f;
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
            bool hasHitten = Physics.SphereCast(sphereOrigin, 0.5f, movementDirection, out infos, 1f, LayerMask.GetMask("DestroyablePiece", "ArenaWall"));
            rayCastHits.Add(hasHitten);
        }

        //Check if all raycast hit are false (return true if all hit are false but return false otherwise) 
        bool movementPossible = rayCastHits.ToArray().All(hit => hit == false);
        
        return !movementPossible;

    }

    private bool IsRotateForbiden()
    {
        List<Vector3> nodes = this.CalculatePoints();
        //this.DrawLines(nodes);
        return this.SweepHasHit(nodes);
    }

    private List<Vector3> CalculatePoints()
    {
        float radius = maxRotateAmplitude;
        List<Vector3> nodes = new List<Vector3>();
        float calcAngle = 0;
        int segments = 12;
        float curveAmount = 360f;

        // Calculate Arc on Y-Z    
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
            if(Physics.Linecast(nodes[i], nodes[i + 1], out hit, LayerMask.GetMask("DestroyablePiece", "ArenaWall"), QueryTriggerInteraction.Ignore))
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

    private GameObject GenerateObjectClone()
    {

        return Instantiate(this.gameObject, this.transform.position, this.transform.rotation);

    }
}