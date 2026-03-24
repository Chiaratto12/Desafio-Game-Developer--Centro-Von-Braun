using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CameraController : MonoBehaviour
{
    [Header("Target")]
    public Transform player;

    [Header("Grid Config")]
    public float cellSize = 1f;

    [Header("Camera Offset")]
    public Vector3 offset;

    [Header("Smoothing")]
    public float smoothSpeed = 0.1f;

    private float _maxPlayerY;
    private Vector3 _targetPosition;

    private void Start()
    {
        if (player == null)
        {
            Debug.LogError("[CameraController] Player não atribuído!");
            return;
        }

        _maxPlayerY = player.position.y;
        transform.position = GetDesiredPosition(_maxPlayerY);
    }

    private void LateUpdate()
    {
        if (player == null) return;

        if (player.position.y > _maxPlayerY)
            _maxPlayerY = player.position.y;

        _targetPosition = GetDesiredPosition(_maxPlayerY);

        transform.position = Vector3.Lerp(transform.position, _targetPosition, smoothSpeed);
    }


    private Vector3 GetDesiredPosition(float targetY)
    {
        return new Vector3(
            offset.x,
            targetY + offset.y,                
            offset.z
        );
    }


    public void ResetProgress()
    {
        if (player == null) return;

        _maxPlayerY = player.position.z;
        transform.position = GetDesiredPosition(_maxPlayerY);
    }
}