using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;

public class Camera_Controller : MonoBehaviour
{
    private void Start()
    {
    }
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
        if(_porcentagemIntensidade > 100)
        {
            _porcentagemIntensidade = 100;
        }
        GetComponent<CinemachineImpulseSource>().GenerateImpulseWithVelocity(new Vector2(_porcentagemIntensidade / 100 * 2f, _porcentagemIntensidade / 100 * 2f));
    }
}
