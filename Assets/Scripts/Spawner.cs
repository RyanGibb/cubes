using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour {
    
    public PlayerMovement movement;
    public float segmentSeparator = 100;
    public float spawnDistance = 300;

    public float spawnPos;

    public Object[] segments;

    void Update () {
        if (spawnPos - movement.transform.position.z < spawnDistance) {
            int segmentIndex = Random.Range(0, segments.Length);
            Object segment = segments[segmentIndex];
            GameObject instance = (GameObject) Instantiate(segment, transform, false);
            Vector3 pos = instance.transform.position;
            float newSpawnPos = spawnPos + segmentSeparator;
            pos.z += newSpawnPos;
            spawnPos = newSpawnPos;
            instance.transform.position = pos;
        }
    }
    
}