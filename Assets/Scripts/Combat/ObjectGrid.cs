using System.Collections.Generic;
using UnityEngine;

public class ObjectGrid : MonoBehaviour
{
    [SerializeField] GameObject[] objectsToSpawn;
    [SerializeField] Vector2 gridSize;
    [SerializeField] float spacing = 2f;
    [SerializeField] bool randomizeRotation = true;
    [SerializeField] [Range(1, 100)] int chanceToSpawn;

    [SerializeField] Vector2 cursor = Vector2.zero;

    // List of safe spots where objects should not spawn, such as on enemy spawn or on player spawn
    [SerializeField] List<Vector2> safeSpots = new List<Vector2>();

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
                if (Random.Range(1, 100) > chanceToSpawn) continue;

                if (safeSpots.Contains(new Vector2(x*spacing, y*spacing))) continue;

                Vector3 spawnPosition = new Vector3(x * spacing + transform.position.x, transform.position.y, y * spacing + transform.position.z);

                GameObject objectToSpawn = objectsToSpawn[Random.Range(0, objectsToSpawn.Length)];

                GameObject spawnedObject = Instantiate(objectToSpawn, spawnPosition, Quaternion.identity, transform);

                // Randomize rotation if enabled
                if (randomizeRotation)
                {
                    spawnedObject.transform.Rotate(Vector3.up, Random.Range(0f, 360f));
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
}
