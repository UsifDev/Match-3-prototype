using Tiles;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class TileInputManager : MonoBehaviour
{
    private const float MINRADIUS = 0.1f;
    private const float MAXRADIUS = 0.4f;
    private const float MAXTIME = 2f;

    private float firstTime;
    private TileControls controls;

    private bool isBeingDragged;

    public bool IsBusy
    {
        get
        {
            return gameObject.GetComponent<TileAnimator>().IsBusy;
        }
    }

    public static GameObject SelectedTile { get; private set; }
    public static GameObject TargetTile { get; private set; }

    private static TouchState touch;
    private static bool hasTouch;
    private static Vector3 firstPos;
    private static Vector3 lastPos;

    void Start()
    {
        NullifyStaticTiles();
        controls = new();
    }

    private void OnMouseDown()
    {
        if (GameManager.Instance.IsScanning) return;
        if (IsBusy) return;
        firstTime = (float)Time.realtimeSinceStartup;
        SelectedTile = gameObject;
    }

    private void OnMouseDrag()
    {
        if (GameManager.Instance.IsScanning) return;
        isBeingDragged = true;
    }

    private void OnMouseEnter()
    {
        if (GameManager.Instance.IsScanning) return;
        if (!isBeingDragged && !IsBusy) TargetTile = gameObject;
    }
    private void OnMouseExit()
    {
        if (GameManager.Instance.IsScanning) return;
        if (!isBeingDragged) TargetTile = null;
    }

    private void OnMouseUp()
    {
        if (GameManager.Instance.IsScanning) return;
        if (!isBeingDragged) return;
        isBeingDragged = false;

        if (TargetTile == null ||
            (float)Time.realtimeSinceStartup - firstTime > MAXTIME ||
            Vector3.Distance(TargetTile.transform.position, SelectedTile.transform.position) != GameManager.GameScaling ||
            TargetTile.name == SelectedTile.name)
        {
            NullifyStaticTiles();
            return;
        }

        GameManager.Instance.Swap();
    }

    private void OnSwipe(InputValue touchValue)
    {
        if (GameManager.Instance.IsScanning) return;
        
        if (!hasTouch) { 
            touch = touchValue.Get<TouchState>();
            hasTouch = true;

            if (touch.phase != TouchPhase.Ended) return;

            firstTime = (float)touch.startTime;
            var lastTime = (float)Time.realtimeSinceStartup;
            if (lastTime - firstTime > MAXTIME) return;

            firstPos = Camera.main.ScreenToWorldPoint(touch.startPosition);
            lastPos = Camera.main.ScreenToWorldPoint(touch.position);
        }

        if (NameAsPositionSetter.CompareNameWithVector(gameObject, firstPos / GameManager.GameScaling))
        {
            SelectedTile = gameObject;
        }
        else
        {
            NullifyStaticTiles();
            return;
        }

        var diffX = Mathf.Abs(firstPos.x - lastPos.x);
        var diffY = Mathf.Abs(firstPos.y - lastPos.y);

        if (diffX < MAXRADIUS * GameManager.GameScaling && diffY < MAXRADIUS * GameManager.GameScaling &&
            diffX > MINRADIUS * GameManager.GameScaling && diffY > MINRADIUS * GameManager.GameScaling)
        {
            NullifyStaticTiles();
            return;
        }

        float swapAngle = Mathf.Rad2Deg * Mathf.Atan2(
            lastPos.y - firstPos.y,
            lastPos.x - firstPos.x
        );

        int x = Mathf.RoundToInt(transform.position.x / GameManager.GameScaling);
        int y = Mathf.RoundToInt(transform.position.y / GameManager.GameScaling);

        if (swapAngle > -45 && swapAngle <= 45 && x < SpawnManager.boardWidth - 1)
            TargetTile = NameAsPositionSetter.GetObject(new(x + 1, y)); // Right swap
        else if (swapAngle > 45 && swapAngle <= 135 && y < SpawnManager.boardHeight - 1)
            TargetTile = NameAsPositionSetter.GetObject(new(x, y + 1)); // Up swap
        else if ((swapAngle > 135 || swapAngle <= -135) && x > 0)
            TargetTile = NameAsPositionSetter.GetObject(new(x - 1, y)); // Left swap
        else if (swapAngle > -135 && swapAngle <= -45 && y > 0)
            TargetTile = NameAsPositionSetter.GetObject(new(x, y - 1)); // Down swap

        if (TargetTile.name == SelectedTile.name)
        {
            NullifyStaticTiles();
            return;
        } else
        {
            GameManager.Instance.Swap();
        }
    }

    private void NullifyStaticTiles()
    {
        SelectedTile = null;
        TargetTile = null;
        hasTouch = false;
    }
}
