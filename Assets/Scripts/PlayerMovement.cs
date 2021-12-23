using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
	public Rigidbody body;
	public float maxXVelocity = 20;
	public float maxXVelocityController = 10;
	public float maxXDiff = 15;
	public float zForce = 10000;
	public float zVelocity = 50;
	public Spawner spawner;

	public int zReRootPos = 1000;
	public int zDeletePos = -1000;

	public int reRootDistance;

	// public float swipeDistance = 0.25f;

	public Transform camera;
	public float headsetMovementMultiplication;
	public Vector3 cameraOffset;
	
	private float targetXOffset;
	private float horizontal;

	void Update() {
		horizontal = Input.GetAxis("Horizontal");
	}

	void FixedUpdate() {
		if (body.velocity.z < zVelocity) {
			body.AddForce(zForce * Time.fixedDeltaTime * Vector3.forward);
		}
		
		float targetX = camera.localPosition.x * headsetMovementMultiplication + targetXOffset;
		float xDiff = targetX - transform.position.x;
		// exponential velocity curve
		float xVelocity = (float) (1 - Math.Pow(xDiff / maxXDiff + 1, -2)) * maxXVelocity;
		Vector3 velocity = body.velocity;
		velocity.x = xVelocity + horizontal * maxXVelocityController;
		Debug.Log(velocity.x);
		body.velocity = velocity;

		// If controller other than headset being used add offset to compensate
		if (horizontal != 0) {
			targetXOffset = transform.position.x;
		}

		Vector3 cameraPosition = transform.position + cameraOffset;
		cameraPosition.x = targetX;
		camera.parent.position = cameraPosition;
		
		// Reroot to avoid floating point errors when the player reaches a Z position of zReRootPos
		if (body.position.z > zReRootPos) {
			Vector3 playerPosition = body.position;
			playerPosition.z -= zReRootPos;
			body.position = playerPosition;
			reRootDistance += zReRootPos;
			spawner.spawnPos -= zReRootPos;
			foreach (Transform child in spawner.transform) {
				Vector3 childPosition = child.position;
				childPosition.z -= zReRootPos;
				// If spawned objects are behind the player, delete them 
				if (childPosition.z < zDeletePos) {
					Destroy(child.gameObject);
				}
				child.position = childPosition;
			}
		}
	}
}