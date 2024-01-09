using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Controle_Cenarios : MonoBehaviour
{
    [SerializeField] int _fase;
    [SerializeField] int _qtdCenarios;
    [SerializeField] int _cenarioAtual;
    [SerializeField] List<int> _cenariosDisponiveis = new List<int>();
    private void Awake()
    {
        DontDestroyOnLoad(this);
        for (int n = 1; n <= _qtdCenarios; n++)
        {
            _cenariosDisponiveis.Add(n);
        }
    }

    public void SortearCenario()
    {
        int _novaCenario = _cenariosDisponiveis[Random.Range(1, _cenariosDisponiveis.Count)];
        SceneManager.LoadSceneAsync($"{_fase}.{_novaCenario}", LoadSceneMode.Single);
        _cenariosDisponiveis.Remove(_novaCenario);
        _cenarioAtual = _novaCenario;
    }
}
