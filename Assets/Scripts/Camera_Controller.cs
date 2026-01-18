using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;

public class Camera_Controller : MonoBehaviour
{
    [HideInInspector] public float intensidadeShakeCamera;
    [SerializeField] float maximoShake;
    private void Update()
    {
        OlharCamera();
    }

    public void OlharCamera()
    {
        GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineFramingTransposer>().m_ScreenX = 0.5f - Utilidades.LimitadorNumero(-0.4f, 0.4f, (Camera.main.ScreenToWorldPoint(Input.mousePosition).x - FindAnyObjectByType<Player>().transform.position.x) / 100);
        GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineFramingTransposer>().m_ScreenY = 0.5f + Utilidades.LimitadorNumero(-0.4f, 0.4f, (Camera.main.ScreenToWorldPoint(Input.mousePosition).y - FindAnyObjectByType<Player>().transform.position.y) / 100);
    }
    public void Tremer(float _porcentagemIntensidade)
    {
        intensidadeShakeCamera += _porcentagemIntensidade;
        if(intensidadeShakeCamera > maximoShake)
        {
            intensidadeShakeCamera = maximoShake;
        }
        GetComponent<CinemachineImpulseSource>().GenerateImpulseWithVelocity(new Vector2(intensidadeShakeCamera / 100, intensidadeShakeCamera / 100));
    }
}
