using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.Threading;

public class CinemachineMovimientoCamara : MonoBehaviour
{
    public static CinemachineMovimientoCamara Instance;

    private CinemachineVirtualCamera cinemachineVirtualCamera;
    
    private CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin;
    
    private float tiempoMovimiento;
    
    private float tiemposMovimientoTotal;
    
    private float intensidadInicial;

    private void Awake()
    {
        cinemachineVirtualCamera = GetComponent<CinemachineVirtualCamera>();
        cinemachineBasicMultiChannelPerlin = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    public void MoverCamara(float intensidad, float frecuencia, float tiempo)
    {
        Instance = this;
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensidad;
        cinemachineBasicMultiChannelPerlin.m_FrequencyGain = frecuencia;
        intensidadInicial = intensidad;
        tiemposMovimientoTotal = tiempo;
        tiempoMovimiento = tiempo;
    }

    private void Update() 
    {
        if(tiempoMovimiento > 0) {
            tiempoMovimiento -= Time.deltaTime;
            cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = Mathf.Lerp(intensidadInicial, 0, 1 - (tiempoMovimiento / tiemposMovimientoTotal));
        }
    }
}
