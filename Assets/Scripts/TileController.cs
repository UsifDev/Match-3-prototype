using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;
public class TileController : MonoBehaviour
{
    private const float MINRADIUS = 0.1f;
    private const float MAXRADIUS = 0.4f;
    private const float MAXTIME = 2f;

    private BoardController boardController;
    private GemControls controls;

    void Start()
    {
        boardController = FindFirstObjectByType<BoardController>();
        controls = new();
    }

    private void OnSwipe(InputValue touchValue)
    {
        var touch = touchValue.Get<TouchState>();
        if (touch.phase != TouchPhase.Ended) return;

        var firstTime = (float)touch.startTime;
        var lastTime = (float)Time.realtimeSinceStartup;

        if (lastTime - firstTime > MAXTIME) return;

        var firstTouchPos = Camera.main.ScreenToWorldPoint(touch.startPosition);
        var lastTouchPos = Camera.main.ScreenToWorldPoint(touch.position);

        if (Mathf.RoundToInt(firstTouchPos.x) != transform.position.x ||
            Mathf.RoundToInt(firstTouchPos.y) != transform.position.y) return;

        var diffX = Mathf.Abs(firstTouchPos.x - lastTouchPos.x);
        var diffY = Mathf.Abs(firstTouchPos.y - lastTouchPos.y);

        if (diffX < MAXRADIUS && diffY < MAXRADIUS &&
            diffX > MINRADIUS && diffY > MINRADIUS)
            return;

        float swipeAngle = Mathf.Rad2Deg * Mathf.Atan2(
            lastTouchPos.y - firstTouchPos.y,
            lastTouchPos.x - firstTouchPos.x
        );

        Vector2Int currentTile = new Vector2Int(
            (int)transform.position.x,
            (int)transform.position.y
        );

        Vector2Int targetTile = currentTile;

        if (swipeAngle > -45 && swipeAngle <= 45 && currentTile.x < boardController.width - 1)
            targetTile.x += 1; // Right swipe
        else if (swipeAngle > 45 && swipeAngle <= 135 && currentTile.y < boardController.height - 1)
            targetTile.y += 1; // Up swipe
        else if ((swipeAngle > 135 || swipeAngle <= -135) && currentTile.x > 0)
            targetTile.x -= 1; // Left swipe
        else if (swipeAngle > -135 && swipeAngle <= -45 && currentTile.y > 0)
            targetTile.y -= 1; // Down swipe

        _ = boardController.AttemptSwapAsync(currentTile, targetTile);
    }
}