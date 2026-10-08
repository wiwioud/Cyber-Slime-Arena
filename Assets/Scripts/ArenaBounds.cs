using UnityEngine;

public class ArenaBounds : MonoBehaviour
{
    public float minX = -8.2f;
    public float maxX = 8.2f;
    public float minY = -4.4f;
    public float maxY = 4.4f;

    void LateUpdate()
    {
        Vector3 position = transform.position;

        position.x = Mathf.Clamp(position.x, minX, maxX);
        position.y = Mathf.Clamp(position.y, minY, maxY);

        transform.position = position;
    }
}