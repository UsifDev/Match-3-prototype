using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Toggle isRandomToggle;
    [SerializeField] private InputField rngSeedInputField;

    private bool isRandomValue;
    public static GameManager Instance { get; private set; } // ENCAPSULATION


    private int w { get { return SpawnManager.boardWidth; } }
    private int h { get { return SpawnManager.boardHeight; } }
    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        SpawnManager.Instance.InitializeBoard(isRandomValue);
    }

    
    void Update()
    {

    }
    public void Swap()
    {
        var temp = TileManager.SelectedTile.name.Split(",");
        Vector2Int currentTile = new(int.Parse(temp[0]), int.Parse(temp[1]));

        temp = TileManager.TargetTile.name.Split(",");
        Vector2Int targetTile = new(int.Parse(temp[0]), int.Parse(temp[1]));

        Swap(currentTile, targetTile);
    }

    public void Swap(Vector2Int currentTile, Vector2Int targetTile)
    {
        Debug.Log("currentTile :" + currentTile.ToString() + TileManager.SelectedTile.name);
        Debug.Log("targetTile :" + targetTile.ToString() + TileManager.TargetTile.name);
    }



    private bool IsValidTarget(Vector2Int targetTile)
    {
        return targetTile.x >= 0 && targetTile.x < w &&
               targetTile.y >= 0 && targetTile.y < h;
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
