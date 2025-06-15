using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine.UI;
using System;
using Random = UnityEngine.Random;

public class BoardController : MonoBehaviour
{
    public int width = 5;
    public int height = 8;
    public GameObject[] itemPrefabs;
    public float animationDuration = 1f;
    public float fallAnimationMultiplier = 0.4f;
    public float swapAnimationMultiplier = 0.5f;
    public float clearAnimationMultiplier = 0.3f;

    public UnityEngine.UI.Button restartButton;
    public Toggle isRandomToggle;
    public bool isRandomValue;
    public InputField rngSeedInputField;
    private int rngSeed;

    public enum GameState
    {
        Idle,
        Swapping,
        Clearing,
        Falling
    }

    public GameState CurrentState { get; private set; } = GameState.Idle;

    private BoardState _boardState;
    private GameObject[,] _tileObjects;

    void Start()
    {
        InitializeBoard();
        rngSeed = 42;
        isRandomValue = isRandomToggle.isOn;
        isRandomToggle.onValueChanged.AddListener(ToggleRandomness);
        restartButton.onClick.AddListener(Restart);
        rngSeedInputField.onEndEdit.AddListener(SetRNGSeed);
    }

    private void InitializeBoard()
    {
        if (isRandomValue) Random.InitState((int)DateTime.Now.Ticks);
        else Random.InitState(rngSeed);
        _boardState = BoardState.CreateBoard(width, height, itemPrefabs.Length);
        _tileObjects = new GameObject[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int tileType = _boardState.TileTypes[x, y];
                _tileObjects[x, y] = Instantiate(
                    itemPrefabs[tileType],
                    new Vector3(x, y, 0),
                    Quaternion.identity,
                    transform
                );
            }
        }
    }

    private void Restart()
    {
        _boardState = null;
        for (int x = 0; x < width; x++)
        for (int y = 0; y < height; y++)
        Destroy(_tileObjects[x, y]);
        InitializeBoard();
    }

    private void ToggleRandomness(bool toggleValue)
    {
        isRandomValue = toggleValue;
    }

    private void SetRNGSeed(string seed)
    {
        rngSeed = int.Parse(seed);
    }

    public async Task<bool> AttemptSwapAsync(Vector2Int pos1, Vector2Int pos2)
    {
        if (CurrentState != GameState.Idle) return false;

        if (!IsValidSwap(pos1, pos2)) return false;

        CurrentState = GameState.Swapping;

        _boardState.SwapTiles(pos1, pos2);
        await AnimateSwapAsync(pos1, pos2);

        var matches = _boardState.FindMatches();
        if (matches.Any())
        {
            await ClearMatchesAsync(matches);
            return true;
        }

        _boardState.SwapTiles(pos1, pos2);
        await AnimateSwapAsync(pos1, pos2);

        CurrentState = GameState.Idle;
        return false;
    }

    private bool IsValidSwap(Vector2Int pos1, Vector2Int pos2)
    {
        return pos2.x >= 0 && pos2.x < width && pos2.y >= 0 && pos2.y < height && _boardState.TileTypes[pos2.x, pos2.y] != -1;
    }


    private async Task ClearMatchesAsync(IEnumerable<Vector2Int> matches)
    {
        CurrentState = GameState.Clearing;

        var animationTasks = matches.Select(match =>
            AnimateClearAsync(_tileObjects[match.x, match.y])
        ).ToList();

        await Task.WhenAll(animationTasks);

        foreach (var match in matches)
        {
            Destroy(_tileObjects[match.x, match.y]);
            _tileObjects[match.x, match.y] = null;
            _boardState.TileTypes[match.x, match.y] = -1;
        }

        await FallDownAsync();
    }


    private async Task FallDownAsync()
    {
        CurrentState = GameState.Falling;

        var animationTasks = new List<Task>();

        for (int x = 0; x < width; x++)
        {
            int emptySlot = height;

            for (int y = 0; y < height; y++)
            {
                if (_boardState.TileTypes[x, y] == -1)
                {
                    if (emptySlot == height) emptySlot = y;
                }
                else if (emptySlot != height)
                {
                    var tileObject = _tileObjects[x, y];
                    var targetPos = new Vector3(x, emptySlot, 0);

                    animationTasks.Add(AnimateFallAsync(tileObject, tileObject.transform.position, targetPos));

                    _boardState.TileTypes[x, emptySlot] = _boardState.TileTypes[x, y];
                    _boardState.TileTypes[x, y] = -1;

                    _tileObjects[x, emptySlot] = _tileObjects[x, y];
                    _tileObjects[x, y] = null;

                    emptySlot++;
                }
            }
        }

        await RegenerateBoardAsync();
        await Task.WhenAll(animationTasks);
        await ProcessMatchesAfterRegenerationAsync();
    }

    private async Task RegenerateBoardAsync()
    {
        var animationTasks = new List<Task>();

        for (int x = 0; x < width; x++)
        {
            int posYs = 0;
            
            for (int y = 0; y < height; y++)
            {
                if (_boardState.TileTypes[x, y] == -1)
                {
                    posYs++;
                }
            }

            for (int y = 0; y < height; y++)
            {
                if (_boardState.TileTypes[x, y] == -1)
                {
                    int newTileType = Random.Range(0, itemPrefabs.Length);
                    _boardState.TileTypes[x, y] = newTileType;

                    var newTile = Instantiate(
                        itemPrefabs[newTileType],
                        new Vector3(x, y + posYs, 0),
                        Quaternion.identity,
                        transform
                    );

                    _tileObjects[x, y] = newTile;

                    animationTasks.Add(AnimateFallAsync(newTile, newTile.transform.position, new Vector3(x, y, 0)));
                }
            }
        }

        await Task.WhenAll(animationTasks);
    }

    private async Task ProcessMatchesAfterRegenerationAsync()
    {
        while (true)
        {
            var matches = _boardState.FindMatches();

            if (!matches.Any())
            {
                CurrentState = GameState.Idle;
                break;
            }

            await ClearMatchesAsync(matches);
        }
    }

    //--------------------------------------------------------------
    //                        Animations
    //--------------------------------------------------------------

    private async Task AnimateSwapAsync(Vector2Int pos1, Vector2Int pos2)
    {
        Vector3 startPos1 = _tileObjects[pos1.x, pos1.y].transform.position;
        Vector3 startPos2 = _tileObjects[pos2.x, pos2.y].transform.position;

        float elapsedTime = 0f;
        float duration = swapAnimationMultiplier * animationDuration;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            _tileObjects[pos1.x, pos1.y].transform.position = Vector3.Lerp(startPos1, startPos2, t);
            _tileObjects[pos2.x, pos2.y].transform.position = Vector3.Lerp(startPos2, startPos1, t);

            await Task.Yield();
        }

        // Swap tile objects
        (_tileObjects[pos1.x, pos1.y], _tileObjects[pos2.x, pos2.y]) =
            (_tileObjects[pos2.x, pos2.y], _tileObjects[pos1.x, pos1.y]);
    }
    private async Task AnimateClearAsync(GameObject tile)
    {
        if (tile == null) return;

        var spriteRenderer = tile.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null) return;

        Color startColor = spriteRenderer.color;
        Vector3 startScale = tile.transform.localScale;
        float elapsedTime = 0f;
        float duration = clearAnimationMultiplier * animationDuration;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            spriteRenderer.color = Color.Lerp(startColor, Color.clear, t);
            tile.transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);

            await Task.Yield();
        }
    }
    private async Task AnimateFallAsync(GameObject tile, Vector3 startPos, Vector3 targetPos)
    {
        float elapsedTime = 0f;
        float duration = fallAnimationMultiplier * animationDuration;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            tile.transform.position = Vector3.Lerp(startPos, targetPos, t);

            await Task.Yield();
        }

        tile.transform.position = targetPos;
    }
}
