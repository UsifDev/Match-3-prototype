using Tiles;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public bool IsBusy
    {
        get
        {
            return gameObject.GetComponent<TileAnimator>().IsBusy;
        }
    }

    public bool IsBeingDestroyed
    {
        get
        {
            return gameObject.GetComponent<TileAnimator>().IsBeingDestroyed;
        }
    }

    public void OnMatch()
    {
        if (IsBusy) return;
        DestroyTile();
    }

    public void OnSwap(Vector3 otherPos)
    {
        AnimateMovement(otherPos);
    }

    // ABSTRACTION
    // ENCAPSULATION
    public void OnFall(int countOfTilesToFall)
    {
        if (countOfTilesToFall <= 0) return;
        gameObject.GetComponent<TileAnimator>().AnimateMovement(countOfTilesToFall);
    }

    // ABSTRACTION
    // ENCAPSULATION
    private void AnimateMovement(Vector3 otherPos)
    {
        gameObject.GetComponent<TileAnimator>().AnimateMovement(otherPos);
    }

    private void DestroyTile()
    {
        if (gameObject.GetComponent<TileAnimator>().IsBeingDestroyed)
            Destroy(gameObject);
        else
            gameObject.SendMessage("OnDestroyTile");
    }
}
