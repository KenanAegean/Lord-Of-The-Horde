using UnityEngine;

[CreateAssetMenu(fileName = "NewTileType", menuName = "Tile/TileType")]
public class TileType : ScriptableObject
{
    public string tileName;             
    public GameObject tilePrefab;       
    public bool isWalkable;             
    public float spawnChance = 1f;      
}
