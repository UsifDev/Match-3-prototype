using UnityEngine;

public class MoveBackground : MonoBehaviour
{
    private float wiggleXRange = 7f;
    private float wiggleYRange = 10f;
    private Vector3 direction;

    private float rotationSpeed = 0.2f;
    private float animationSpeed = 0.5f;

    void Start()
    {
        direction = new Vector3(wiggleXRange,wiggleYRange,0).normalized;
    }

    void Update()
    {
        transform.Translate(animationSpeed * Time.deltaTime * direction);

        if( transform.position.x < -wiggleXRange || transform.position.x > wiggleXRange)
            direction = new(-direction.x, direction.y);

        if (transform.position.y < -wiggleYRange || transform.position.y > wiggleYRange)
            direction = new(direction.x, -direction.y);

        transform.Rotate(rotationSpeed * Time.deltaTime * Vector3.forward);
    }
}
