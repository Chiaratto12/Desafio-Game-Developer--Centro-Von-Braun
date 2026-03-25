using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PredictionController : MonoBehaviour
{
    private TrafficController _trafficController;
    private WeatherController _weatherController;
    private HUDController _hudController;
    private List<Coroutine> _scheduledCoroutines = new();

    /// <summary>
    /// Inicializa o controller
    /// </summary>
    /// <param name="tc">Controller do tráfico</param>
    /// <param name="wc">Controller do clima</param>
    public void Initialize(TrafficController tc, WeatherController wc, HUDController hud)
    {
        _trafficController = tc;
        _weatherController = wc;
        _hudController = hud;
    }

    /// <summary>
    /// Agenda todas as predições recebidas da API. estimated_time está em milissegundos.
    /// </summary>
    /// <param name="predictions">Lista de predições</param>
    public void SchedulePredictions(PredictedStatusEntry[] predictions)
    {
        CancelAll();

        foreach (var entry in predictions)
        {
            float delayInSeconds = entry.estimated_time / 1000f;
            var coroutine = StartCoroutine(ApplyPredictionAfterDelay(entry.predictions, delayInSeconds));
            _scheduledCoroutines.Add(coroutine);
        }
    }

    /// <summary>
    /// Aplica predições após um intervalo de tempo
    /// </summary>
    /// <param name="status">Status que será aplicado ao trânsito</param>
    /// <param name="delaySeconds">Intervalo em segundos</param>
    /// <returns></returns>
    private IEnumerator ApplyPredictionAfterDelay(Status status, float delaySeconds)
    {
        yield return new WaitForSeconds(delaySeconds);

        Debug.Log($"[Scheduler] Aplicando predição: clima={status.weather}, densidade={status.vehicleDensity}");
        _trafficController.ApplyStatus(status);
        _weatherController.ApplyWeather(status.weather);
        _hudController.UpdateHUD(GameManager.Instance.GetCurrentLevel(), status);
    }

    /// <summary>
    /// Cancela todas as predições
    /// </summary>
    public void CancelAll()
    {
        foreach (var c in _scheduledCoroutines)
            if (c != null) StopCoroutine(c);
        _scheduledCoroutines.Clear();
    }
}