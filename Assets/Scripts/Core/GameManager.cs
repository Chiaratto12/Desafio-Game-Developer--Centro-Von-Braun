using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState { Boot, Loading, Playing, Win, GameOver }
    public GameState CurrentState { get; private set; } = GameState.Boot;

    [Header("References")]
    public TrafficApiService apiService;
    public TrafficController trafficController;
    public WeatherController weatherController;
    public PredictionController predictionController;
    public PlayerController playerController;
    public CameraController cameraController;
    public HUDController hudController;

    [Header("Config")]
    public int startLevel = 1;

    private int _currentLevel;
    private float _timeLimit;
    private float _timeRemaining;
    private bool _timerRunning;
    public Vector2 goalLine;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        _currentLevel = startLevel;
        TransitionTo(GameState.Loading);
    }

    private void Update()
    {
        if (!_timerRunning || CurrentState != GameState.Playing) return;

        _timeRemaining -= Time.deltaTime;
        hudController.UpdateTimer(_timeRemaining);

        if (_timeRemaining <= 0f)
            TriggerGameOver();
    }

    /// <summary>
    /// Transita para um novo estado de jogo
    /// </summary>
    /// <param name="next">Estado de jogo selecionado</param>
    private void TransitionTo(GameState next)
    {
        CurrentState = next;

        switch (next)
        {
            case GameState.Loading: StartCoroutine(LoadLevel()); break;
            case GameState.Playing: BeginPlay(); break;
            case GameState.Win: StartCoroutine(HandleWin()); break;
            case GameState.GameOver: HandleGameOver(); break;
        }
    }

    /// <summary>
    /// Carrega nova fase
    /// </summary>
    /// <returns></returns>
    private IEnumerator LoadLevel()
    {
        hudController.ShowMessage($"Carregando nível {_currentLevel}...");

        TrafficResponse response = null;

        // Tenta API remota; em falha, usa mock local
        yield return apiService.FetchTrafficStatus(
            onSuccess: data => response = data,
            onError: error =>
            {
                Debug.LogWarning($"[GameManager] API falhou ({error}). Usando mock local.");
                response = apiService.LoadLocalMock();
            }
        );

        if (response == null)
        {
            Debug.LogError("[GameManager] Sem dados. Verifique o mock.");
            yield break;
        }

        InitializeSimulation(response);
        TransitionTo(GameState.Playing);
    }

    /// <summary>
    /// Inicializa nova simulação, setando todos os valores da fase
    /// </summary>
    /// <param name="response">Resposta da API</param>
    private void InitializeSimulation(TrafficResponse response)
    {
        trafficController.ApplyStatus(response.current_status);
        weatherController.ApplyWeather(response.current_status.weather);
        playerController.SetWeatherMultiplier(response.current_status.weather);

        predictionController.Initialize(trafficController, weatherController, hudController);
        predictionController.SchedulePredictions(response.predicted_status);

        var lastPrediction = response.predicted_status[^1];
        _timeLimit = lastPrediction.estimated_time / 1000f;
        _timeRemaining = _timeLimit;

        hudController.UpdateHUD(_currentLevel, response.current_status);

        Debug.Log($"[GameManager] Nível {_currentLevel} | Tempo limite: {_timeLimit}s");
    }

    /// <summary>
    /// Inicia a fase
    /// </summary>
    private void BeginPlay()
    {
        playerController.SetInputEnabled(true);
        playerController.ResetPosition();
        cameraController.ResetProgress();
        _timerRunning = true;
        hudController.ShowMessage("");
    }

    /// <summary>
    /// Gatilho para vitória do jogador
    /// </summary>
    public void TriggerWin()
    {
        if (CurrentState != GameState.Playing) return;
        TransitionTo(GameState.Win);
    }

    /// <summary>
    /// Transiciona o jogador para a próxima fase caso tenha ganhado
    /// </summary>
    /// <returns></returns>
    private IEnumerator HandleWin()
    {
        StopSimulation();
        hudController.ShowMessage($"Nível {_currentLevel} completo!");
        yield return new WaitForSeconds(1.5f);

        _currentLevel++;
        TransitionTo(GameState.Loading);
    }

    /// <summary>
    /// Gatilho para derrota do jogador
    /// </summary>
    public void TriggerGameOver()
    {
        if (CurrentState != GameState.Playing) return;
        TransitionTo(GameState.GameOver);
    }

    /// <summary>
    /// Para o jogo e avisa o jogador para reiniciar caso ele tenha perdido
    /// </summary>
    /// <returns></returns>
    private void HandleGameOver()
    {
        StopSimulation();
        hudController.ShowMessage("Game Over! Pressione R para reiniciar.");
        StartCoroutine(WaitForRestart());
    }

    /// <summary>
    /// Reinicia o jogo para a fase 1
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitForRestart()
    {
        while (!Input.GetKeyDown(KeyCode.R))
            yield return null;

        _currentLevel = startLevel;
        TransitionTo(GameState.Loading);
    }
    
    /// <summary>
    /// Para a fase/simulação
    /// </summary>
    private void StopSimulation()
    {
        _timerRunning = false;
        playerController.SetInputEnabled(false);
        predictionController.CancelAll();
        trafficController.StopSpawning();
        trafficController.ClearAllVehicles();
    }

    /// <summary>
    /// Retorna o nível atual
    /// </summary>
    /// <returns></returns>
    public int GetCurrentLevel() { 
        return _currentLevel;
    }
}