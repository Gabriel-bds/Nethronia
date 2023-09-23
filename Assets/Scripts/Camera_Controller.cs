using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class Camera_Controller : MonoBehaviour
{
    public void Tremer(float _porcentagemIntensidade)
    {
        if(_porcentagemIntensidade > 100)
        {
            _porcentagemIntensidade = 100;
        }
        GetComponent<CinemachineImpulseSource>().GenerateImpulseWithVelocity(new Vector2(_porcentagemIntensidade / 100 * 2f, _porcentagemIntensidade / 100 * 2f));
    }
}
