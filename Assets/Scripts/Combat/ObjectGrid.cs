using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGrid : MonoBehaviour
{
    public static ObjectGrid Instance { get; private set; }

    [SerializeField] GameObject[] objectsToSpawn;
    [SerializeField] Vector2 gridSize;
    [SerializeField] float spacing;
    [SerializeField] bool randomizeRotation = true;
    [SerializeField] [Range(1, 100)] int chanceToSpawn;

    [SerializeField] Vector2 cursor = Vector2.zero;

    // List of safe spots where objects should not spawn, such as on enemy spawn or on player spawn
    [SerializeField] List<Vector2> safeSpots = new List<Vector2>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        SpawnObjects();
    }

    void SpawnObjects()
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                // Roll a chance for the spawn
                if (UnityEngine.Random.Range(1, 100) > chanceToSpawn) continue;

                if (safeSpots.Contains(new Vector2(x*spacing, y*spacing))) continue;

                Vector3 spawnPosition = new Vector3(x * spacing + transform.position.x, transform.position.y, y * spacing + transform.position.z);

                GameObject objectToSpawn = objectsToSpawn[UnityEngine.Random.Range(0, objectsToSpawn.Length)];

                GameObject spawnedObject = Instantiate(objectToSpawn, spawnPosition, Quaternion.identity, transform);

                spawnedObject.layer = LayerMask.NameToLayer("Movement Blocking Object");
                foreach(Transform transform in spawnedObject.transform) 
                {
                    transform.gameObject.layer = LayerMask.NameToLayer("Movement Blocking Object");
                }

                // Randomize rotation if enabled
                if (randomizeRotation)
                {
                    spawnedObject.transform.Rotate(Vector3.up, UnityEngine.Random.Range(0f, 360f));
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 worldPosition = new Vector3(cursor.x, 0f, cursor.y);
        Gizmos.DrawWireSphere(transform.position + worldPosition, 2);
    }

    public Tuple<Vector2, Vector2> GetGridBoundaries()
    {
        Tuple<Vector2, Vector2> result = new Tuple<Vector2, Vector2>(
            new Vector2(transform.position.x, transform.position.z),
            new Vector2(transform.position.x + ((gridSize.x-1) * spacing), transform.position.z + ((gridSize.y - 1) * spacing)));
        return result;
    }

    public float GetObjectSpacing()
    {
        return spacing;
    }

}
