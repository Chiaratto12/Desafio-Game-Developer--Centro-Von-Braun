using UnityEngine;

public class WeatherController : MonoBehaviour
{
    public static WeatherController Instance { get; private set; }

    [Header("References")]
    public PlayerController playerController; 

    public string CurrentWeather { get; private set; } = "sunny";

    private void Awake() => Instance = this;

    /// <summary>
    /// Aplica o clima na fase e no jogador
    /// </summary>
    /// <param name="weather">Clima selecionado</param>
    public void ApplyWeather(string weather)
    {
        if (CurrentWeather == weather) return;

        CurrentWeather = weather;

        ApplyPlayerMultiplier(weather);

        Debug.Log($"[WeatherController] Clima alterado para: {weather}");
    }

    /// <summary>
    /// Aplica o multiplicador de velocidade no jogador baseado no clima selecinado
    /// </summary>
    /// <param name="weather">Clima selecionado</param>
    private void ApplyPlayerMultiplier(string weather)
    {
        playerController.SetWeatherMultiplier(weather);
    }

    /// <summary>
    /// Retorna o multplicador atual do clima selecionado
    /// </summary>
    /// <returns></returns>
    public float GetCurrentMultiplier()
    {
        return CurrentWeather switch
        {
            "sunny" => 1.0f,
            "clouded" => 0.8f,
            "foggy" => 0.8f,
            "light rain" => 0.6f,
            "heavy rain" => 0.4f,
            _ => 1.0f
        };
    }
}