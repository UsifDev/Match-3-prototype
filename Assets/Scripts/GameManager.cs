using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Toggle isRandomToggle;
    [SerializeField] private InputField rngSeedInputField;

    private bool isRandomValue;

    public enum GameState
    {
        IDLE,
        SCANNING,
    } 
    public GameState state {  get; private set; }

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
        state = GameState.IDLE;
    }

    
    void Update()
    {
        if (state == GameState.SCANNING && !SelectedTileManager.IsBusy && !TargetTileManager.IsBusy) 
        {
            state = GameState.IDLE;
        }
    }

    public void Swap()
    {
        SelectedTileManager.OnSwap(TargetTileManager.transform.position);
        TargetTileManager.OnSwap(SelectedTileManager.transform.position);

        state = GameState.SCANNING;
    }

    //private void FindMatches()
    //{
    //    var matches = new HashSet<Vector2Int>();

    //    int startingX = (int) SelectedTileManager.transform.position.x - 2;
    //    float startingY = SelectedTileManager.transform.position.y - 2;

    //    int rowsToScan = 5;

    //    if (startingX > 0) {


    //        startingX = 0;
    //    }

    //    for (int x = 0; x < W; x++)
    //    {
    //        for (int y = 0; y < H; y++)
    //        {
    //            var tile1 = NameAsPositionSetter.GetObject(x, y);
    //            var tile2 = NameAsPositionSetter.GetObject(x + 1, y);
    //            var tile3 = NameAsPositionSetter.GetObject(x + 2, y);

    //            // Horizontal matches
    //            if (x < W - 2 &&
    //                tile1.tag == tile2.tag &&
    //                tile1.tag == tile3.tag)
    //            {
    //                SelectedTileManager
    //            }

    //            tile2 = NameAsPositionSetter.GetObject(x, y + 1);
    //            tile3 = NameAsPositionSetter.GetObject(x, y + 2);

    //            // Vertical matches
    //            if (x < H - 2 &&
    //                tile1.tag == tile2.tag &&
    //                tile1.tag == tile3.tag)
    //            {
                    
    //            }
    //            // 
    //            if (y < Height - 2 &&
    //                TileTypes[x, y] == TileTypes[x, y + 1] &&
    //                TileTypes[x, y] == TileTypes[x, y + 2])
    //            {
    //                matches.UnionWith(new[]
    //                {
    //                    new Vector2Int(x, y),
    //                    new Vector2Int(x, y+1),
    //                    new Vector2Int(x, y+2)
    //                });
    //            }
    //        }
    //    }
    //}

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
