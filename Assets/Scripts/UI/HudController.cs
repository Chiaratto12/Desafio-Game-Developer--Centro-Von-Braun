using UnityEngine;
using TMPro;

public class HUDController : MonoBehaviour
{
    public static HUDController Instance { get; private set; }

    [Header("UI Elements")]
    public TMP_Text levelText;
    public TMP_Text weatherText;
    public TMP_Text densityText;
    public TMP_Text speedText;
    public TMP_Text timerText;
    public TMP_Text messageText;

    private void Awake() => Instance = this;

    /// <summary>
    /// Atualiza o HUD
    /// </summary>
    /// <param name="level">Level atual</param>
    /// <param name="status">Status atual</param>
    public void UpdateHUD(int level, Status status)
    {
        levelText.text = $"Nível: {level}";
        weatherText.text = $"Clima: {status.weather}";
        densityText.text = $"Densidade: {status.vehicleDensity:F2}";
        speedText.text = $"Velocidade: {status.averageSpeed:F0} km/h";
    }

    /// <summary>
    /// Mostra mensagem na HUD
    /// </summary>
    /// <param name="message">Mensagem a ser exibida</param>
    public void ShowMessage(string message)
    {
        messageText.text = message;
    }

    /// <summary>
    /// Atualiza o tempo restante
    /// </summary>
    /// <param name="seconds">Tempo em segundos</param>
    public void UpdateTimer(float seconds)
    {
        timerText.text = $"Tempo: {seconds:F1}s";
        timerText.color = seconds < 3f ? Color.red : Color.white;
    }
}