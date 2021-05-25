using UnityEngine;

public class PlayerMovement : MonoBehaviour {
	public Rigidbody body;
	public float xVelocity = 10;
	public float zForce = 10000;
	public float zVelocity = 50;
	public Spawner spawner;

	public float zReRootPos = 1000;
	public float zDeletePos = -10;

	public float reRootDistance;
	
	private float horizontal;

	void Update() {
		horizontal = Input.GetAxisRaw("Horizontal");
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
				if (childPosition.z + child.lossyScale.z / 2 < zDeletePos) {
					Destroy(child.gameObject);
				}
				child.position = childPosition;
			}
		}
	}
}