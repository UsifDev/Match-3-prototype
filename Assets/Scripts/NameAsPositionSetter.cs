using UnityEngine;

public class NameAsPositionSetter : MonoBehaviour
{
    void Start()
    {
        ResetName();
    }

    void Update()
    {
        if (!NameMatchesPos(gameObject)) ResetName();
    }

    void ResetName()
    {
        gameObject.name = "" + transform.position.x + "," + transform.position.y;
    }

    public static bool NameMatchesPos(GameObject obj)
    {
        return obj.name == obj.transform.position.x + "," + obj.transform.position.y;
    }

    public static bool CompareNameWithVector(GameObject obj, Vector3 pos)
    {
        return obj.name == pos.x + "," + pos.y;
    }

    public static GameObject FindObjectWithPosition(Vector3 pos)
    {
        return 
    }
}
