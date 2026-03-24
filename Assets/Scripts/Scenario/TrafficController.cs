using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficController : MonoBehaviour
{
    public static TrafficController Instance { get; private set; }

    [Header("Config")]
    public GameObject vehiclePrefab;
    public float cellSize = 1f;
    public float referenceSpeed = 15f;

    public List<VehicleController> ActiveVehicles { get; private set; } = new();

    [System.Serializable]
    public class Lane
    {
        public Transform spawnPoint;
        public Transform despawnPoint;
        public Vector3 direction;
    }

    public Lane[] lanes;

    private float _spawnInterval;
    private float _vehicleSpeed;
    private Coroutine _spawnCoroutine;

    private void Awake() => Instance = this;

    private void Start()
    {
        Status status = new Status();
        status.vehicleDensity = 0.5f;
        status.weather = "sunny";
        status.averageSpeed = 60.0f;
        ApplyStatus(status);
    }

    public void ApplyStatus(Status status)
    {
        _spawnInterval = 1f / status.vehicleDensity;
        _vehicleSpeed = (status.averageSpeed / 100f) * referenceSpeed;

        if (_spawnCoroutine != null) StopCoroutine(_spawnCoroutine);
        _spawnCoroutine = StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            SpawnInRandomLane();
            yield return new WaitForSeconds(_spawnInterval);
        }
    }

    private void SpawnInRandomLane()
    {
        Lane lane = lanes[Random.Range(0, lanes.Length)];

        GameObject obj = Instantiate(vehiclePrefab, lane.spawnPoint.position, Quaternion.identity);
        VehicleController mover = obj.GetComponent<VehicleController>();
        mover.Initialize(_vehicleSpeed, lane.despawnPoint.position, lane.direction, cellSize);

        ActiveVehicles.Add(mover);
    }

    public void UnregisterVehicle(VehicleController vehicle)
    {
        ActiveVehicles.Remove(vehicle);
    }
}