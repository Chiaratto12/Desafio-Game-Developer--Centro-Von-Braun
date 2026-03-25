using UnityEngine;

public class VehicleController : MonoBehaviour
{
    private float _speed;
    private Vector3 _direction;
    private Vector3 _despawnPosition;
    private float _cellSize;

    /// <summary>
    /// Inicializa o veículo
    /// </summary>
    /// <param name="speed">Velocidade do veículo</param>
    /// <param name="despawnPos">Posição para o veículo sumir</param>
    /// <param name="direction">Drieção que o veículo irá</param>
    /// <param name="cellSize">Tamanho da célula no grid</param>
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

    /// <summary>
    /// Calcula se o veículo chegou no objeto para sumir
    /// </summary>
    /// <returns></returns>
    private bool HasReachedDespawn()
    {
        return Vector3.Dot(_direction, _despawnPosition - transform.position) <= 0;
    }

    public Vector2Int CurrentCell => new Vector2Int(
        Mathf.RoundToInt(transform.position.x / _cellSize),
        Mathf.RoundToInt(transform.position.z / _cellSize)
    );
}