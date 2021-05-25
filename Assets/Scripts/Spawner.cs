using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour {
    
    public PlayerMovement movement;
    public float spawnDistance = 300;
    public float segmentSeparator = 1;
    public float minSegmentLength = 100;
    public float maxSegmentLength = 500;
    public float segmentWidth = 15;

    public float spawnPos = 100;

    public GameObject box;
    public GameObject obstacle;
    public GameObject wall;
    
    public GameObject[] segmentPrefabs;
    public float[] segmentLengths;

    private List<Action> segments;
    private int lastSegmentIndex = -1;

    void Start() {
        segments = new List<Action>();
        for (int i = 0; i < segmentPrefabs.Length; i++) {
            GameObject segmentPrefab = segmentPrefabs[i];
            float segmentLength = segmentLengths[i];
            segments.Add(() => CreatePrefabSegment(segmentPrefab, segmentLength));
        }
        segments.Add(Forest);
        segments.Add(Gauntlet);
    }

    void CreatePrefabSegment(Object segmentPrefab, float segmentLength) {
        GameObject segmentInstance = (GameObject) Instantiate(segmentPrefab, transform, false);
        Vector3 pos = segmentInstance.transform.position;
        pos.z = spawnPos;
        spawnPos += segmentSeparator + segmentLength;
        segmentInstance.transform.position = pos;
    }

    void Update () {
        if (spawnPos - movement.transform.position.z < spawnDistance) {
            int segmentIndex = lastSegmentIndex;
            while (segmentIndex == lastSegmentIndex) {
                segmentIndex = Random.Range(0, segments.Count);
            }
            Action segmentAction = segments[segmentIndex];
            segmentAction.Invoke();
        }
    }

    void Forest() {
        float length = Random.Range(minSegmentLength, maxSegmentLength);
        for (int i = 0; i < length; i += 10) {
            if (Random.Range(0, 10) == 0) {
                float height = Random.Range(10, 15); 
                float x = Random.Range(0, segmentWidth) - segmentWidth / 2;
                for (int y = 1; y <= height; y += 1) {
                    GameObject obstacleInstance = Instantiate(obstacle, transform, false);
                    obstacleInstance.transform.position = 
                        new Vector3(x, y, i + spawnPos);
                }
            }
            else {
                GameObject obstacleInstance = Instantiate(obstacle, transform, false);
                float height = Random.Range(10, 100);
                obstacleInstance.transform.position = 
                    new Vector3(Random.Range(0, segmentWidth) - segmentWidth / 2, height / 2 + 0.5f, i + spawnPos);
                obstacleInstance.transform.localScale =
                    new Vector3(1, height, 1);
                obstacleInstance.GetComponent<Rigidbody>().mass = height;
            }
        }
        for (int i = 2; i < length; i += 10) {
            if (Random.Range(0, 10) == 0) {
                GameObject obstacleInstance = Instantiate(box, transform, false);
                obstacleInstance.transform.position = 
                    new Vector3(Random.Range(0, segmentWidth) - segmentWidth / 2, 1, i + spawnPos);
            } else if (Random.Range(0, 20) == 0) {
                GameObject obstacleInstance = Instantiate(box, transform, false);
                obstacleInstance.transform.position = 
                    new Vector3(Random.Range(0, segmentWidth - 2) - (segmentWidth - 2) / 2, 1, i + spawnPos);
                obstacleInstance.transform.localScale =
                    new Vector3(3, 1, 1);
                obstacleInstance.GetComponent<Rigidbody>().mass = 3;
            }
        }
        spawnPos += segmentSeparator + length;
    }
    
    void Gauntlet() {
        float length = Random.Range(minSegmentLength, maxSegmentLength);
        for (int i = 0; i < length; i += 20) {
            if (Random.Range(0, 10) == 0) {
                float width = Random.Range(0, segmentWidth - 3);
                for (float x = Random.Range(0, segmentWidth - width); x <= width; x += 1) {
                    GameObject obstacleInstance = Instantiate(obstacle, transform, false);
                    obstacleInstance.transform.position = 
                        new Vector3(x, 1, i + spawnPos);
                }
            } else if (Random.Range(0, 10) == 0) {
                GameObject obstacleInstance = Instantiate(obstacle, transform, false);
                float width = Random.Range(5, segmentWidth - 3);
                obstacleInstance.transform.position = 
                    new Vector3(Random.Range(0, segmentWidth - width) - (segmentWidth - width) / 2, 1, i + spawnPos);
                obstacleInstance.transform.localScale =
                    new Vector3(width, 1, 1);
                obstacleInstance.GetComponent<Rigidbody>().mass = width;
            } else {
                GameObject obstacleInstance = Instantiate(obstacle, transform, false);
                float width = Random.Range(1, 5);
                obstacleInstance.transform.position = 
                    new Vector3(Random.Range(0, segmentWidth - width) - (segmentWidth - width) / 2, 1, i + spawnPos);
                obstacleInstance.transform.localScale =
                    new Vector3(width, 1, 1);
                obstacleInstance.GetComponent<Rigidbody>().mass = width;
            }
        }
        for (int i = 2; i < length; i += 10) {
            if (Random.Range(0, 20) == 0) {
                GameObject obstacleInstance = Instantiate(box, transform, false);
                obstacleInstance.transform.position = 
                    new Vector3(Random.Range(0, segmentWidth) - segmentWidth / 2, 1, i + spawnPos);
            } else if (Random.Range(0, 40) == 0) {
                GameObject obstacleInstance = Instantiate(box, transform, false);
                obstacleInstance.transform.position = 
                    new Vector3(Random.Range(0, segmentWidth - 2) - (segmentWidth - 2) / 2, 1, i + spawnPos);
                obstacleInstance.transform.localScale =
                    new Vector3(3, 1, 1);
            }
        }
        spawnPos += segmentSeparator + length;
    }
    
}