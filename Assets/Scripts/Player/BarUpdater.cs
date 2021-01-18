using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarUpdater : MonoBehaviour
{
    Roy _roy;
    [SerializeField] Image _royEnergy;
    [SerializeField] Image _royFuel;
    Klunk _klunk;
    [SerializeField] Image _klunkFuel;
    [SerializeField] Image _klunkEnergy;

    private void Awake()
    {
        _roy = FindObjectOfType<Roy>();
        _klunk = FindObjectOfType<Klunk>();

        _roy.OnCurrentEnergyChange += _roy_OnCurrentEnergyChange;
        _roy.OnCurrentFuelChange += _roy_OnCurrentFuelChange;

        _klunk.OnCurrentEnergyChange += _klunk_OnCurrentEnergyChange;
        _klunk.OnCurrentFuelChange += _klunk_OnCurrentFuelChange;
    }

    private void _klunk_OnCurrentFuelChange(float percent)
    {
        _klunkFuel.fillAmount = percent;
    }

    private void _klunk_OnCurrentEnergyChange(float percent)
    {
        _klunkEnergy.fillAmount = percent;
    }

    private void _roy_OnCurrentFuelChange(float percent)
    {
        _royFuel.fillAmount = percent;
    }

    private void _roy_OnCurrentEnergyChange(float percent)
    {
        _royEnergy.fillAmount = percent;        
    }

    private void OnDestroy()
    {
        _roy.OnCurrentEnergyChange -= _roy_OnCurrentEnergyChange;
        _roy.OnCurrentFuelChange -= _roy_OnCurrentFuelChange;
    }
}
