using UnityEngine;

public class MoveBackground : MonoBehaviour
{
    private float wiggleXRange = 7f;
    private float wiggleYRange = 10f;
    private byte translationSign = 1;
    private Vector3 direction;

    private float rotationSpeed = 0.2f;
    private float animationSpeed = 0.5f;

    void Start()
    {
        direction = new Vector3(wiggleXRange,wiggleYRange,0).normalized;
    }

    void Update()
    {
        transform.Translate(Mathf.Pow(-1,translationSign) * animationSpeed * Time.deltaTime * direction);
        if( transform.position.x < -wiggleXRange || transform.position.y < -wiggleYRange)
        {
            translationSign = 0;
        } else if ( transform.position.x > wiggleXRange || transform.position.y > wiggleYRange)
        {
            translationSign = 1;
        }

        transform.Rotate(rotationSpeed * Time.deltaTime * Vector3.forward);
    }
}
