using UnityEngine;

public class OndaArcana : MonoBehaviour
{
    [HideInInspector] public float Direcao = 1f;         
    [HideInInspector] public float Velocidade = 9f;
    [HideInInspector] public float AlcanceMaximo = 24f;   
    [HideInInspector] public Vector2 Tamanho = new Vector2(1.3f, 1.5f);
    [HideInInspector] public int Dano = 23;
    [HideInInspector] public LayerMask CamadaParedes;     
    public Color CorOnda = new Color(0.75f, 0.3f, 1f, 0.9f);
    public Color CorRastro = new Color(0.6f, 0.2f, 1f, 0.5f);
    float percorrido;
    float timerRastro;
    bool jaAcertou;

    void Update()
    {
        float passo = Velocidade * Time.deltaTime;
        Vector2 centro = transform.position;

        if (CamadaParedes.value != 0)
        {
            RaycastHit2D batida = Physics2D.Raycast(centro, new Vector2(Direcao, 0f), passo + Tamanho.x * 0.5f, CamadaParedes);
            if (batida.collider != null)
            {
                Terminar();
                return;
            }
        }

        transform.position = new Vector3(centro.x + Direcao * passo, centro.y, transform.position.z);
        percorrido += passo;

        float pulso = 1f + 0.08f * Mathf.Sin(Time.time * 20f);
        float pulsoAltura = 1f + 0.10f * Mathf.Sin(Time.time * 17f);
        transform.localScale = new Vector3(Tamanho.x * pulso, Tamanho.y * pulsoAltura, 1f);

        if (!jaAcertou) VerificarJogador();

        timerRastro -= Time.deltaTime;
        if (timerRastro <= 0f)
        {
            timerRastro = 0.07f;
            Color sumindo = new Color(CorRastro.r, CorRastro.g, CorRastro.b, 0f);
            EfeitoPulso.Criar(transform.position, Tamanho * 0.9f, Tamanho * 0.35f, CorRastro, sumindo, 0.3f, 57);
        }

        if (percorrido >= AlcanceMaximo) Terminar();
    }

    void VerificarJogador()
    {
        Collider2D[] acertados = Physics2D.OverlapBoxAll(transform.position, Tamanho, 0f);
        for (int i = 0; i < acertados.Length; i++)
        {
            Personagem jogador = acertados[i].GetComponentInParent<Personagem>();
            if (jogador != null)
            {
                // Dash = invulnerável. Se ele ainda estiver dentro da onda quando o dash acabar, ela pode acertar.
                if (!jogador.EstaEmDashAtivo)
                {
                    jaAcertou = true;
                    jogador.ReceberDano(Dano, transform.position);
                }
                return;
            }
        }
    }

    void Terminar()
    {
        Color sumindo = new Color(CorOnda.r, CorOnda.g, CorOnda.b, 0f);
        EfeitoPulso.Criar(transform.position, Tamanho, Tamanho * 1.8f, CorOnda, sumindo, 0.3f, 58);
        Destroy(gameObject);
    }
}