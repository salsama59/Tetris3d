using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceMovement : MonoBehaviour {

    Rigidbody gameObJectRigidBody;
    Transform gameObjectTransform;
    public GameObject field;
    private Vector3 minRange;
    private Vector3 maxRange;

    public float speed;

    // Use this for initialization
    void Start () {

        this.gameObjectTransform = this.gameObject.GetComponent<Transform>();
        this.gameObJectRigidBody = this.gameObject.GetComponent<Rigidbody>();

        this.CalculateFieldRanges();

    }
	
	// Update is called once per frame
	void FixedUpdate () {

        float verticalMove = Input.GetAxis("Vertical");

        Vector3 newGameObjectVelocity = new Vector3();

        Vector3 newPosition = new Vector3();

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            newPosition = this.gameObjectTransform.position + Vector3.right;
            this.gameObjectTransform.position = this.ClampObjectPosition(newPosition);
        }
        else if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            newPosition = this.gameObjectTransform.position + Vector3.left;
            this.gameObjectTransform.position = this.ClampObjectPosition(newPosition);
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

    void CalculateFieldRanges()
    {
        Renderer fieldRenderer = this.field.GetComponent<Renderer>();
        this.maxRange = fieldRenderer.bounds.size * 0.5f;
        this.minRange = new Vector3(this.maxRange.x * -1, this.maxRange.y * -1, this.maxRange.z * -1);
        Debug.Log("Max range = " + this.maxRange);
        Debug.Log("Min range = " + this.minRange);
    }

    private Vector3 ClampObjectPosition(Vector3 position)
    {

        return new Vector3(
            Mathf.Clamp(position.x, minRange.x, maxRange.x),
            Mathf.Clamp(position.y, minRange.y, maxRange.y),
            Mathf.Clamp(position.z, minRange.z, maxRange.z)
            );
    }
}