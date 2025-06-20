using UnityEngine;

public class NameAsPositionSetter : MonoBehaviour
{
    void Start()
    {
        ResetName();
    }

    void Update()
    {
        if (gameObject.name != transform.position.x / GameManager.GameScaling + "," + transform.position.y / GameManager.GameScaling) ResetName();
    }

    void ResetName()
    {
        gameObject.name = "" + transform.position.x / GameManager.GameScaling + "," + transform.position.y / GameManager.GameScaling;
    }

    public static bool CompareNameWithVector(GameObject obj, Vector3 pos)
    {
        return obj.name == Mathf.RoundToInt(pos.x) + "," + Mathf.RoundToInt(pos.y);
    }

    public static GameObject GetObject(Vector3 pos)
    {
        return GameObject.Find((int) pos.x + "," + (int) pos.y);
    }

    public static GameObject GetObject(int x, int y)
    {
        return GameObject.Find(x + "," + y);
    }
}
