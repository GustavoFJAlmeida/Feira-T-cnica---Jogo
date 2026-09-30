using UnityEngine;

public class InimigoSaltador : MonoBehaviour, IAtordoavel
{
    [Header("Perseguição / Pulo")]
    public float Alcance = 7f;
    public float TempoEntrePulos = 1.6f;
    public float ForcaPuloHorizontal = 4f;
    public float ForcaPuloVertical = 9f;
    [Header("Patrulha (quando o jogador está longe)")]
    public float DistanciaPatrulha = 3f;  
    public float VelocidadePatrulha = 1.5f;
    [Header("Checagem de chão (recomendado configurar)")]
    public Transform CheckPe;            
    public float RaioCheck = 0.2f;
    public LayerMask CamadaDoChao;
    Rigidbody2D corpo;
    SpriteRenderer sprite;
    Transform jogador;
    float timer;
    bool atordoado;
    Vector2 posicaoInicial;
    float direcaoPatrulha = 1f;

    void Start()
    {
        corpo = GetComponent<Rigidbody2D>();
        if (corpo != null)
        {
            corpo.bodyType = RigidbodyType2D.Dynamic;
            corpo.freezeRotation = true;
            if (corpo.gravityScale <= 0f) corpo.gravityScale = 3f;
        }
        sprite = GetComponent<SpriteRenderer>();

        GameObject objJogador = GameObject.FindGameObjectWithTag("Player");
        if (objJogador != null) jogador = objJogador.transform;
        else Debug.LogWarning(gameObject.name + ": não encontrei nenhum objeto com a tag 'Player' na cena.");

        posicaoInicial = transform.position;
        timer = TempoEntrePulos;
    }

    bool EstaNoChao()
    {
        if (CheckPe != null)
            return Physics2D.OverlapCircle(CheckPe.position, RaioCheck, CamadaDoChao) != null;
        return corpo != null && Mathf.Abs(corpo.velocity.y) < 0.3f;
    }

    void Update()
    {
        if (atordoado || corpo == null) return;

        bool noChao = EstaNoChao();
        float distancia = jogador != null ? Vector2.Distance(transform.position, jogador.position) : float.MaxValue;
        bool perseguindo = jogador != null && distancia <= Alcance;

        if (sprite != null)
            sprite.flipX = perseguindo ? (jogador.position.x < transform.position.x) : (direcaoPatrulha < 0f);

       
        if (!noChao) return;

        if (perseguindo)
        {
            corpo.velocity = new Vector2(Mathf.MoveTowards(corpo.velocity.x, 0f, 25f * Time.deltaTime), corpo.velocity.y);

            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                timer = TempoEntrePulos;
                float lado = jogador.position.x < transform.position.x ? -1f : 1f;
                corpo.velocity = new Vector2(lado * ForcaPuloHorizontal, ForcaPuloVertical);
            }
        }
        else
        {
            timer = TempoEntrePulos; 

            float distanciaDaOrigem = transform.position.x - posicaoInicial.x;
            if (distanciaDaOrigem >= DistanciaPatrulha) direcaoPatrulha = -1f;
            else if (distanciaDaOrigem <= -DistanciaPatrulha) direcaoPatrulha = 1f;

            corpo.velocity = new Vector2(direcaoPatrulha * VelocidadePatrulha, corpo.velocity.y);
        }
    }

    public void Atordoar(float duracao)
    {
        atordoado = true;
        if (corpo != null) corpo.velocity = new Vector2(0f, corpo.velocity.y);
        CancelInvoke("Recuperar");
        Invoke("Recuperar", duracao);
    }

    void Recuperar()
    {
        atordoado = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, Alcance);
        Gizmos.color = Color.cyan;
        Vector3 origem = Application.isPlaying ? (Vector3)posicaoInicial : transform.position;
        Gizmos.DrawLine(origem + Vector3.left * DistanciaPatrulha, origem + Vector3.right * DistanciaPatrulha);
    }
}