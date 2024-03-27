using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Controle_Cenarios : MonoBehaviour
{
    [SerializeField] int _fase;
    public int _qtdCenarios;
    [SerializeField] int _cenarioAtual;
    public List<int> _cenariosDisponiveis = new List<int>();
    private void Awake()
    {
        DontDestroyOnLoad(this);
        for (int n = 1; n <= _qtdCenarios; n++)
        {
            _cenariosDisponiveis.Add(n);
        }
        
        SortearCenario();
        
    }
    private void Start()
    {
        //SceneManager.LoadSceneAsync($"2.inicio", LoadSceneMode.Single);
    }

    public void SortearCenario()
    {
        int _indexNovo = Random.Range(0, _cenariosDisponiveis.Count);
        int _novaCenario = _cenariosDisponiveis[_indexNovo];
        SceneManager.LoadSceneAsync($"{_fase}.{_novaCenario}", LoadSceneMode.Single);
        _cenariosDisponiveis.Remove(_novaCenario);
        _cenarioAtual = _novaCenario;

    }
}
