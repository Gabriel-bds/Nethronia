using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering.Universal;

// Ferramentas para configurar no editor as habilidades (assets das passivas, gerenciador no Player e Calor Crescente).
// Tudo pode ser refeito: os menus pulam o que ja existe.
public static class ConfiguradorHabilidades
{
    const string PastaAtaquesPlayer = "Assets/Resources/Prefabs/Combate/Ataques/Player";
    const string CaminhoPrefabCalorCrescente = PastaAtaquesPlayer + "/Fogo/Calor crescente/CalorCrescente.prefab";
    const string PastaAnimacoesPlayer = "Assets/Artes/Entidades/Seres Vivos/Players/Player 1/Sprites/novo player/partes/Animacoes";
    const string CaminhoControllerPlayer = PastaAnimacoesPlayer + "/player.controller";
    const string CaminhoAnimacaoBase = PastaAnimacoesPlayer + "/Ataques/Vitalidade/ExplosaoVital.anim";
    const string CaminhoAnimacaoCalorCrescente = PastaAnimacoesPlayer + "/Ataques/Fogo/CalorCrescente.anim";
    const string NomeEstadoBase = "ExplosaoVital";
    const string NomeEstadoCalorCrescente = "CalorCrescente";

    const string PastaAssetsPassivas = "Assets/Dados/Habilidades/Passivas";

    // Passivas do PPT "Game Design Habilidades": tipo, nome, raridade, poder e descricao
    static readonly (Type tipo, string nome, Raridade_carta raridade, Tipo_Poder poder, string descricao)[] Passivas =
    {
        (typeof(Arremesso), "Arremesso", Raridade_carta.Epico, Tipo_Poder.Forca,
            "Quando um inimigo e repelido, ao atingir outro inimigo, ele recebera parte do dano do inimigo que foi repelido. Acumula."),
        (typeof(AtaqueSafado), "Ataque Safado", Raridade_carta.Raro, Tipo_Poder.Forca,
            "Ataques fisicos podem destruir projeteis e tem a area aumentada."),
        (typeof(AtaqueEmSequencia), "Ataque em Sequencia", Raridade_carta.Comum, Tipo_Poder.Forca,
            "Permite executar um ataque X vezes antes de precisar recarregar."),
        (typeof(Suspiro), "Suspiro", Raridade_carta.Epico, Tipo_Poder.Resistencia,
            "Com menos de 50% de vida, recebe menos dano quanto menos vida tiver, com reducao de ate X% com a vida beirando 0."),
        (typeof(Rocha), "Rocha", Raridade_carta.Raro, Tipo_Poder.Resistencia,
            "Ficar parado por X segundos cria um escudo que reduz o dano de X ataques."),
        (typeof(RicochetePerfeito), "Ricochete Perfeito", Raridade_carta.Comum, Tipo_Poder.Resistencia,
            "Projeteis ricocheteados atravessam os inimigos."),
        (typeof(RouboVelocidade), "Roubo de Velocidade", Raridade_carta.Comum, Tipo_Poder.Velocidade,
            "Ao acumular nos inimigos, eles sofrem um efeito de desaceleracao."),
        (typeof(GanhoMassa), "Ganho de Massa", Raridade_carta.Epico, Tipo_Poder.Velocidade,
            "Bonus de dano fisico em movimento, relativo a velocidade do jogador."),
        (typeof(GanhoTempo), "Ganho de Tempo", Raridade_carta.Raro, Tipo_Poder.Velocidade,
            "Ao atacar inimigos, todas as habilidades recarregam mais rapido."),
        (typeof(Ceifador), "Ceifador", Raridade_carta.Raro, Tipo_Poder.Vitalidade,
            "Ganha X% da vida de cada inimigo morto."),
        (typeof(Vampirismo), "Vampirismo", Raridade_carta.Epico, Tipo_Poder.Vitalidade,
            "A cada dano infligido a um inimigo, recebe X% como vida."),
        (typeof(ReservaBiologica), "Reserva Biologica", Raridade_carta.Lendario, Tipo_Poder.Vitalidade,
            "Permite ressuscitar de X mortes com X% de vida."),
        (typeof(ConcentracaoInfernal), "Concentracao Infernal", Raridade_carta.Lendario, Tipo_Poder.Fogo,
            "O dano da rajada de fogo cresce a cada tique enquanto o jogador esta parado; ao se mover, cai na mesma proporcao."),
        (typeof(MorteCongelante), "Morte Congelante", Raridade_carta.Epico, Tipo_Poder.Gelo,
            "Quando um inimigo congelado morre, cria uma area que causa dano de gelo e acumulo de congelamento."),
        (typeof(OpressaoGlacial), "Opressao Glacial", Raridade_carta.Lendario, Tipo_Poder.Gelo,
            "Para cada inimigo congelado, o dano de gelo e aumentado em X%."),
        (typeof(DescargaReflexiva), "Descarga Reflexiva", Raridade_carta.Epico, Tipo_Poder.Eletricidade,
            "Ao ser atingido, ha X% de chance de criar uma onda de choque."),
        (typeof(Sobrecarga), "Sobrecarga", Raridade_carta.Lendario, Tipo_Poder.Eletricidade,
            "Atingir inimigos com habilidades eletricas gera cargas; no limite, o proximo ataque eletrico e amplificado."),
        (typeof(SangueMortal), "Sangue Mortal", Raridade_carta.Epico, Tipo_Poder.Veneno,
            "As pocas de sangue do jogador sao venenosas: inimigos nelas levam dano de veneno e acumulo."),
        (typeof(MutacaoToxica), "Mutacao Toxica", Raridade_carta.Lendario, Tipo_Poder.Veneno,
            "Inimigos envenenados sofrem X% a mais de dano e infligem X% a menos de dano (acumula)."),
    };

    static readonly string[] ProjeteisQueAtravessam =
    {
        PastaAtaquesPlayer + "/Fogo/Bola de fogo/Bola de fogo player.prefab",
        PastaAtaquesPlayer + "/Bola de gelo.prefab",
        PastaAtaquesPlayer + "/Bola de eletricidade.prefab",
        PastaAtaquesPlayer + "/Bola de veneno.prefab",
    };

    [MenuItem("Nethronia/Habilidades/Configurar tudo")]
    static void ConfigurarTudo()
    {
        CriarAssetsPassivas();
        ConfigurarGerenciadorNoPlayer();
        MarcarProjeteisQueAtravessam();
        CriarCalorCrescente();
    }

    [MenuItem("Nethronia/Habilidades/Criar assets das passivas")]
    static void CriarAssetsPassivas()
    {
        List<string> criados = new List<string>();
        foreach (var passiva in Passivas)
        {
            string caminho = CaminhoAssetPassiva(passiva.poder, passiva.nome);
            if (AssetDatabase.LoadAssetAtPath<DadosHabilidadePassiva>(caminho) != null) continue;

            Directory.CreateDirectory(Path.GetDirectoryName(caminho));
            DadosHabilidadePassiva dados = (DadosHabilidadePassiva)ScriptableObject.CreateInstance(passiva.tipo);
            dados.PreencherIdentificacao(passiva.nome, passiva.descricao, passiva.raridade, passiva.poder);
            AssetDatabase.CreateAsset(dados, caminho);
            criados.Add(passiva.nome);
        }
        AssetDatabase.SaveAssets();
        Debug.Log(criados.Count > 0
            ? $"[Habilidades] Assets criados em {PastaAssetsPassivas}: {string.Join(", ", criados)}"
            : "[Habilidades] Todos os assets de passivas ja existem.");
    }

    [MenuItem("Nethronia/Habilidades/Configurar gerenciador de habilidades no Player")]
    static void ConfigurarGerenciadorNoPlayer()
    {
        Player player = UnityEngine.Object.FindAnyObjectByType<Player>(FindObjectsInactive.Include);
        if (player == null)
        {
            Debug.LogWarning("[Habilidades] Nenhum Player na cena aberta.");
            return;
        }

        // A lista comeca vazia: as habilidades entram quando forem desbloqueadas (cartas/lobby).
        // Para testar, arraste os assets de Assets/Dados/Habilidades/Passivas para a lista.
        if (player.GetComponent<GerenciadorHabilidades>() == null)
        {
            Undo.AddComponent<GerenciadorHabilidades>(player.gameObject);
            EditorSceneManager.MarkSceneDirty(player.gameObject.scene);
        }
        Debug.Log($"[Habilidades] {player.name} tem o GerenciadorHabilidades.", player);
    }

    static string CaminhoAssetPassiva(Tipo_Poder poder, string nome)
    {
        return $"{PastaAssetsPassivas}/{poder}/{nome}.asset";
    }

    [MenuItem("Nethronia/Habilidades/Marcar projeteis elementais para atravessar")]
    static void MarcarProjeteisQueAtravessam()
    {
        foreach (string caminho in ProjeteisQueAtravessam)
        {
            if (!File.Exists(caminho))
            {
                Debug.LogWarning($"[Habilidades] Projetil nao encontrado: {caminho}");
                continue;
            }

            GameObject raiz = PrefabUtility.LoadPrefabContents(caminho);
            Projetil projetil = raiz.GetComponentInChildren<Projetil>(true);
            if (projetil != null)
            {
                SerializedObject serializado = new SerializedObject(projetil);
                serializado.FindProperty("_atravessarComDanoRestante").boolValue = true;
                serializado.ApplyModifiedPropertiesWithoutUndo();
                PrefabUtility.SaveAsPrefabAsset(raiz, caminho);
            }
            PrefabUtility.UnloadPrefabContents(raiz);
        }
        Debug.Log("[Habilidades] Bola de fogo, gelo, eletricidade e veneno agora atravessam com o dano restante.");
    }

    [MenuItem("Nethronia/Habilidades/Criar Calor Crescente (prefab, animacao e estado)")]
    static void CriarCalorCrescente()
    {
        int idAtaque = CriarPrefabCalorCrescente();
        AnimationClip animacao = CriarAnimacaoCalorCrescente(idAtaque);
        if (animacao != null)
            CriarEstadoNoController(animacao, idAtaque);
        AssetDatabase.SaveAssets();
    }

    static int CriarPrefabCalorCrescente()
    {
        GameObject prefabExistente = AssetDatabase.LoadAssetAtPath<GameObject>(CaminhoPrefabCalorCrescente);
        if (prefabExistente != null)
        {
            Debug.Log("[Habilidades] Prefab do Calor Crescente ja existe.");
            return prefabExistente.GetComponent<Ataque>()._idAtaque;
        }

        int idAtaque = ProximoIdAtaqueLivre();
        Directory.CreateDirectory(Path.GetDirectoryName(CaminhoPrefabCalorCrescente));

        GameObject raiz = new GameObject("CalorCrescente");
        CalorCrescente calor = raiz.AddComponent<CalorCrescente>();
        raiz.AddComponent<EfeitoIncinerar>();

        SerializedObject serializado = new SerializedObject(calor);
        serializado.FindProperty("_idAtaque").intValue = idAtaque;
        serializado.FindProperty("_tipoDano").enumValueIndex = (int)Tipo_Dano.Fogo;
        serializado.FindProperty("_dano").floatValue = 20f;
        serializado.FindProperty("_repulsao").floatValue = 0f;
        serializado.FindProperty("_tempoRecargaTotal").floatValue = 20f;
        serializado.FindProperty("_alvos").intValue = LayerMask.GetMask("Inimigo");
        serializado.FindProperty("_corDano").colorValue = new Color32(255, 104, 0, 255);
        serializado.FindProperty("nivelMaximoMagnitudeVisual").intValue = 100;
        serializado.FindProperty("_particulaTique").objectReferenceValue =
            Resources.Load<GameObject>("Prefabs/Combate/Particulas/Poderes/Incineracao");
        serializado.ApplyModifiedPropertiesWithoutUndo();

        CriarParticulasCalor(raiz);

        PrefabUtility.SaveAsPrefabAsset(raiz, CaminhoPrefabCalorCrescente);
        UnityEngine.Object.DestroyImmediate(raiz);
        Debug.Log($"[Habilidades] Prefab do Calor Crescente criado em {CaminhoPrefabCalorCrescente} (id de ataque {idAtaque}).");
        return idAtaque;
    }

    // Brasas subindo em volta do dono + uma luz quente. So particula, nenhum sprite novo.
    static void CriarParticulasCalor(GameObject raiz)
    {
        GameObject objetoBrasas = new GameObject("Brasas");
        objetoBrasas.transform.SetParent(raiz.transform, false);

        ParticleSystem brasas = objetoBrasas.AddComponent<ParticleSystem>();
        brasas.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        var principal = brasas.main;
        principal.loop = true;
        principal.duration = 5f;
        principal.startLifetime = new ParticleSystem.MinMaxCurve(1.5f, 3f);
        principal.startSpeed = new ParticleSystem.MinMaxCurve(0.3f, 1.2f);
        principal.startSize = new ParticleSystem.MinMaxCurve(0.05f, 0.2f);
        principal.startColor = new ParticleSystem.MinMaxGradient(new Color(1f, 0.55f, 0.1f), new Color(1f, 0.2f, 0f));
        principal.gravityModifier = -0.05f;
        principal.simulationSpace = ParticleSystemSimulationSpace.World;
        principal.maxParticles = 500;

        var emissao = brasas.emission;
        emissao.rateOverTime = 60f;

        var forma = brasas.shape;
        forma.shapeType = ParticleSystemShapeType.Circle;
        forma.radius = 8f;
        forma.rotation = Vector3.zero;

        var corAoLongoDaVida = brasas.colorOverLifetime;
        corAoLongoDaVida.enabled = true;
        Gradient gradiente = new Gradient();
        gradiente.SetKeys(
            new[] { new GradientColorKey(new Color(1f, 0.7f, 0.2f), 0f), new GradientColorKey(new Color(0.8f, 0.1f, 0f), 1f) },
            new[] { new GradientAlphaKey(0f, 0f), new GradientAlphaKey(1f, 0.2f), new GradientAlphaKey(0f, 1f) });
        corAoLongoDaVida.color = gradiente;

        var ruido = brasas.noise;
        ruido.enabled = true;
        ruido.strength = 0.4f;
        ruido.frequency = 0.6f;

        ParticleSystemRenderer renderizador = objetoBrasas.GetComponent<ParticleSystemRenderer>();
        renderizador.sharedMaterial = MaterialParticulaExistente();
        renderizador.sortingOrder = 50;

        GameObject objetoLuz = new GameObject("Luz calor");
        objetoLuz.transform.SetParent(raiz.transform, false);
        Light2D luz = objetoLuz.AddComponent<Light2D>();
        luz.lightType = Light2D.LightType.Point;
        luz.color = new Color(1f, 0.45f, 0.1f);
        luz.intensity = 0.6f;
        luz.pointLightInnerRadius = 2f;
        luz.pointLightOuterRadius = 10f;
    }

    static Material MaterialParticulaExistente()
    {
        foreach (string nome in new[] { "Incineracao", "Explosao inicinerar", "Flocos", "Envenenado" })
        {
            GameObject particula = Resources.Load<GameObject>("Prefabs/Combate/Particulas/Poderes/" + nome);
            if (particula == null) continue;
            ParticleSystemRenderer renderizador = particula.GetComponentInChildren<ParticleSystemRenderer>(true);
            if (renderizador != null && renderizador.sharedMaterial != null)
                return renderizador.sharedMaterial;
        }
        return AssetDatabase.GetBuiltinExtraResource<Material>("Default-ParticleSystem.mat");
    }

    static int ProximoIdAtaqueLivre()
    {
        int maiorId = 0;
        foreach (string guid in AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/Resources/Prefabs/Combate/Ataques" }))
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(AssetDatabase.GUIDToAssetPath(guid));
            Ataque ataque = prefab != null ? prefab.GetComponent<Ataque>() : null;
            if (ataque != null) maiorId = Mathf.Max(maiorId, ataque._idAtaque);
        }
        return maiorId + 1;
    }

    // Copia a animacao da Explosao Vital (mesmo gesto do corpo) e troca o id do evento InstanciarAtaque
    static AnimationClip CriarAnimacaoCalorCrescente(int idAtaque)
    {
        AnimationClip existente = AssetDatabase.LoadAssetAtPath<AnimationClip>(CaminhoAnimacaoCalorCrescente);
        if (existente != null) return existente;

        Directory.CreateDirectory(Path.GetDirectoryName(CaminhoAnimacaoCalorCrescente));
        if (!AssetDatabase.CopyAsset(CaminhoAnimacaoBase, CaminhoAnimacaoCalorCrescente))
        {
            Debug.LogError($"[Habilidades] Nao foi possivel copiar {CaminhoAnimacaoBase}.");
            return null;
        }

        AnimationClip animacao = AssetDatabase.LoadAssetAtPath<AnimationClip>(CaminhoAnimacaoCalorCrescente);
        AnimationEvent[] eventos = AnimationUtility.GetAnimationEvents(animacao);
        foreach (AnimationEvent evento in eventos)
            if (evento.functionName == "InstanciarAtaque")
                evento.intParameter = idAtaque;
        AnimationUtility.SetAnimationEvents(animacao, eventos);
        EditorUtility.SetDirty(animacao);

        Debug.Log($"[Habilidades] Animacao criada em {CaminhoAnimacaoCalorCrescente} (copia da Explosao Vital, ajuste a vontade).");
        return animacao;
    }

    // Cria o estado CalorCrescente com as mesmas transicoes/comportamentos do estado da Explosao Vital,
    // trocando a condicao "Ataque == id da explosao" pelo id do Calor Crescente.
    static void CriarEstadoNoController(AnimationClip animacao, int idAtaque)
    {
        AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(CaminhoControllerPlayer);
        if (controller == null)
        {
            Debug.LogError($"[Habilidades] Controller nao encontrado: {CaminhoControllerPlayer}");
            return;
        }

        foreach (AnimatorControllerLayer camada in controller.layers)
        {
            AnimatorStateMachine maquina = camada.stateMachine;
            AnimatorState estadoBase = BuscarEstado(maquina, NomeEstadoBase, out AnimatorStateMachine maquinaDoEstado);
            if (estadoBase == null) continue;

            if (BuscarEstado(maquina, NomeEstadoCalorCrescente, out _) != null)
            {
                Debug.Log("[Habilidades] O estado CalorCrescente ja existe no controller.");
                return;
            }

            int idExplosao = IdAtaqueDaAnimacao(estadoBase.motion as AnimationClip);
            Vector3 posicao = PosicaoDoEstado(maquinaDoEstado, estadoBase) + new Vector3(0, 70, 0);
            AnimatorState novoEstado = maquinaDoEstado.AddState(NomeEstadoCalorCrescente, posicao);
            novoEstado.motion = animacao;
            novoEstado.speed = estadoBase.speed;
            novoEstado.writeDefaultValues = estadoBase.writeDefaultValues;
            novoEstado.tag = estadoBase.tag;

            foreach (StateMachineBehaviour comportamento in estadoBase.behaviours)
            {
                StateMachineBehaviour copia = novoEstado.AddStateMachineBehaviour(comportamento.GetType());
                EditorUtility.CopySerialized(comportamento, copia);
            }

            foreach (AnimatorStateTransition transicao in estadoBase.transitions)
            {
                AnimatorStateTransition copia = transicao.isExit
                    ? novoEstado.AddExitTransition()
                    : transicao.destinationState != null
                        ? novoEstado.AddTransition(transicao.destinationState)
                        : novoEstado.AddTransition(transicao.destinationStateMachine);
                CopiarTransicao(transicao, copia, idExplosao, idAtaque);
            }

            foreach (AnimatorStateTransition transicao in maquina.anyStateTransitions.Where(t => t.destinationState == estadoBase).ToArray())
                CopiarTransicao(transicao, maquina.AddAnyStateTransition(novoEstado), idExplosao, idAtaque);

            foreach (ChildAnimatorState filho in TodosEstados(maquina))
                foreach (AnimatorStateTransition transicao in filho.state.transitions.Where(t => t.destinationState == estadoBase).ToArray())
                    CopiarTransicao(transicao, filho.state.AddTransition(novoEstado), idExplosao, idAtaque);

            EditorUtility.SetDirty(controller);
            Debug.Log($"[Habilidades] Estado CalorCrescente criado na camada '{camada.name}' (Ataque == {idAtaque}).");
            return;
        }

        Debug.LogWarning($"[Habilidades] Estado {NomeEstadoBase} nao encontrado no controller; crie o estado do Calor Crescente manualmente.");
    }

    static void CopiarTransicao(AnimatorStateTransition origem, AnimatorStateTransition destino, int idAntigo, int idNovo)
    {
        destino.hasExitTime = origem.hasExitTime;
        destino.exitTime = origem.exitTime;
        destino.hasFixedDuration = origem.hasFixedDuration;
        destino.duration = origem.duration;
        destino.offset = origem.offset;
        destino.interruptionSource = origem.interruptionSource;
        destino.orderedInterruption = origem.orderedInterruption;
        destino.canTransitionToSelf = origem.canTransitionToSelf;

        foreach (AnimatorCondition condicao in origem.conditions)
        {
            float limiar = condicao.parameter == "Ataque" && Mathf.Approximately(condicao.threshold, idAntigo) ? idNovo : condicao.threshold;
            destino.AddCondition(condicao.mode, limiar, condicao.parameter);
        }
    }

    static int IdAtaqueDaAnimacao(AnimationClip animacao)
    {
        if (animacao == null) return -1;
        foreach (AnimationEvent evento in AnimationUtility.GetAnimationEvents(animacao))
            if (evento.functionName == "InstanciarAtaque")
                return evento.intParameter;
        return -1;
    }

    static AnimatorState BuscarEstado(AnimatorStateMachine maquina, string nome, out AnimatorStateMachine maquinaDoEstado)
    {
        foreach (ChildAnimatorState filho in maquina.states)
        {
            if (filho.state.name == nome)
            {
                maquinaDoEstado = maquina;
                return filho.state;
            }
        }
        foreach (ChildAnimatorStateMachine subMaquina in maquina.stateMachines)
        {
            AnimatorState encontrado = BuscarEstado(subMaquina.stateMachine, nome, out maquinaDoEstado);
            if (encontrado != null) return encontrado;
        }
        maquinaDoEstado = null;
        return null;
    }

    static IEnumerable<ChildAnimatorState> TodosEstados(AnimatorStateMachine maquina)
    {
        foreach (ChildAnimatorState filho in maquina.states)
            yield return filho;
        foreach (ChildAnimatorStateMachine subMaquina in maquina.stateMachines)
            foreach (ChildAnimatorState filho in TodosEstados(subMaquina.stateMachine))
                yield return filho;
    }

    static Vector3 PosicaoDoEstado(AnimatorStateMachine maquina, AnimatorState estado)
    {
        foreach (ChildAnimatorState filho in maquina.states)
            if (filho.state == estado) return filho.position;
        return Vector3.zero;
    }
}
