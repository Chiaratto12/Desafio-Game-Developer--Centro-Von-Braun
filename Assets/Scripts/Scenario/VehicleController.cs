using UnityEngine;

public class VehicleController : MonoBehaviour
{
    private float _speed;
    private Vector3 _direction;
    private Vector3 _despawnPosition;
    private float _cellSize;

    public void Initialize(float speed, Vector3 despawnPos, Vector3 direction, float cellSize)
    {
        _speed = speed;
        _despawnPosition = despawnPos;
        _direction = direction.normalized;
        _cellSize = cellSize;
    }

    private void Update()
    {
        transform.Translate(_direction * _speed * Time.deltaTime);

        if (HasReachedDespawn())
            Destroy(gameObject);
    }

    private bool HasReachedDespawn()
    {
        return Vector3.Dot(_direction, _despawnPosition - transform.position) <= 0;
    }

    public Vector2Int CurrentCell => new Vector2Int(
        Mathf.RoundToInt(transform.position.x / _cellSize),
        Mathf.RoundToInt(transform.position.z / _cellSize)
    );
}