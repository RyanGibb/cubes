using UnityEngine;

public class PlayerMovement : MonoBehaviour {
	public Rigidbody body;
	public float xVelocity = 10;
	public float zForce = 10000;
	public float zVelocity = 50;
	public Spawner spawner;

	public float zReRootPos = 1000;
	public float zDeletePos = -1000;

	public float reRootDistance;

	public float swipeDistance = 0.25f;

	private float horizontal;
	private Vector3 firstTouchPos;

	void Update() {
		horizontal = Input.GetAxis("Horizontal");
		if (Input.touchCount == 1) {
			Touch touch = Input.GetTouch(0);
			horizontal = (touch.position.x * 2 - Screen.width) / 2 / (swipeDistance * Screen.width);
			if (horizontal < -1) {
				horizontal = -1;
			} else if (horizontal > 1) {
				horizontal = 1;
			}
		}
	}

	void FixedUpdate() {
		if (body.velocity.z < zVelocity) {
			body.AddForce(zForce * Time.fixedDeltaTime * Vector3.forward);
		}
		Vector3 velocity = body.velocity;
		velocity.x = horizontal * xVelocity;
		body.velocity = velocity;
		
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