using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void DetectMagneticHandler(Magnetic detected);

public class Grabber : MonoBehaviour
{
    [SerializeField] Collider _mainCollider;
    [SerializeField] Magnet _magnet;
    [Space]

    [Header("Zona sensivel")]
    [SerializeField] float _sensorRadius = 4.5f;
    [SerializeField] Color _debbugSensorZoneColor = Color.red;
    [Space]

    [Header("Configurações da zona magnetica")]
    [SerializeField] float _radius = 3f;
    [SerializeField] Color _debbugMagnetZoneColor = Color.white;
    [SerializeField] LayerMask _whereIsMagnetics = (1 << 0);

    public event DetectMagneticHandler OnDetectMagnetic;

    List<Magnetic> _detected = new List<Magnetic>();

    private void Reset()
    {
        _mainCollider = GetComponent<Collider>();
        _magnet = GetComponent<Magnet>();
    }

    private void FixedUpdate()
    {
        Collider[] inSensor = Physics.OverlapSphere(_mainCollider.bounds.center, _sensorRadius, _whereIsMagnetics);
        if (inSensor.Length > 0)
        {
            foreach (Collider col in inSensor)
            {
                Magnetic magnet = _magnet.GetMagnetic(col.transform.GetInstanceID());
                if (_detected.Contains(magnet))
                {
                    continue;
                }
                OnDetectMagnetic?.Invoke(magnet);
                _detected.Add(magnet);
            }
        }
        else
        {
            _detected.RemoveRange(0, _detected.Count);
        }

        Collider[] inSide = Physics.OverlapSphere(_mainCollider.bounds.center, _radius, _whereIsMagnetics);
        if (inSide.Length > 0)
        {
            foreach (Collider col in inSide)
            {
                float distanceFactor = 1 - (Vector3.Distance(col.transform.position, _mainCollider.bounds.center) / _radius);
                distanceFactor = Mathf.Abs(distanceFactor);
                _magnet.TryAttractMagnetic(col.transform.GetInstanceID(), distanceFactor);
            }
        }
    }



    private void OnDrawGizmos()
    {
        Gizmos.color = _debbugMagnetZoneColor;
        Gizmos.DrawWireSphere(_mainCollider.bounds.center, _radius);

        Gizmos.color = _debbugSensorZoneColor;
        Gizmos.DrawWireSphere(_mainCollider.bounds.center, _sensorRadius);
    }
}
