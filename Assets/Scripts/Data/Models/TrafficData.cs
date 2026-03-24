[System.Serializable]
public class TrafficResponse
{
    public Status current_status;
    public PredictedStatusEntry[] predicted_status;
}

[System.Serializable]
public class PredictedStatusEntry
{
    public int estimated_time;
    public Status predictions;
}

[System.Serializable]
public class Status
{
    public float vehicleDensity;
    public float averageSpeed;
    public string weather;          
}