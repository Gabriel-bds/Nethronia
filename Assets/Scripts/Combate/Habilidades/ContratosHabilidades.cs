// Contratos das habilidades passivas.
// Toda passiva assina IHabilidadePassiva e, alem disso, so os contratos dos momentos em que ela age.
// O GerenciadorHabilidades so chama cada passiva nos momentos que ela assinou.

public interface IHabilidadePassiva
{
    DadosHabilidadePassiva Dados { get; }
    void Ativar(ContextoHabilidade contexto);
    void Desativar();
}

// Altera o dano que o dono causa (ex.: Ganho de Massa, Opressao Glacial)
public interface IModificaDanoCausado
{
    float ModificarDanoCausado(InformacaoDano informacao);
}

// Altera o dano que o dono recebe (ex.: Suspiro, Rocha)
public interface IModificaDanoRecebido
{
    float ModificarDanoRecebido(InformacaoDano informacao);
}

// Reage depois que o dono causou dano (ex.: Vampirismo, Sobrecarga)
public interface IReageDanoCausado
{
    void AoCausarDano(InformacaoDano informacao);
}

// Reage depois que o dono recebeu dano (ex.: Descarga Reflexiva, Sangue Mortal)
public interface IReageDanoRecebido
{
    void AoReceberDano(InformacaoDano informacao);
}

// Reage quando o dono mata alguem (ex.: Ceifador, Morte Congelante)
public interface IReageAbate
{
    void AoMatar(Ser_Vivo vitima);
}

// Chamado quando a vida do dono chega a zero. Retorna true se impediu a morte (ex.: Reserva Biologica)
public interface IReageMorteDono
{
    bool AoDonoMorrer();
}

// Reage quando o dono lanca um ataque (ex.: Ataque Safado, Concentracao Infernal)
public interface IReageAtaqueLancado
{
    void AoLancarAtaque(Ataque ataque);
}

// Decide se um ataque do dono pode ser usado de novo sem entrar em recarga (ex.: Ataque em Sequencia)
public interface IControlaRecarga
{
    bool UsarSemRecarregar(Ataque ataque);
}

// Reage quando o dono rebate um projetil (ex.: Ricochete Perfeito)
public interface IReageProjetilRefletido
{
    void AoRefletirProjetil(Projetil projetil);
}

// Precisa rodar a cada quadro (ex.: Rocha, Concentracao Infernal)
public interface IAtualizavel
{
    void Atualizar(float deltaTempo);
}
