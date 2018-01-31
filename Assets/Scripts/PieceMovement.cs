using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceMovement : MonoBehaviour {

    Rigidbody gameObJectRigidBody;
    public float speed;

    // Use this for initialization
    void Start () {
		
        gameObJectRigidBody =  this.gameObject.GetComponent<Rigidbody>();
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {

        float horiZontalMove = Input.GetAxis("Horizontal");
        float verticalMove = Input.GetAxis("Vertical");

        Vector3 newGameObjectVelocity = new Vector3();

        if(Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
        {
            newGameObjectVelocity = new Vector3(horiZontalMove * speed, gameObJectRigidBody.velocity.y, gameObJectRigidBody.velocity.z);
        }
        else if(Input.GetKey(KeyCode.DownArrow))
        {
            newGameObjectVelocity = new Vector3(gameObJectRigidBody.velocity.x, gameObJectRigidBody.velocity.y, verticalMove * speed);
        }
        else
        {
            newGameObjectVelocity = new Vector3(gameObJectRigidBody.velocity.x, gameObJectRigidBody.velocity.y, -0.5f * speed);
        }

        gameObJectRigidBody.velocity = newGameObjectVelocity;

    }
}