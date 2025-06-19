using System.Linq;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    // ENCAPSULATION
    [SerializeField] private GameObject[] tilePrefabs;

    // ENCAPSULATION
    public static int boardWidth { get; private set; } = 5;
    public static int boardHeight { get; private set; } = 8;

    private int[,]  tileTypes = new int[boardWidth, boardHeight];

    public static SpawnManager Instance { get; private set; } // ENCAPSULATION
    private void Awake()
    {
        Instance = this;
    }

    // Board initializer
    public void InitializeBoard()
    {
        if (GeneralSettingsManager.Instance.isRandomOn)
            GeneralSettingsManager.Instance.rngSeed = (int)Time.time;

        Random.InitState(GeneralSettingsManager.Instance.rngSeed);

        for (int x = 0; x < boardWidth; x++)
        {
            for (int y = 0; y < boardHeight; y++)
            {
                tileTypes[x, y] = GetNonMatchingTileType(x, y); // ABSTRACTION
                int tileType = tileTypes[x, y];
                Instantiate(
                    tilePrefabs[tileType],
                    new Vector3(x, y, 0),
                    Quaternion.identity
                );
            }
        }
    }

    public void SpawnTilesAtCol(int col, int countOfTilesToSpawn)
    {
        for (int i = 0; i < countOfTilesToSpawn; i++)
        {
            var obj = Instantiate(tilePrefabs[GetRandomTileIndex()], new Vector3(col, boardHeight + i, 0), Quaternion.identity);
            obj.GetComponent<TileManager>().OnFall(countOfTilesToSpawn);
        }
    }

    private int GetNonMatchingTileType(int x, int y)
    {
        var validTypes = Enumerable.Range(0, tilePrefabs.Length)
            .Where(type => !CreatesMatch(x, y, type)) // ABSTRACTION
            .ToList();

        return validTypes.Count > 0
            ? validTypes[Random.Range(0, validTypes.Count)]
            : Random.Range(0, tilePrefabs.Length);
    }

    private bool CreatesMatch(int x, int y, int newType)
    {
        bool ans = false;
        if (x >= 2) ans = tileTypes[x - 1, y] == tileTypes[x - 2, y] && tileTypes[x - 1, y] == newType;
        if (!ans && y >= 2) ans = tileTypes[x, y - 1] == tileTypes[x, y - 2] && tileTypes[x, y - 1] == newType;
        return ans;
    }

    private int GetRandomTileIndex()
    {
        return Random.Range(0, tilePrefabs.Length);
    }
}
