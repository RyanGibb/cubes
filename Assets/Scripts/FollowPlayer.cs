using UnityEngine;

public class FollowPlayer : MonoBehaviour {

    public Transform player;
    public float yOffset;
    public float zOffset;
    public float headsetMovementMultiplication;
    
    Vector3 lastPos = Vector3.zero;
    
    void Update() {
        if(lastPos == Vector3.zero) lastPos = transform.position;
        var offset = transform.position - lastPos;
        offset.y = 0;
        transform.parent.position += offset * headsetMovementMultiplication;
        lastPos = transform.position;
        
        Vector3 cameraOffsetPosition = transform.parent.position;
        cameraOffsetPosition.y = player.position.y + yOffset;
        cameraOffsetPosition.z = player.position.z + zOffset;
        transform.parent.position = cameraOffsetPosition;
    }
}
