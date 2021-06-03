using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = System.Random;

public class Spawner : MonoBehaviour {
    public PlayerMovement movement;
    
    public int spawnDistance = 300;
    public int segmentSeparator = 1;
    public int minSegmentLength = 100;
    public int maxSegmentLength = 500;
    public int segmentWidth = 15;

    public int spawnPos = 100;

    public GameObject box;
    public GameObject obstacle;
    public GameObject wall;
    
    public GameObject boxWall;
    public GameObject choose;
    public GameObject cubeBox;
    public GameObject funnel;
    public GameObject leftWall;
    public GameObject obelisk;
    public GameObject rightWall;
    public GameObject staggered;
    public GameObject staggeredBoxes;
    public GameObject staggeredBoxesReverse;
    public GameObject staggeredReverse;
    public GameObject staggeredWide;
    public GameObject staggeredWideReverse;
    public GameObject wideBox;

    private Dictionary<Action, Action[]> markovChain;
    private Action nextSegment;

    private readonly Random rand = new Random();

    void Start() {
        void BoxWall() => CreatePrefabSegment(boxWall, 10);
        void Choose() => CreatePrefabSegment(choose, 60);
        void CubeBox() => CreatePrefabSegment(cubeBox, 10);
        void Funnel() => CreatePrefabSegment(funnel, 60);
        void LeftWall() => CreatePrefabSegment(leftWall, 30);
        void Obelisk() => CreatePrefabSegment(obelisk, 50);
        void RightWall() => CreatePrefabSegment(rightWall, 30);
        void Staggered() => CreatePrefabSegment(staggered, 40);
        void StaggeredBoxes() => CreatePrefabSegment(staggeredBoxes, 10);
        void StaggeredBoxesReverse() => CreatePrefabSegment(staggeredBoxesReverse, 10);
        void StaggeredReverse() => CreatePrefabSegment(staggeredReverse, 40);
        void StaggeredWide() => CreatePrefabSegment(staggeredWide, 40);
        void StaggeredWideReverse() => CreatePrefabSegment(staggeredWideReverse, 40);
        void WideBox() => CreatePrefabSegment(wideBox, 10);
        void Start() { }
        void BoxSegment() { }
        markovChain = new Dictionary<Action, Action[]> {
            {
                Start,
                new Action[] {
                    Forest, Gauntlet, Cubes, Staggered, StaggeredReverse, StaggeredWide, StaggeredWideReverse, Funnel,
                    Choose, LeftWall, RightWall, BoxSegment
                }
            },
            {Forest, new Action[] {Start}},
            {Gauntlet, new Action[] {Start}},
            {Cubes, new Action[] {Start}},
            {Staggered, new Action[] {Start, StaggeredReverse}},
            {StaggeredReverse, new Action[] {Start, Staggered}},
            {StaggeredWide, new Action[] {Start, StaggeredWideReverse}},
            {StaggeredWideReverse, new Action[] {Start, StaggeredWide}},
            {Funnel, new Action[] {Staggered, StaggeredReverse, StaggeredWide, StaggeredWideReverse}},
            {Choose, new Action[] {Staggered, StaggeredReverse, StaggeredWide, StaggeredWideReverse}},
            {RightWall, new Action[] {StaggeredReverse, StaggeredWideReverse}},
            {LeftWall, new Action[] {Staggered, StaggeredWide}},
            {BoxSegment, new Action[] {BoxWall, StaggeredBoxes, StaggeredBoxesReverse, CubeBox, WideBox}},
            {BoxWall, new Action[] {Obelisk}},
            {StaggeredBoxes, new Action[] {Obelisk}},
            {StaggeredBoxesReverse, new Action[] {Obelisk}},
            {CubeBox, new Action[] {Obelisk}},
            {WideBox, new Action[] {Obelisk}},
            {Obelisk, new Action[] {Start}}
        };
        nextSegment = Start;
    }

    void CreatePrefabSegment(Object segmentPrefab, int segmentLength) {
        GameObject segmentInstance = (GameObject) Instantiate(segmentPrefab, transform, false);
        Vector3 pos = segmentInstance.transform.position;
        pos.z = spawnPos;
        spawnPos += segmentSeparator + segmentLength;
        segmentInstance.transform.position = pos;
    }

    void Update () {
        if (spawnPos - movement.transform.position.z < spawnDistance) {
            Action segmentAction = nextSegment;
            segmentAction.Invoke();
            Action[] possibleSegments = markovChain[nextSegment];
            nextSegment = possibleSegments[rand.Next(possibleSegments.Length)];
        }
    }

    void Forest() {
        int length = rand.Next(minSegmentLength, maxSegmentLength);
        // Generate obstacle towers (or 'trees')
        for (int i = 0; i < length; i += 10) {
            double prob = rand.NextDouble();
            // generate tower from cubes
            if (prob <= 0.1) {
                float height = rand.Next(8, 16);
                float x = rand.Next(0, segmentWidth) - (segmentWidth - 1) / 2; 
                for (int y = 1; y <= height; y += 1) {
                    GameObject obstacleInstance = Instantiate(obstacle, transform, false);
                    obstacleInstance.transform.position = new Vector3(x, y, i + spawnPos);
                    obstacleInstance.GetComponent<Rigidbody>().mass = 0.1f;
                }
            }
            // generate tower
            else {
                GameObject obstacleInstance = Instantiate(obstacle, transform, false);
                float height = rand.Next(8, 128);
                float x = rand.Next(0, segmentWidth) - (segmentWidth - 1) / 2; 
                obstacleInstance.transform.position = new Vector3(x, height / 2 + 0.5f, i + spawnPos);
                obstacleInstance.transform.localScale = new Vector3(1, height, 1);
                obstacleInstance.GetComponent<Rigidbody>().mass = height;
            }
        }
        // Generate boxes
        for (int i = 5; i < length; i += 10) {
            double prob = rand.NextDouble();
            if (prob <= 0.1) {
                GameObject obstacleInstance = Instantiate(box, transform, false);
                float xPos = rand.Next(0, segmentWidth - 1) - (segmentWidth - 1) / 2;
                obstacleInstance.transform.position = new Vector3(xPos, 1, i + spawnPos);
            }
        }
        spawnPos += segmentSeparator + length;
    }
    
    void Gauntlet() {
        int length = rand.Next(minSegmentLength, maxSegmentLength);
        // Generate horizontal obstacle barriers
        for (int i = 0; i < length; i += 15) {
            double prob = rand.NextDouble();
            // Generate wide barrier made from cubes
            if (prob <= 0.1) {
                int width = rand.Next(0, segmentWidth - 3);
                float xStart = rand.Next(0, segmentWidth - width) - (segmentWidth - 1) / 2;
                for (float x = xStart; x <= width + xStart; x += 1) {
                    GameObject obstacleInstance = Instantiate(obstacle, transform, false);
                    obstacleInstance.transform.position = new Vector3(x, 1, i + spawnPos);
                }
            }
            // Generate wide barrier made from cubes
            else if (prob <= 0.2) {
                GameObject obstacleInstance = Instantiate(obstacle, transform, false);
                int width = rand.Next(5, segmentWidth - 3);
                int x = rand.Next(0, segmentWidth - width) - (segmentWidth - width) / 2;
                obstacleInstance.transform.position = new Vector3(x, 1, i + spawnPos);
                obstacleInstance.transform.localScale = new Vector3(width, 1, 1);
                obstacleInstance.GetComponent<Rigidbody>().mass = width;
            }
            // Generate barrier made from cubes
            else {
                GameObject obstacleInstance = Instantiate(obstacle, transform, false);
                int width = rand.Next(1, 5);
                int x = rand.Next(0, segmentWidth - width) - (segmentWidth - width) / 2;
                obstacleInstance.transform.position = new Vector3(x, 1, i + spawnPos);
                obstacleInstance.transform.localScale = new Vector3(width, 1, 1);
                obstacleInstance.GetComponent<Rigidbody>().mass = width;
            }
        }
        // Generate horizontal boxes
        for (int i = 5; i < length; i += 15) {
            double prob = rand.NextDouble();
            if (prob <= 0.1) {
                GameObject obstacleInstance = Instantiate(box, transform, false);
                int width = rand.Next(1, 5);
                int x = rand.Next(0, segmentWidth - width) - (segmentWidth - width) / 2;
                obstacleInstance.transform.position = new Vector3(x, 1, i + spawnPos);
                obstacleInstance.transform.localScale = new Vector3(width, 1, 1);
            }
        }
        spawnPos += segmentSeparator + length;
    }
    
    void Cubes() {
        int length = rand.Next(minSegmentLength, maxSegmentLength);
        // Generate obstacle cubes
        for (int i = 0; i < length; i += 10) {
            double prob = rand.NextDouble();
            // Generate cube made of small cubes
            if (prob <= 0.1) {
                int size = rand.Next(1, 5);
                float xStart = rand.Next(0, segmentWidth - 1 - size) - (segmentWidth - 1) / 2;
                for (float x = xStart; x <= xStart + size; x += 1) {
                    for (float y = 1; y <= size + 1; y += 1) {
                        for (float z = i + spawnPos; z <= i + spawnPos + size; z += 1) {
                            GameObject obstacleInstance = Instantiate(obstacle, transform, false);
                            obstacleInstance.transform.position = new Vector3(x, y, z);
                            obstacleInstance.GetComponent<Rigidbody>().mass = 1 / (float) (size * size);
                        }
                    }
                }
                i += size;
            }
            // Generate large cube
            else if (prob <= 0.2) {
                GameObject obstacleInstance = Instantiate(obstacle, transform, false);
                int size = rand.Next(5, segmentWidth - 3);
                int x = rand.Next(0, segmentWidth - size) - (segmentWidth - size) / 2;
                int y = size / 2 + 1;
                obstacleInstance.transform.position = new Vector3(x, y, i + spawnPos);
                obstacleInstance.transform.localScale = new Vector3(size, size, size);
                obstacleInstance.GetComponent<Rigidbody>().mass = size;
                i += size;
            }
            // Generate cube
            else {
                GameObject obstacleInstance = Instantiate(obstacle, transform, false);
                int size = rand.Next(1, 5);
                int x = rand.Next(0, segmentWidth - size) - (segmentWidth - size) / 2;
                int y = size / 2 + 1;
                obstacleInstance.transform.position = new Vector3(x, y, i + spawnPos);
                obstacleInstance.transform.localScale = new Vector3(size, size, size);
                obstacleInstance.GetComponent<Rigidbody>().mass = size;
                i += size;
            }
        }
        // Generate box cubes
        for (int i = 5; i < length; i += 10) {
            if (rand.Next(0, 10) == 0) {
                GameObject obstacleInstance = Instantiate(box, transform, false);
                int size = rand.Next(1, 5);
                int x = rand.Next(0, segmentWidth - size) - (segmentWidth - size) / 2;
                int y = size / 2 + 1;
                obstacleInstance.transform.position = new Vector3(x, y, i + spawnPos);
                obstacleInstance.transform.localScale = new Vector3(size, size, size);
                obstacleInstance.GetComponent<Rigidbody>().mass = size;
                i += size;
            }
        }
        spawnPos += segmentSeparator + length;
    }
    
}