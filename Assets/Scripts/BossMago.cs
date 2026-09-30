using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public class ConfigFaseMago
{
    public string Nome = "Fase";
    [Header("Ritmo")]
    public float TempoEntreAtaques = 3.2f;
    public float TeleporteMin = 8f;                
    public float TeleporteMax = 11f;
    [Range(0f, 1f)] public float ChanceCombo = 0f;   
    public bool EmboscadaAposTeleporte = false;      
    [Header("Ataque 1 - Orbes Arcanos")]
    public int OrbesQuantidade = 3;
    public float OrbesVelocidade = 7f;
    public float OrbesTempoConjuracao = 0.9f;       
    public float OrbesIntervaloDisparo = 0.35f;
    public float OrbesDispersaoGraus = 0f;           
    [Header("Ataque 2 - Chuva de Meteoros")]
    public int MeteorosQuantidade = 3;
    public float MeteorosTempoAviso = 1.3f;          
    public float MeteorosRaio = 1.6f;
    public float MeteorosIntervalo = 0.35f;
    [Header("Ataque 3 - Onda de Choque")]
    public int OndasQuantidade = 1;
    public bool OndaDosDoisLados = false;
    public float OndaVelocidade = 8f;
    public float OndaIntervalo = 0.9f;               
}

public class BossMago : MonoBehaviour, IVida
{
    [Header("Vida")]
    public int VidaMaxima = 500;
    public int Vida;
    public Slider BarraDeVida;                       
    public bool EsconderBarraAteIniciar = true;
    [Range(0f, 1f)] public float PorcentagemFase2 = 0.66f;
    [Range(0f, 1f)] public float PorcentagemFase3 = 0.33f;
    [Header("Dano (1,5x o da Medusa)")]
    public int DanoOrbes = 15;                      
    public int DanoMeteoros = 30;                    
    public int DanoOnda = 23;                        
    [Header("Detecção / Arena")]
    public Transform Jogador;                        
    public float RangeDeteccao = 14f;                
    public float MetadeLarguraArena = 10f;           
    public LayerMask CamadaDoChao;                   
    public float DistanciaFallbackAteChao = 1.5f;   
    public Transform PontoCentro;                    
    [Header("Teleporte (poder passivo)")]
    public Transform[] PontosDeTeleporte;            
    public float DistanciaMinimaDoJogador = 5f;
    public float DistanciaMinimaDoSalto = 4f;
    public float DuracaoFadeTeleporte = 0.25f;
    [Header("Ataque 1 - Orbes")]
    public GameObject PrefabProjetil;                
    public float RaioOrbita = 1.9f;
    public float VelocidadeOrbitaGraus = 220f;
    public Vector2 OffsetConjuracao = new Vector2(0f, 0.6f);
    public Color CorOrbe = new Color(0.55f, 0.85f, 1f, 1f);
    [Header("Ataque 2 - Meteoros")]
    public float DispersaoMeteoros = 6f;
    public float AlturaDoImpacto = 2.4f;
    [Header("Ataque 3 - Onda de Choque")]
    public float TempoAvisoOnda = 0.8f;
    public Vector2 TamanhoOnda = new Vector2(1.3f, 1.5f);
    public float AlcanceOnda = 24f;
    [Header("Fases (ajuste a dificuldade aqui)")]
    public ConfigFaseMago[] Fases = new ConfigFaseMago[]
    {
        new ConfigFaseMago
        {
            Nome = "Fase 1 - Aprendiz",
            TempoEntreAtaques = 3.2f, TeleporteMin = 8f, TeleporteMax = 11f, ChanceCombo = 0f, EmboscadaAposTeleporte = false,
            OrbesQuantidade = 3, OrbesVelocidade = 7f, OrbesTempoConjuracao = 0.9f, OrbesIntervaloDisparo = 0.35f, OrbesDispersaoGraus = 0f,
            MeteorosQuantidade = 3, MeteorosTempoAviso = 1.3f, MeteorosRaio = 1.6f, MeteorosIntervalo = 0.35f,
            OndasQuantidade = 1, OndaDosDoisLados = false, OndaVelocidade = 8f, OndaIntervalo = 0.9f
        },
        new ConfigFaseMago
        {
            Nome = "Fase 2 - Arquimago",
            TempoEntreAtaques = 2.3f, TeleporteMin = 5.5f, TeleporteMax = 8f, ChanceCombo = 0.15f, EmboscadaAposTeleporte = false,
            OrbesQuantidade = 5, OrbesVelocidade = 8.5f, OrbesTempoConjuracao = 0.8f, OrbesIntervaloDisparo = 0.25f, OrbesDispersaoGraus = 5f,
            MeteorosQuantidade = 5, MeteorosTempoAviso = 1.05f, MeteorosRaio = 1.7f, MeteorosIntervalo = 0.28f,
            OndasQuantidade = 1, OndaDosDoisLados = true, OndaVelocidade = 9.5f, OndaIntervalo = 0.9f
        },
        new ConfigFaseMago
        {
            Nome = "Fase 3 - Furia Arcana",
            TempoEntreAtaques = 1.6f, TeleporteMin = 3.5f, TeleporteMax = 5.5f, ChanceCombo = 0.35f, EmboscadaAposTeleporte = true,
            OrbesQuantidade = 8, OrbesVelocidade = 9.5f, OrbesTempoConjuracao = 0.7f, OrbesIntervaloDisparo = 0.16f, OrbesDispersaoGraus = 9f,
            MeteorosQuantidade = 7, MeteorosTempoAviso = 0.85f, MeteorosRaio = 1.8f, MeteorosIntervalo = 0.2f,
            OndasQuantidade = 2, OndaDosDoisLados = true, OndaVelocidade = 11f, OndaIntervalo = 0.9f
        }
    };
    public Color[] CoresDasFases = new Color[]
    {
        Color.white,
        new Color(1f, 0.92f, 0.78f, 1f),
        new Color(1f, 0.78f, 0.78f, 1f)
    };
    [Header("Visual")]
    public bool SpriteOlhaParaDireita = true;        
    public float AmplitudeFlutuar = 0.12f;           
    public float VelocidadeFlutuar = 2.2f;
    public Camera CameraDoJogo;                    
    [Header("Transição de fase")]
    public float DuracaoTransicaoFase = 1.6f;
    public float RaioNovaArcana = 6f;                
    public float ForcaNovaArcana = 14f;
    [Header("Derrota")]
    public int PontosAoDerrotar = 1000;
    public Sprite SpriteMagoDerrotado;              
    public Vector2 DeslocamentoAoMorrer = Vector2.zero;
    public Image TelaBranca;                         
    public float DuracaoTremidaMorte = 0.6f;
    public float IntensidadeTremidaMorte = 0.25f;
    public float DuracaoFadeParaBranco = 1f;
    public float DuracaoTelaBranca = 2.5f;
    public float DuracaoFadeDeVolta = 1f;
    public string CenaAposDerrota = "Final";         
    public float EsperaAntesDaCena = 0.5f;
    Animator animador;
    List<string> parametrosBool = new List<string>();
    List<string> parametrosTrigger = new List<string>();
    SpriteRenderer spriteBoss;
    Rigidbody2D corpo;
    Rigidbody2D corpoJogador;
    Collider2D[] colisores;
    Color corBase = Color.white;
    Vector3 posicaoInicial;
    float baseY;
    float chaoReferenciaY;
    const float AlturaBuscaChao = 2.5f;
    int faseAtual = 1;
    bool lutaIniciada;
    bool ocupado;
    bool invulneravel;
    bool morto;
    bool emTransicaoDeFase;
    bool visualBloqueado;
    int tokenFade;
    float timerAtaque;
    float timerTeleporte;
    int ultimoAtaque = -1;
    Coroutine rotinaAtual;
    List<GameObject> orbesEmConjuracao;
    bool tremendo;
    Camera cameraTremendo;
    Vector3 posCameraOriginal;
    ConfigFaseMago configPadrao = new ConfigFaseMago();
    Color corTeleporteInicio = new Color(0.6f, 0.4f, 1f, 0.85f);
    Color corTeleporteFim = new Color(0.6f, 0.4f, 1f, 0f);
    public bool EstaMorto { get { return morto; } }
    public int FaseAtual { get { return faseAtual; } }

    void Start()
    {
        Vida = VidaMaxima;
        spriteBoss = GetComponent<SpriteRenderer>();
        corpo = GetComponent<Rigidbody2D>();
        colisores = GetComponentsInChildren<Collider2D>();
        posicaoInicial = transform.position;
        baseY = posicaoInicial.y;

        CachearParametrosDoAnimator();

        if (corpo != null)
        {
            corpo.bodyType = RigidbodyType2D.Kinematic;
            corpo.gravityScale = 0f;
            corpo.velocity = Vector2.zero;
            corpo.freezeRotation = true;
        }

        if (Jogador == null)
        {
            GameObject objJogador = GameObject.FindGameObjectWithTag("Player");
            if (objJogador != null) Jogador = objJogador.transform;
        }
        if (Jogador != null) corpoJogador = Jogador.GetComponent<Rigidbody2D>();

        CalcularChaoDeReferencia();
        AplicarCorDaFase();

        if (BarraDeVida != null && EsconderBarraAteIniciar) BarraDeVida.gameObject.SetActive(false);
        AtualizarBarra();
    }

    void CalcularChaoDeReferencia()
    {
        chaoReferenciaY = transform.position.y - DistanciaFallbackAteChao;
        if (CamadaDoChao.value != 0)
        {
            RaycastHit2D batida = Physics2D.Raycast(transform.position, Vector2.down, 40f, CamadaDoChao);
            if (batida.collider != null) chaoReferenciaY = batida.point.y;
        }
    }

    float EncontrarChaoY(float x)
    {
        if (CamadaDoChao.value != 0)
        {
            Vector2 origem = new Vector2(x, chaoReferenciaY + AlturaBuscaChao);
            RaycastHit2D batida = Physics2D.Raycast(origem, Vector2.down, 40f, CamadaDoChao);
            if (batida.collider != null) return batida.point.y;
        }
        return chaoReferenciaY;
    }

    ConfigFaseMago ConfigAtual()
    {
        if (Fases == null || Fases.Length == 0) return configPadrao;
        int indice = Mathf.Clamp(faseAtual - 1, 0, Fases.Length - 1);
        if (Fases[indice] == null) return configPadrao;
        return Fases[indice];
    }

    void Update()
    {
        if (morto || Jogador == null) return;

        VirarParaJogador();
        Flutuar();

        if (emTransicaoDeFase) return;

        if (!lutaIniciada)
        {
            if (Vector2.Distance(transform.position, Jogador.position) <= RangeDeteccao) IniciarLuta();
            return;
        }

        if (ocupado) return;

        timerTeleporte -= Time.deltaTime;
        timerAtaque -= Time.deltaTime;

        if (timerTeleporte <= 0f)
        {
            ocupado = true;
            rotinaAtual = StartCoroutine(Teleportar());
            return;
        }

        if (timerAtaque <= 0f) EscolherEIniciarAtaque();
    }

    void IniciarLuta()
    {
        lutaIniciada = true;
        if (BarraDeVida != null) BarraDeVida.gameObject.SetActive(true);
        ConfigFaseMago cfg = ConfigAtual();
        timerAtaque = 1.5f;
        timerTeleporte = Random.Range(cfg.TeleporteMin, cfg.TeleporteMax);
    }

    void VirarParaJogador()
    {
        if (spriteBoss == null || Jogador == null) return;
        bool jogadorNaEsquerda = Jogador.position.x < transform.position.x;
        spriteBoss.flipX = SpriteOlhaParaDireita ? jogadorNaEsquerda : !jogadorNaEsquerda;
    }

    void Flutuar()
    {
        if (AmplitudeFlutuar <= 0f) return;
        Vector3 p = transform.position;
        transform.position = new Vector3(p.x, baseY + Mathf.Sin(Time.time * VelocidadeFlutuar) * AmplitudeFlutuar, p.z);
    }

    void EscolherEIniciarAtaque()
    {
        int escolha = Random.Range(0, 3);
        if (escolha == ultimoAtaque) escolha = (escolha + 1 + Random.Range(0, 2)) % 3;
        ultimoAtaque = escolha;

        ocupado = true;
        if (escolha == 0) rotinaAtual = StartCoroutine(AtaqueOrbes());
        else if (escolha == 1) rotinaAtual = StartCoroutine(AtaqueMeteoros());
        else rotinaAtual = StartCoroutine(AtaqueOnda());
    }

    void FinalizarAcao()
    {
        ConfigFaseMago cfg = ConfigAtual();
        timerAtaque = cfg.TempoEntreAtaques;
        if (Random.value < cfg.ChanceCombo) timerAtaque = 0.35f; // combo: emenda outro ataque
        rotinaAtual = null;
        ocupado = false;
    }

    IEnumerator Teleportar()
    {
        ConfigFaseMago cfg = ConfigAtual();
        invulneravel = true;
        AnimTrigger("Teleportar");
        DefinirColisores(false);

        EfeitoPulso.Criar(transform.position, new Vector2(1f, 1f), new Vector2(3.5f, 3.5f), corTeleporteInicio, corTeleporteFim, 0.4f, 55);
        yield return StartCoroutine(FadeSprite(1f, 0f, DuracaoFadeTeleporte));

        Vector3 destino = EscolherDestinoTeleporte();
        transform.position = destino;
        baseY = destino.y;
        VirarParaJogador();

        EfeitoPulso.Criar(destino, new Vector2(3.5f, 3.5f), new Vector2(1f, 1f), corTeleporteInicio, corTeleporteFim, 0.4f, 55);
        yield return StartCoroutine(FadeSprite(0f, 1f, DuracaoFadeTeleporte));

        DefinirColisores(true);
        invulneravel = false;

        timerTeleporte = Random.Range(cfg.TeleporteMin, cfg.TeleporteMax);
        timerAtaque = cfg.EmboscadaAposTeleporte ? 0.25f : Mathf.Max(timerAtaque, 0.7f);
        rotinaAtual = null;
        ocupado = false;
    }

    Vector3 EscolherDestinoTeleporte()
    {
        List<Vector3> candidatos = new List<Vector3>();

        if (PontosDeTeleporte != null)
        {
            for (int i = 0; i < PontosDeTeleporte.Length; i++)
            {
                if (PontosDeTeleporte[i] != null)
                    candidatos.Add(new Vector3(PontosDeTeleporte[i].position.x, PontosDeTeleporte[i].position.y, transform.position.z));
            }
        }

        if (candidatos.Count == 0)
        {
            for (int i = 0; i < 12; i++)
            {
                float x = posicaoInicial.x + Random.Range(-MetadeLarguraArena, MetadeLarguraArena);
                candidatos.Add(new Vector3(x, posicaoInicial.y, transform.position.z));
            }
        }

        List<Vector3> validos = new List<Vector3>();
        Vector3 maisLonge = candidatos[0];
        float maiorDistancia = -1f;

        for (int i = 0; i < candidatos.Count; i++)
        {
            float distJogador = Vector2.Distance(candidatos[i], Jogador.position);
            float distAtual = Vector2.Distance(candidatos[i], transform.position);

            if (distJogador > maiorDistancia)
            {
                maiorDistancia = distJogador;
                maisLonge = candidatos[i];
            }
            if (distJogador >= DistanciaMinimaDoJogador && distAtual >= DistanciaMinimaDoSalto) validos.Add(candidatos[i]);
        }

        if (validos.Count > 0) return validos[Random.Range(0, validos.Count)];
        return maisLonge;
    }

    IEnumerator AtaqueOrbes()
    {
        ConfigFaseMago cfg = ConfigAtual();
        AnimBool("AtaqueOrbes", true);

        int quantidade = Mathf.Max(1, cfg.OrbesQuantidade);
        List<GameObject> orbes = new List<GameObject>();
        orbesEmConjuracao = orbes;

        for (int i = 0; i < quantidade; i++) orbes.Add(CriarOrbe(CentroConjuracao()));

        int proximo = 0;
        float tempo = 0f;
        float proximoDisparo = cfg.OrbesTempoConjuracao;

        while (proximo < quantidade)
        {
            tempo += Time.deltaTime;
            float crescimento = Mathf.Clamp01(tempo / 0.35f);
            Vector3 centro = CentroConjuracao();

            for (int i = proximo; i < quantidade; i++)
            {
                if (orbes[i] == null) continue;
                float angulo = (tempo * VelocidadeOrbitaGraus + i * (360f / quantidade)) * Mathf.Deg2Rad;
                Vector3 deslocamento = new Vector3(Mathf.Cos(angulo), Mathf.Sin(angulo), 0f) * (RaioOrbita * crescimento);
                orbes[i].transform.position = centro + deslocamento;
            }

            if (tempo >= proximoDisparo)
            {
                DispararOrbe(orbes[proximo], cfg);
                proximo++;
                proximoDisparo += cfg.OrbesIntervaloDisparo;
            }
            yield return null;
        }

        orbesEmConjuracao = null;
        yield return new WaitForSeconds(0.5f);
        AnimBool("AtaqueOrbes", false);
        FinalizarAcao();
    }

    Vector3 CentroConjuracao()
    {
        return transform.position + new Vector3(OffsetConjuracao.x, OffsetConjuracao.y, 0f);
    }

    GameObject CriarOrbe(Vector3 posicao)
    {
        GameObject orbe;
        if (PrefabProjetil != null)
        {
            orbe = (GameObject)Instantiate(PrefabProjetil, posicao, Quaternion.identity);
            orbe.SetActive(true);
        }
        else
        {
            orbe = EfeitoPulso.CriarSprite("OrbeArcano", posicao, new Vector2(0.7f, 0.7f), CorOrbe, 60);
            CircleCollider2D circulo = orbe.AddComponent<CircleCollider2D>();
            circulo.isTrigger = true;
            circulo.radius = 0.45f;
        }

        ProjetilBoss projetil = orbe.GetComponent<ProjetilBoss>();
        if (projetil == null) projetil = orbe.AddComponent<ProjetilBoss>();
        projetil.Dano = DanoOrbes;
        projetil.Dono = gameObject;

        Rigidbody2D rb = orbe.GetComponent<Rigidbody2D>();
        if (rb == null) rb = orbe.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.velocity = Vector2.zero;

        // Enquanto gira em volta do mago, a orbe ainda não machuca
        Collider2D[] colisoresOrbe = orbe.GetComponentsInChildren<Collider2D>();
        for (int i = 0; i < colisoresOrbe.Length; i++) colisoresOrbe[i].enabled = false;

        Destroy(orbe, 12f);
        return orbe;
    }

    void DispararOrbe(GameObject orbe, ConfigFaseMago cfg)
    {
        if (orbe == null || Jogador == null) return;

        Vector2 origem = orbe.transform.position;
        Vector2 direcao = ((Vector2)Jogador.position - origem).normalized;
        direcao = Rotacionar(direcao, Random.Range(-cfg.OrbesDispersaoGraus, cfg.OrbesDispersaoGraus));

        Collider2D[] cols = orbe.GetComponentsInChildren<Collider2D>();
        for (int i = 0; i < cols.Length; i++) cols[i].enabled = true;

        Rigidbody2D rb = orbe.GetComponent<Rigidbody2D>();
        if (rb != null) rb.velocity = direcao * cfg.OrbesVelocidade;

        Color sumindo = new Color(CorOrbe.r, CorOrbe.g, CorOrbe.b, 0f);
        EfeitoPulso.Criar(orbe.transform.position, new Vector2(0.5f, 0.5f), new Vector2(1.6f, 1.6f), CorOrbe, sumindo, 0.25f, 61);
    }

    Vector2 Rotacionar(Vector2 v, float graus)
    {
        float rad = graus * Mathf.Deg2Rad;
        float c = Mathf.Cos(rad);
        float s = Mathf.Sin(rad);
        return new Vector2(v.x * c - v.y * s, v.x * s + v.y * c);
    }

    IEnumerator AtaqueMeteoros()
    {
        ConfigFaseMago cfg = ConfigAtual();
        AnimBool("AtaqueMeteoros", true);

        yield return new WaitForSeconds(0.5f); // levanta o cajado

        int quantidade = Mathf.Max(1, cfg.MeteorosQuantidade);
        float limiteMin = posicaoInicial.x - MetadeLarguraArena - 2f;
        float limiteMax = posicaoInicial.x + MetadeLarguraArena + 2f;

        for (int i = 0; i < quantidade; i++)
        {
            float alvoX;
            if (i == 0) alvoX = Jogador.position.x + PrevisaoMovimentoJogador(cfg.MeteorosTempoAviso);
            else alvoX = Jogador.position.x + Random.Range(-DispersaoMeteoros, DispersaoMeteoros);

            alvoX = Mathf.Clamp(alvoX, limiteMin, limiteMax);
            CriarMeteoro(alvoX, cfg);
            yield return new WaitForSeconds(cfg.MeteorosIntervalo);
        }

        yield return new WaitForSeconds(0.4f);
        AnimBool("AtaqueMeteoros", false);
        FinalizarAcao();
    }

    float PrevisaoMovimentoJogador(float tempo)
    {
        if (corpoJogador == null) return 0f;
        return Mathf.Clamp(corpoJogador.velocity.x * tempo * 0.6f, -6f, 6f);
    }

    void CriarMeteoro(float alvoX, ConfigFaseMago cfg)
    {
        float chaoY = EncontrarChaoY(alvoX);

        GameObject objeto = new GameObject("MeteoroArcano");
        objeto.transform.position = new Vector3(alvoX, chaoY, transform.position.z);

        MeteoroArcano meteoro = objeto.AddComponent<MeteoroArcano>();
        meteoro.Alvo = new Vector2(alvoX, chaoY);
        meteoro.Raio = cfg.MeteorosRaio;
        meteoro.TempoAviso = cfg.MeteorosTempoAviso;
        meteoro.AlturaDoImpacto = AlturaDoImpacto;
        meteoro.Dano = DanoMeteoros;
    }

    IEnumerator AtaqueOnda()
    {
        ConfigFaseMago cfg = ConfigAtual();
        AnimBool("AtaqueOnda", true);

        Vector3 chaoDoMago = new Vector3(transform.position.x, EncontrarChaoY(transform.position.x) + 0.05f, transform.position.z);
        Color corAviso = new Color(0.75f, 0.3f, 1f, 0.6f);
        EfeitoPulso.Criar(chaoDoMago, new Vector2(1f, 0.3f), new Vector2(7f, 1.2f), corAviso, new Color(corAviso.r, corAviso.g, corAviso.b, 0f), TempoAvisoOnda, 5);
        yield return new WaitForSeconds(TempoAvisoOnda);

        int quantidade = Mathf.Max(1, cfg.OndasQuantidade);
        for (int i = 0; i < quantidade; i++)
        {
            float lado = Jogador.position.x < transform.position.x ? -1f : 1f;
            LancarOnda(lado, cfg);
            if (cfg.OndaDosDoisLados) LancarOnda(-lado, cfg);
            if (i < quantidade - 1) yield return new WaitForSeconds(cfg.OndaIntervalo);
        }

        yield return new WaitForSeconds(0.6f);
        AnimBool("AtaqueOnda", false);
        FinalizarAcao();
    }

    void LancarOnda(float lado, ConfigFaseMago cfg)
    {
        float xOrigem = transform.position.x + lado * 1.2f;
        float chaoY = EncontrarChaoY(xOrigem);
        Vector3 posicao = new Vector3(xOrigem, chaoY + TamanhoOnda.y * 0.5f + 0.05f, transform.position.z);

        GameObject objeto = EfeitoPulso.CriarSprite("OndaArcana", posicao, TamanhoOnda, new Color(0.75f, 0.3f, 1f, 0.9f), 58);
        OndaArcana onda = objeto.AddComponent<OndaArcana>();
        onda.Direcao = lado;
        onda.Velocidade = cfg.OndaVelocidade;
        onda.AlcanceMaximo = AlcanceOnda;
        onda.Tamanho = TamanhoOnda;
        onda.Dano = DanoOnda;
        onda.CamadaParedes = CamadaDoChao;
    }

    public void ReceberDano(int dano)
    {
        if (morto || invulneravel) return;
        if (!lutaIniciada) IniciarLuta();

        Vida -= dano;
        if (Vida < 0) Vida = 0;
        Debug.Log("Mago levou " + dano + " de dano. Vida: " + Vida + "/" + VidaMaxima);

        AnimTrigger("TomouDano");
        StartCoroutine(Piscar());
        AtualizarBarra();

        if (Vida <= 0)
        {
            Morrer();
            return;
        }

        int faseAlvo = CalcularFasePelaVida();
        if (faseAlvo > faseAtual && !emTransicaoDeFase) IniciarTransicaoDeFase(faseAlvo);
    }

    int CalcularFasePelaVida()
    {
        float porcentagem = (float)Vida / (float)VidaMaxima;
        if (porcentagem <= PorcentagemFase3) return 3;
        if (porcentagem <= PorcentagemFase2) return 2;
        return 1;
    }

    void IniciarTransicaoDeFase(int novaFase)
    {
        emTransicaoDeFase = true;
        ocupado = true;
        invulneravel = true;

        if (rotinaAtual != null)
        {
            StopCoroutine(rotinaAtual);
            rotinaAtual = null;
        }
        LimparPerigos();
        ResetarAnimacoes();
        RestaurarVisual();

        rotinaAtual = StartCoroutine(TransicaoDeFase(novaFase));
    }

    IEnumerator TransicaoDeFase(int novaFase)
    {
        invulneravel = true;
        DefinirColisores(false);
        AnimTrigger("MudouFase");

        yield return StartCoroutine(FadeSprite(1f, 0f, 0.25f));

        Vector3 centro = PontoCentro != null ? PontoCentro.position : posicaoInicial;
        transform.position = new Vector3(centro.x, centro.y, transform.position.z);
        baseY = centro.y;
        faseAtual = novaFase;
        AplicarCorDaFase();

        yield return StartCoroutine(FadeSprite(0f, 1f, 0.25f));

        StartCoroutine(TremerCamera(0.6f, 0.25f));
        EfeitoPulso.Criar(transform.position, new Vector2(1f, 1f), new Vector2(RaioNovaArcana * 2f, RaioNovaArcana * 2f),
            new Color(1f, 1f, 1f, 0.9f), new Color(0.7f, 0.4f, 1f, 0f), 0.7f, 62);
        EmpurrarJogadorParaLonge();

        yield return new WaitForSeconds(DuracaoTransicaoFase);

        ConfigFaseMago cfg = ConfigAtual();
        DefinirColisores(true);
        invulneravel = false;
        timerAtaque = 1.2f;
        timerTeleporte = Random.Range(cfg.TeleporteMin, cfg.TeleporteMax);
        emTransicaoDeFase = false;
        rotinaAtual = null;
        ocupado = false;
    }

    void EmpurrarJogadorParaLonge()
    {
        if (Jogador == null) return;
        Personagem scriptJogador = Jogador.GetComponent<Personagem>();
        if (scriptJogador == null) return;

        Vector2 diferenca = Jogador.position - transform.position;
        if (diferenca.magnitude > RaioNovaArcana) return;

        float lado = diferenca.x >= 0f ? 1f : -1f;
        scriptJogador.AplicarEmpurrao(new Vector2(lado * ForcaNovaArcana, ForcaNovaArcana * 0.5f), 0.3f);
    }

   
    void Morrer()
    {
        morto = true;
        ocupado = true;
        invulneravel = true;

        StopAllCoroutines(); 
        PararTremida();
        LimparPerigos();
        ResetarAnimacoes();
        DefinirColisores(false); // depois de morto o jogador não "bate" mais nele
        if (corpo != null) corpo.velocity = Vector2.zero;
        RestaurarVisual();

        GerenciadorPontuacao.Instancia.AdicionarPontosInimigo(PontosAoDerrotar);
        AtualizarBarra();

        StartCoroutine(EfeitoMorte());
    }

    IEnumerator EfeitoMorte()
    {
        bool trocarSprite = SpriteMagoDerrotado != null && spriteBoss != null;
        if (trocarSprite)
        {
            if (animador != null) animador.enabled = false;
        }
        else
        {
            AnimTrigger("Morreu");
        }

        yield return StartCoroutine(TremerCamera(DuracaoTremidaMorte, IntensidadeTremidaMorte));

        if (TelaBranca != null) yield return StartCoroutine(FadeTela(0f, 1f, DuracaoFadeParaBranco));

        // A troca acontece com a tela toda branca, então o "corte" não aparece
        if (trocarSprite)
        {
            spriteBoss.sprite = SpriteMagoDerrotado;
            transform.position += (Vector3)DeslocamentoAoMorrer;
        }

        yield return new WaitForSecondsRealtime(TelaBranca != null ? DuracaoTelaBranca : 1.5f);

        if (TelaBranca != null) yield return StartCoroutine(FadeTela(1f, 0f, DuracaoFadeDeVolta));

        if (!string.IsNullOrEmpty(CenaAposDerrota))
        {
            yield return new WaitForSecondsRealtime(EsperaAntesDaCena);
            SceneManager.LoadScene(CenaAposDerrota);
        }
    }

    void LimparPerigos()
    {
        if (orbesEmConjuracao != null)
        {
            for (int i = 0; i < orbesEmConjuracao.Count; i++)
            {
                if (orbesEmConjuracao[i] != null) Destroy(orbesEmConjuracao[i]);
            }
            orbesEmConjuracao = null;
        }

        MeteoroArcano[] meteoros = FindObjectsOfType<MeteoroArcano>();
        for (int i = 0; i < meteoros.Length; i++) Destroy(meteoros[i].gameObject);

        OndaArcana[] ondas = FindObjectsOfType<OndaArcana>();
        for (int i = 0; i < ondas.Length; i++) Destroy(ondas[i].gameObject);

        ProjetilBoss[] projeteis = FindObjectsOfType<ProjetilBoss>();
        for (int i = 0; i < projeteis.Length; i++)
        {
            if (projeteis[i].Dono == gameObject) Destroy(projeteis[i].gameObject);
        }
    }

    void DefinirColisores(bool ativo)
    {
        if (colisores == null) return;
        for (int i = 0; i < colisores.Length; i++)
        {
            if (colisores[i] != null) colisores[i].enabled = ativo;
        }
    }

    void AplicarCorDaFase()
    {
        if (spriteBoss == null) return;

        Color tint = Color.white;
        if (CoresDasFases != null && CoresDasFases.Length > 0)
        {
            int indice = Mathf.Clamp(faseAtual - 1, 0, CoresDasFases.Length - 1);
            tint = CoresDasFases[indice];
        }
        corBase = tint;

        Color aplicada = corBase;
        aplicada.a = spriteBoss.color.a; // preserva a transparência (pode estar no meio do teleporte)
        spriteBoss.color = aplicada;
    }

    void RestaurarVisual()
    {
        tokenFade++; // cancela qualquer fade que esteja rodando
        visualBloqueado = false;
        if (spriteBoss != null) spriteBoss.color = corBase;
    }

    IEnumerator Piscar()
    {
        if (spriteBoss == null) yield break;
        // Usa exatamente Color.red pra combinar com o piscar do Personagem (que ignora quem já está vermelho)
        spriteBoss.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        if (spriteBoss != null && !visualBloqueado && !morto) spriteBoss.color = corBase;
    }

    IEnumerator FadeSprite(float alphaDe, float alphaPara, float duracao)
    {
        if (spriteBoss == null) yield break;

        int meuToken = ++tokenFade;
        visualBloqueado = true;

        Color cor = corBase;
        float tempo = 0f;
        while (tempo < duracao)
        {
            if (meuToken != tokenFade) yield break;
            cor.a = Mathf.Lerp(alphaDe, alphaPara, tempo / duracao);
            spriteBoss.color = cor;
            tempo += Time.deltaTime;
            yield return null;
        }

        if (meuToken != tokenFade) yield break;
        cor.a = alphaPara;
        spriteBoss.color = cor;
        visualBloqueado = false;
    }

    IEnumerator FadeTela(float alphaDe, float alphaPara, float duracao)
    {
        if (TelaBranca == null) yield break;

        Color cor = TelaBranca.color;
        float tempo = 0f;
        while (tempo < duracao)
        {
            cor.a = Mathf.Lerp(alphaDe, alphaPara, tempo / duracao);
            TelaBranca.color = cor;
            tempo += Time.unscaledDeltaTime;
            yield return null;
        }
        cor.a = alphaPara;
        TelaBranca.color = cor;
    }

    IEnumerator TremerCamera(float duracao, float intensidade)
    {
        if (tremendo) yield break;
        Camera camera = CameraDoJogo != null ? CameraDoJogo : Camera.main;
        if (camera == null) yield break;

        tremendo = true;
        cameraTremendo = camera;
        posCameraOriginal = camera.transform.localPosition;

        float tempo = 0f;
        while (tempo < duracao)
        {
            float x = Random.Range(-1f, 1f) * intensidade;
            float y = Random.Range(-1f, 1f) * intensidade;
            camera.transform.localPosition = posCameraOriginal + new Vector3(x, y, 0f);
            tempo += Time.unscaledDeltaTime;
            yield return null;
        }

        camera.transform.localPosition = posCameraOriginal;
        tremendo = false;
    }

    void PararTremida()
    {
        if (tremendo && cameraTremendo != null) cameraTremendo.transform.localPosition = posCameraOriginal;
        tremendo = false;
    }

    void AtualizarBarra()
    {
        if (BarraDeVida == null) return;
        BarraDeVida.minValue = 0;
        BarraDeVida.maxValue = VidaMaxima;
        BarraDeVida.value = Vida;
    }

    
    void CachearParametrosDoAnimator()
    {
        animador = GetComponent<Animator>();
        if (animador == null || animador.runtimeAnimatorController == null) return;

        AnimatorControllerParameter[] parametros = animador.parameters;
        for (int i = 0; i < parametros.Length; i++)
        {
            if (parametros[i].type == AnimatorControllerParameterType.Bool) parametrosBool.Add(parametros[i].name);
            else if (parametros[i].type == AnimatorControllerParameterType.Trigger) parametrosTrigger.Add(parametros[i].name);
        }
    }

    void AnimBool(string nome, bool valor)
    {
        if (animador != null && animador.enabled && parametrosBool.Contains(nome)) animador.SetBool(nome, valor);
    }

    void AnimTrigger(string nome)
    {
        if (animador != null && animador.enabled && parametrosTrigger.Contains(nome)) animador.SetTrigger(nome);
    }

    void ResetarAnimacoes()
    {
        AnimBool("AtaqueOrbes", false);
        AnimBool("AtaqueMeteoros", false);
        AnimBool("AtaqueOnda", false);
    }

    void OnDrawGizmosSelected()
    {
        Vector3 origem = Application.isPlaying ? posicaoInicial : transform.position;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, RangeDeteccao);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(origem + Vector3.left * MetadeLarguraArena, origem + Vector3.right * MetadeLarguraArena);

        Gizmos.color = Color.magenta;
        if (PontosDeTeleporte != null)
        {
            for (int i = 0; i < PontosDeTeleporte.Length; i++)
            {
                if (PontosDeTeleporte[i] != null) Gizmos.DrawWireSphere(PontosDeTeleporte[i].position, 0.5f);
            }
        }
    }
}