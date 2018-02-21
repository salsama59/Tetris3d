using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceMovement : MonoBehaviour {

    Rigidbody gameObJectRigidBody;
    Transform gameObjectTransform;
    public GameObject field;
    private Vector3 minRange;
    private Vector3 maxRange;
    private bool isMoving;
    public float speed;
    private float elapsedTime;
    public float targetElapsedtime;
    public float timeToMoveToward;

    // Use this for initialization
    void Start ()
    {
        IsMoving = true;
        this.gameObjectTransform = this.gameObject.GetComponent<Transform>();
        this.gameObJectRigidBody = this.gameObject.GetComponent<Rigidbody>();

        this.CalculateFieldRanges();
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
                newPosition = this.gameObjectTransform.position + Vector3.right;
                this.MoveObjectToNewPosition(newPosition);
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                newPosition = this.gameObjectTransform.position + Vector3.left;
                this.MoveObjectToNewPosition(newPosition);
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                RotateObject(true);
            }
            else if(Input.GetKeyDown(KeyCode.LeftAlt))
            {
                this.RotateObject(false);
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
            this.gameObjectTransform.position = this.ClampObjectPosition(Vector3.Lerp(this.gameObjectTransform.position, newPosition, timeToMoveToward));
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

    void CalculateFieldRanges()
    {
        MeshFilter fieldRenderer = this.field.GetComponent<MeshFilter>();
        Vector3 objectHalfSize = fieldRenderer.mesh.bounds.size * 0.5f;
        Vector3 objectScale = this.field.transform.localScale;

        this.maxRange = new Vector3(
            objectHalfSize.x * objectScale.x + this.field.transform.position.x - this.GetObjectHalfsize(this.gameObject).x,
            objectHalfSize.y * objectScale.y + this.field.transform.position.y - this.GetObjectHalfsize(this.gameObject).y,
            objectHalfSize.z * objectScale.z + this.field.transform.position.z - this.GetObjectHalfsize(this.gameObject).z
            );

        this.minRange = new Vector3(
            this.field.transform.position.x - objectHalfSize.x * objectScale.x + this.GetObjectHalfsize(this.gameObject).x,
            this.field.transform.position.y - objectHalfSize.y * objectScale.y + this.GetObjectHalfsize(this.gameObject).y,
            this.field.transform.position.z - objectHalfSize.z * objectScale.z + this.GetObjectHalfsize(this.gameObject).z);
    }

    private Vector3 ClampObjectPosition(Vector3 position)
    {

        return new Vector3(
            Mathf.Clamp(position.x, minRange.x, maxRange.x),
            Mathf.Clamp(position.y, position.y, position.y),
            Mathf.Clamp(position.z, position.z, position.z)
            );
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
}