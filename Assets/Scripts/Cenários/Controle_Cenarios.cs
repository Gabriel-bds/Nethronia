using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Controle_Cenarios : MonoBehaviour
{
    [SerializeField] int _fase;
    public int _qtdCenarios;
    public int _limiteCenarios;
    public int _cenariosPassados;
    [SerializeField] int _cenarioAtual;
    public List<int> _cenariosDisponiveis = new List<int>();
    private void Awake()
    {
        DontDestroyOnLoad(this);
        for (int n = 1; n <= _qtdCenarios; n++)
        {
            _cenariosDisponiveis.Add(n);
        }
        
        //SortearCenario();
        
    }
    public void SortearCenario()
    {
        if(_cenariosPassados >= _limiteCenarios)
        {
            SceneManager.LoadSceneAsync($"{_fase}.f", LoadSceneMode.Single);
        }
        else
        {
            int _indexNovo = Random.Range(0, _cenariosDisponiveis.Count);
            int _novaCenario = _cenariosDisponiveis[_indexNovo];
            //SceneManager.LoadSceneAsync($"{_fase}.{2}", LoadSceneMode.Single);
            _cenariosDisponiveis.Remove(_novaCenario);
            _cenarioAtual = _novaCenario;
            _cenariosPassados++;
        }
    }
}
