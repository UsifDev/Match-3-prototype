using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public bool IsScanning { get; private set; } = false;

    private bool isFindingMatches = false;
    private bool isMatchFound = false;
    private bool isRegenerating = false;

    [SerializeField] private AudioClip matchFailClip;
    [SerializeField] private AudioClip matchClip;
    [SerializeField] private AudioClip swapClip;
    private AudioSource gameSoundsPlayer;

    [SerializeField] private ParticleSystem matchedParticles;

    private bool IsSoundOn
    {
        get { return GeneralSettingsManager.Instance.isSoundOn; }
    }

    private TileManager SelectedTileManager { 
        get { return TileInputManager.SelectedTile?.GetComponent<TileManager>(); } 
    }
    private TileManager TargetTileManager
    {
        get { return TileInputManager.TargetTile?.GetComponent<TileManager>(); }
    }
    private int W { get { return SpawnManager.boardWidth; } }
    private int H { get { return SpawnManager.boardHeight; } }

    public static int GameScaling { get; private set; }  = 200;

    public static GameManager Instance { get; private set; } // ENCAPSULATION
    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        SpawnManager.Instance.InitializeBoard();
        gameSoundsPlayer = GetComponent<AudioSource>();
        isFindingMatches = false;
        isMatchFound = false;
        IsScanning = false;
        isRegenerating = false;
    }
    
    void Update()
    {
        // check if we got a match out of our specials swapping
        if (isMatchFound)
        {
            // if so, start regenerating and if this is a frame where we are done generating, re-evaluate the board

            if (isRegenerating)
            {
                regenerateBoard();
            }
            else
            {
                // check all tiles to make sure none are busy before re-evaluating
                for (int x = 0; x < W; x++)
                    for (int y = 0; y < H; y++)
                        if(NameAsPositionSetter.GetObject(x, y) == null || NameAsPositionSetter.GetObject(x, y).GetComponent<TileManager>().IsBusy)
                            return;

                FindMatches();
            }
            return;
        }
        
        // specials have died so stop scanning and allow user input
        if (TileInputManager.SelectedTile == null || TileInputManager.TargetTile == null)
        {
            IsScanning = false;
            return;
        }

        // specials are animating
        if (SelectedTileManager.IsBusy && TargetTileManager.IsBusy) return;
        
        // specials have attempted to swap
        if (!IsScanning) return;
        if(isFindingMatches) 
            FindMatches();
        else
        {
            if (IsSoundOn) gameSoundsPlayer.PlayOneShot(swapClip, 0.2f);
            if (IsSoundOn) gameSoundsPlayer.PlayOneShot(matchFailClip, 0.2f);

            SelectedTileManager.OnSwap(TargetTileManager.transform.position);
            TargetTileManager.OnSwap(SelectedTileManager.transform.position);

            IsScanning = false;
        }
    }

    public void Swap()
    {
        if (IsSoundOn) gameSoundsPlayer.PlayOneShot(swapClip, 0.2f);

        SelectedTileManager.OnSwap(TargetTileManager.transform.position);
        TargetTileManager.OnSwap(SelectedTileManager.transform.position);

        IsScanning = true;
        isFindingMatches = true;
    }

    private void FindMatches()
    {
        var flaggedTiles = new HashSet<TileManager>();

        for (int x = 0; x < W ; x++)
        {
            for (int y = 0; y < H ; y++)
            {
                TileManager originTile = NameAsPositionSetter.GetObject(x, y).GetComponent<TileManager>();
                TileManager tile2 = null;
                TileManager tile3 = null;

                // Horizontal matches
                if (x < W - 2 )
                {
                    tile2 = NameAsPositionSetter.GetObject(x + 1 , y).GetComponent<TileManager>();
                    tile3 = NameAsPositionSetter.GetObject(x + 2,  y).GetComponent<TileManager>();
                    if (originTile.CompareTag(tile2.tag) && originTile.CompareTag(tile3.tag))
                        flaggedTiles.UnionWith(new[] { originTile, tile2, tile3 });
                }

                // Vertical matches
                if (y < H - 2 )
                {
                    tile2 = NameAsPositionSetter.GetObject(x, y + 1 ).GetComponent<TileManager>();
                    tile3 = NameAsPositionSetter.GetObject(x, y + 2 ).GetComponent<TileManager>();
                    if (originTile.CompareTag(tile2.tag) && originTile.CompareTag(tile3.tag))
                        flaggedTiles.UnionWith(new[] { originTile, tile2, tile3 });
                }
            }
        }

        //iterate over flagged tiles
        if (flaggedTiles.Count > 0)
        {
            bool madeSound = false;

            foreach (var tile in flaggedTiles)
            {
                if (IsSoundOn && !madeSound && isMatchFound && tile.IsBeingDestroyed) gameSoundsPlayer.PlayOneShot(matchClip, 0.2f);
                    else madeSound = true;
                if (isMatchFound && tile.IsBeingDestroyed) Instantiate(matchedParticles, tile.transform.position, Quaternion.identity); 
                
                tile.OnMatch();
            }

            isMatchFound = true;
            isRegenerating = true;
        }
        else
        {
            isMatchFound = false;
        }

        isFindingMatches = false;
    }

    private void regenerateBoard()
    {
        for (int x = 0; x < W; x ++)
        {
            int countOfTilesToFall = 0;
            for (int y = 0; y < H; y ++)
            {
                var obj = NameAsPositionSetter.GetObject(x, y);
                if (obj == null)
                    countOfTilesToFall++;
                else 
                    obj.GetComponent<TileManager>().OnFall(countOfTilesToFall);
            }
            SpawnManager.Instance.SpawnTilesAtCol(x, countOfTilesToFall);
        }
        isRegenerating = false;
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
