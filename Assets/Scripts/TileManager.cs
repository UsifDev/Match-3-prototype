using Tiles;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using static UnityEditor.PlayerSettings;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class TileManager : MonoBehaviour
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

    public static GameObject SelectedTile {  get; private set; }
    public static GameObject TargetTile { get; private set; }

void Start()
    {
        NullifyStaticTiles();
        controls = new();
    }

    void Delete()
    {
        Destroy(gameObject);
    }
    private void OnMouseDown()
    {
        if (IsBusy) return;
        firstTime = (float)Time.realtimeSinceStartup;
        SelectedTile = gameObject;
    }

    private void OnMouseDrag()
    {
        isBeingDragged = true;
    }

    private void OnMouseEnter()
    {
        if(!isBeingDragged && !IsBusy) TargetTile = gameObject;
    }
    private void OnMouseExit()
    {
        if (!isBeingDragged) TargetTile = null;
    }

    private void OnMouseUp()
    {
        if (!isBeingDragged) return;
        isBeingDragged = false;

        if (TargetTile == null ||
            (float)Time.realtimeSinceStartup - firstTime > MAXTIME ||
            Vector3.Distance(TargetTile.transform.position, SelectedTile.transform.position) != 1 ||
            TargetTile.name == SelectedTile.name)
        {
            NullifyStaticTiles();
            return;
        }

        GameManager.Instance.Swap();
    }

    private void OnSwipe(InputValue touchValue)
    {
        var touch = touchValue.Get<TouchState>();
        if (touch.phase != TouchPhase.Ended) return;

        firstTime = (float)touch.startTime;
        var lastTime = (float)Time.realtimeSinceStartup;
        if (lastTime - firstTime > MAXTIME) return;

        Vector3 firstPos = Camera.main.ScreenToWorldPoint(touch.startPosition);
        Vector3 lastPos = Camera.main.ScreenToWorldPoint(touch.position);

        if (NameAsPositionSetter.CompareNameWithVector(gameObject, firstPos))
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

        if (diffX < MAXRADIUS && diffY < MAXRADIUS &&
            diffX > MINRADIUS && diffY > MINRADIUS)
        {
            NullifyStaticTiles();
            return;
        }

        float swapAngle = Mathf.Rad2Deg * Mathf.Atan2(
            lastPos.y - firstPos.y,
            lastPos.x - firstPos.x
        );

        Vector2Int currentTile = new Vector2Int(
            (int)transform.position.x,
            (int)transform.position.y
        );

        Vector2Int targetTile = currentTile;

        if (swapAngle > -45 && swapAngle <= 45 && currentTile.x < SpawnManager.boardWidth - 1)
            TargetTile = GameObject.Find((transform.position.x + 1) + "," + (transform.position.y)); // Right swap
        else if (swapAngle > 45 && swapAngle <= 135 && currentTile.y < SpawnManager.boardHeight - 1)
            TargetTile = GameObject.Find((transform.position.x) + "," + (transform.position.y + 1)); // Up swap
        else if ((swapAngle > 135 || swapAngle <= -135) && currentTile.x > 0)
            TargetTile = GameObject.Find((transform.position.x - 1) + "," + (transform.position.y)); // Left swap
        else if (swapAngle > -135 && swapAngle <= -45 && currentTile.y > 0)
            TargetTile = GameObject.Find((transform.position.x) + "," + (transform.position.y - 1)); // Down swap

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
    }

    // ABSTRACTION
    // ENCAPSULATION
    public void AnimateFalling(int countOfTilesToFall)
    {
        gameObject.GetComponent<TileAnimator>().AnimateMovement(countOfTilesToFall);
    }

    // ABSTRACTION
    // ENCAPSULATION
    public void AnimateMovement(Vector3 otherPos)
    {
        gameObject.GetComponent<TileAnimator>().AnimateMovement(otherPos);
    }

    public void DestroyTile()
    {
        if (gameObject.GetComponent<TileAnimator>().IsBeingDestroyed)
            Destroy(gameObject);
        else
            gameObject.SendMessage("OnDestroyTile");
    }
}
