using System;
using System.Collections.Generic;
using UnityEngine;

// Ataque Safado (Raro): ataques fisicos destroem projeteis inimigos e tem a area aumentada.
[CreateAssetMenu(menuName = "Nethronia/Habilidades passivas/Forca/Ataque Safado")]
public class AtaqueSafado : DadosHabilidadePassiva
{
    [Header("Ataque Safado:")]
    [Tooltip("Aumento percentual da area (escala) dos ataques fisicos")]
    [SerializeField] private EscalaValor _aumentoPercentualArea = new EscalaValor(10f, 50f, 100);
    [SerializeField] private GameObject _particulaProjetilDestruido;

    public override IHabilidadePassiva CriarInstancia() => new Logica(this);

    private class Logica : HabilidadePassiva<AtaqueSafado>, IReageAtaqueLancado
    {
        public Logica(AtaqueSafado dados) : base(dados) { }

        public void AoLancarAtaque(Ataque ataque)
        {
            if (ataque._tipoDano != Tipo_Dano.Fisico || ataque.GetType() != typeof(Ataque)) return; // so golpes fisicos comuns

            ataque.transform.localScale *= 1f + _dados._aumentoPercentualArea.Avaliar(Nivel) / 100f;
            ataque.gameObject.AddComponent<DestruidorProjeteis>().Configurar(Dono, AoDestruirProjetil);
        }

        private void AoDestruirProjetil(Vector3 posicao)
        {
            InstanciarParticula(_dados._particulaProjetilDestruido, posicao, 1.5f);
        }
    }
}

// Colocado no golpe fisico: quebra os projeteis de outros donos que encostarem nele
public class DestruidorProjeteis : MonoBehaviour
{
    private Ser_Vivo _dono;
    private Action<Vector3> _aoDestruir;
    private Collider2D _colisor;
    private readonly List<Collider2D> _sobrepostos = new List<Collider2D>();
    private ContactFilter2D _filtro;

    public void Configurar(Ser_Vivo dono, Action<Vector3> aoDestruir)
    {
        _dono = dono;
        _aoDestruir = aoDestruir;
        _colisor = GetComponent<Collider2D>();
        _filtro = new ContactFilter2D { useTriggers = true };
    }

    private void FixedUpdate()
    {
        if (_colisor == null || !_colisor.enabled) return;

        _sobrepostos.Clear();
        _colisor.Overlap(_filtro, _sobrepostos);

        foreach (Collider2D colisor in _sobrepostos)
        {
            Projetil projetil = colisor.GetComponent<Projetil>();
            if (projetil == null || projetil._dono == _dono) continue;

            _aoDestruir?.Invoke(projetil.transform.position);
            projetil.Quebrar();
        }
    }
}
