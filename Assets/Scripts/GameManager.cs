using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Toggle isRandomToggle;
    [SerializeField] private InputField rngSeedInputField;

    private bool isRandomValue;

    public bool IsScanning { get; private set; } = false;

    private bool isFindingMatches = false;
    private bool isMatchFound = false;


    private TileManager SelectedTileManager { 
        get { return TileInputManager.SelectedTile.GetComponent<TileManager>(); } 
    }
    private TileManager TargetTileManager
    {
        get { return TileInputManager.TargetTile.GetComponent<TileManager>(); }
    }
    private int W { get { return SpawnManager.boardWidth; } }
    private int H { get { return SpawnManager.boardHeight; } }
    public static GameManager Instance { get; private set; } // ENCAPSULATION
    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        SpawnManager.Instance.InitializeBoard(isRandomValue);
        isFindingMatches = false;
        isMatchFound = false;
        IsScanning = false;
    }
    
    void Update()
    {
        if (TileInputManager.SelectedTile == null || TileInputManager.TargetTile == null) 
        {
            IsScanning = false;
            return;
        }
        if (IsScanning && !SelectedTileManager.IsBusy && !TargetTileManager.IsBusy) 
        {
            if (isFindingMatches)
            {
                FindMatches();
                return;
            }
                
            if(!isMatchFound) 
            {
                SelectedTileManager.OnSwap(TargetTileManager.transform.position);
                TargetTileManager.OnSwap(SelectedTileManager.transform.position);
            }

            IsScanning = false;
        }
    }

    public void Swap()
    {
        SelectedTileManager.OnSwap(TargetTileManager.transform.position);
        TargetTileManager.OnSwap(SelectedTileManager.transform.position);

        IsScanning = true;
        isFindingMatches = true;
    }

    private void FindMatches()
    {
        var flaggedTiles = new HashSet<TileManager>();

        for (int x = 2; x < W; x++)
        {
            for (int y = 2; y < H; y++)
            {
                var tile1 = NameAsPositionSetter.GetObject(x, y).GetComponent<TileManager>();

                // Horizontal matches
                var tile2 = NameAsPositionSetter.GetObject(x - 1, y).GetComponent<TileManager>();
                var tile3 = NameAsPositionSetter.GetObject(x - 2, y).GetComponent<TileManager>();
                if (tile1.CompareTag(tile2.tag) && tile1.CompareTag(tile3.tag))
                    flaggedTiles.UnionWith(new[] { tile1, tile2, tile3 });

                // Vertical matches
                tile2 = NameAsPositionSetter.GetObject(x, y - 1).GetComponent<TileManager>();
                tile3 = NameAsPositionSetter.GetObject(x, y - 2).GetComponent<TileManager>();
                if (tile1.CompareTag(tile2.tag) && tile1.CompareTag(tile3.tag))
                    flaggedTiles.UnionWith(new[] { tile1, tile2, tile3 });
            }
        }

        //iterate over flagged tiles
        if (flaggedTiles.Count > 0)
        {
            isMatchFound = true;
            foreach (var tile in flaggedTiles)
            {
                tile.OnMatch();
            }
        }

        isFindingMatches = false;
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
