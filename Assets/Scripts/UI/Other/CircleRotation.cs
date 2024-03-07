using UnityEngine;

public class CircleRotation : MonoBehaviour
{
    private float roationSpeed = 25f;
    private bool clockwise = true;

    void Start()
    {
        if (!clockwise)
        {
            roationSpeed = roationSpeed * -1;
        }
    }

    void Update()
    {
        transform.Rotate(0, 0, roationSpeed * Time.deltaTime);
    }
}
