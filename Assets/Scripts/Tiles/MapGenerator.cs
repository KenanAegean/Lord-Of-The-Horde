using UnityEngine;
using System.Collections.Generic;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] public TileType[] tileTypes;         
    [SerializeField] public int chunkSize = 10;            
    [SerializeField] public float tileSize = 1f;           
    [SerializeField] public Transform mapParent;           

    [SerializeField] public float unloadDistance = 30f;    

    private Dictionary<Vector2, GameObject> spawnedChunks = new Dictionary<Vector2, GameObject>();
    [SerializeField] public Transform player;              

    void Update()
    {
        if (player == null)
        {
            Debug.LogWarning("Player reference is missing in MapGenerator.");
            return;
        }

        Vector2 currentChunk = new Vector2(Mathf.Floor(player.position.x / chunkSize), Mathf.Floor(player.position.y / chunkSize));
        GenerateChunksAroundPlayer(currentChunk);
        UnloadDistantChunks(currentChunk); 
    }

    public void GenerateMap()
    {
        if (mapParent == null)
        {
            Debug.LogError("Map parent is not assigned in the inspector.");
            return;
        }

        Vector2 currentChunk = new Vector2(Mathf.Floor(player.position.x / chunkSize), Mathf.Floor(player.position.y / chunkSize));
        GenerateChunksAroundPlayer(currentChunk);
    }

    // Generates chunks around the player's current chunk
    void GenerateChunksAroundPlayer(Vector2 currentChunk)
    {
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                Vector2 chunkPos = new Vector2(currentChunk.x + x, currentChunk.y + y);
                if (!spawnedChunks.ContainsKey(chunkPos))
                {
                    GameObject newChunk = GenerateChunk(chunkPos);
                    spawnedChunks.Add(chunkPos, newChunk);
                }
            }
        }
    }

    // Unload chunks that are too far from the player
    void UnloadDistantChunks(Vector2 currentChunk)
    {
        List<Vector2> chunksToRemove = new List<Vector2>();

        foreach (var chunk in spawnedChunks)
        {
            Vector2 chunkPos = chunk.Key;
            float distance = Vector2.Distance(currentChunk, chunkPos);

            if (distance > unloadDistance)
            {
                // Mark chunk for removal
                chunksToRemove.Add(chunkPos);
            }
        }

        foreach (Vector2 chunkPos in chunksToRemove)
        {
            GameObject chunkToDestroy = spawnedChunks[chunkPos];
            Destroy(chunkToDestroy);
            spawnedChunks.Remove(chunkPos);
        }
    }

    GameObject GenerateChunk(Vector2 chunkPos)
    {
        GameObject chunk = new GameObject("Chunk " + chunkPos);
        chunk.transform.SetParent(mapParent, false);  // Set chunk parent as Map
        chunk.transform.position = new Vector3(chunkPos.x * chunkSize * tileSize, chunkPos.y * chunkSize * tileSize, 0);

        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                Vector2 tilePos = new Vector2(chunkPos.x * chunkSize + x, chunkPos.y * chunkSize + y);
                SpawnTile(tilePos, chunk.transform);  // Spawn tiles as children of the chunk
            }
        }
        return chunk;
    }

    void SpawnTile(Vector2 position, Transform chunkParent)
    {
        TileType selectedTile = SelectTileType();  
        if (selectedTile != null)
        {
            Vector3 tilePosition = new Vector3(position.x * tileSize, position.y * tileSize, 10f);
            GameObject tile = Instantiate(selectedTile.tilePrefab, tilePosition, Quaternion.identity, chunkParent);
        }
    }

 
    TileType SelectTileType()
    {
        float totalChance = 0f;
        foreach (TileType tile in tileTypes)
        {
            totalChance += tile.spawnChance;
        }

        float randomValue = Random.Range(0f, totalChance);
        float cumulativeChance = 0f;

        foreach (TileType tile in tileTypes)
        {
            cumulativeChance += tile.spawnChance;
            if (randomValue < cumulativeChance)
            {
                return tile;
            }
        }

        return null;
    }
}
