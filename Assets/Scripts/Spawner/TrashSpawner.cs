using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashSpawner : MonoBehaviour
{
    [SerializeField] GameObject _trashPrefab;
    [SerializeField] GameObject _trashSpawnPoint;
    [Tooltip("In Seconds")]
    [SerializeField] int _trashSpawnInterval = 75;
    [SerializeField] static float _trashMax = 15;
    [SerializeField] bool _spawnWhenStart = true;

    float _timerToSpawn = 0;

    private void Reset()
    {
        _trashPrefab = Resources.Load<GameObject>("Prefabs/Set_Trash");
    }

    private void Awake()
    {
        if (!_spawnWhenStart)
        {
            _timerToSpawn = _trashSpawnInterval;
        }
    }

    private void FixedUpdate()
    {
        _timerToSpawn -= Time.fixedDeltaTime;
        if (_timerToSpawn <= 0) 
        {
            Instantiate(_trashPrefab, _trashSpawnPoint.transform.position, Quaternion.identity);
            _timerToSpawn = _trashSpawnInterval;
        }
    }
}
