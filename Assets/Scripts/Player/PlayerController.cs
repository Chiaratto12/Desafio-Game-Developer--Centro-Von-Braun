using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Grid Config")]
    public float cellSize = 1f;
    public float moveTime = 0.15f;

    private Vector2Int _gridPosition;
    private bool _isMoving = false;
    private float _currentSpeedMultiplier = 1f;

    private Camera _camera;

    private void Start()
    {
        _camera = Camera.main;
    }

    private void Update()
    {
        if (_isMoving) return;

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        if (h != 0 || v != 0)
        {
            Vector2Int direction = Vector2Int.zero;

            if (Mathf.Abs(v) > 0)
                direction = v > 0 ? Vector2Int.up : Vector2Int.down;
            else
                direction = h > 0 ? Vector2Int.right : Vector2Int.left;

            TryMove(direction);
        }
    }

    /// <summary>
    /// Calcula se é possível mover o jogador na direção desejada
    /// </summary>
    /// <param name="direction">Direção que o jogador irá</param>
    private void TryMove(Vector2Int direction)
    {
        Vector2Int targetCell = _gridPosition + direction;

        if (!IsWithinBounds(targetCell)) return;

        _gridPosition = targetCell;
        StartCoroutine(SmoothMove(targetCell));
    }
    
    /// <summary>
    /// Movimenta o jogador com suavização
    /// </summary>
    /// <param name="targetCell">Célula no grid onde o jogador irá se mover</param>
    /// <returns></returns>
    private IEnumerator SmoothMove(Vector2Int targetCell)
    {
        _isMoving = true;

        Vector2 startPos = transform.position;
        Vector2 endPos = GridToWorld(targetCell);

        float adjustedMoveTime = moveTime / _currentSpeedMultiplier;
        float elapsed = 0f;

        while (elapsed < adjustedMoveTime)
        {
            elapsed += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, endPos, elapsed / adjustedMoveTime);
            yield return null;
        }

        transform.position = endPos;
        _isMoving = false;
    }

    /// <summary>
    /// Retorna posição da célula no grid em vetor
    /// </summary>
    /// <param name="cell">Célula do grid</param>
    /// <returns></returns>
    private Vector2 GridToWorld(Vector2Int cell)
    {
        return new Vector2(cell.x * cellSize, cell.y * cellSize);
    }

    /// <summary>
    /// Seta o multiplicador de velocidade baseado no tipo de clima
    /// </summary>
    /// <param name="weather">Clima setado</param>
    public void SetWeatherMultiplier(string weather)
    {
        _currentSpeedMultiplier = weather switch
        {
            "sunny" => 1.0f,
            "clouded" => 0.8f,
            "foggy" => 0.8f,
            "light rain" => 0.6f,
            "heavy rain" => 0.4f,
            _ => 1.0f
        };
    }

    /// <summary>
    /// Calcula se o jogador chegou no limite desejado no grid
    /// </summary>
    /// <param name="cell">Célula atual onde o jogador está</param>
    /// <returns></returns>
    private bool IsWithinBounds(Vector2Int cell)
    {
        return cell.x >= -9 && cell.x < 10 && cell.y >= -4;
    }
}
