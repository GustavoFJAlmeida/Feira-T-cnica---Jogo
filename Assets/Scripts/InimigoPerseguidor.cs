using UnityEngine;

public class InimigoPerseguidor : MonoBehaviour, IAtordoavel
{
    SpriteRenderer PersonagemSpriteRenderer;
    Vector2 PosicaoInicial;

    public float Distancia;
    public float VelocidadeMovimento;
    public Vector2 Direcao;
    public GameObject Player;              
    public float MaximoPerto;
    public float VelocidadePerseguicao = 3f;
    public bool PerseguirNoEixoY = false;
    public float GravidadeQuandoTerrestre = 3f; 
    public float GravidadeAtordoado = 3f;

    Rigidbody2D CorpoRigido;
    bool EstaAtordoado;
    bool EstavaPerseguindo;
    float direcaoPatrulha = 1f;

    void Start()
    {
        PersonagemSpriteRenderer = GetComponent<SpriteRenderer>();
        CorpoRigido = GetComponent<Rigidbody2D>();
        PosicaoInicial = transform.position;
        EstaAtordoado = false;

        if (Player == null)
        {
            GameObject objJogador = GameObject.FindGameObjectWithTag("Player");
            if (objJogador != null) Player = objJogador;
        }

        if (CorpoRigido != null)
        {
            CorpoRigido.bodyType = RigidbodyType2D.Dynamic;
            CorpoRigido.freezeRotation = true;
            CorpoRigido.gravityScale = PerseguirNoEixoY ? 0f : GravidadeQuandoTerrestre;
        }
    }

    void Update()
    {
        if (EstaAtordoado || CorpoRigido == null) return;

        if (Player == null)
        {
            MovimentoPingPong();
            return;
        }

        float distancia = Vector2.Distance(Player.transform.position, transform.position);

        if (distancia <= MaximoPerto)
        {
            SeguirPlayer();
            EstavaPerseguindo = true;
        }
        else
        {
            if (EstavaPerseguindo)
            {
                PosicaoInicial = transform.position;
                EstavaPerseguindo = false;
            }
            MovimentoPingPong();
        }
    }

    void SeguirPlayer()
    {
        Vector2 direcao = (Vector2)Player.transform.position - CorpoRigido.position;
        if (!PerseguirNoEixoY) direcao.y = 0f;
        if (direcao != Vector2.zero) direcao = direcao.normalized;

        float velY = PerseguirNoEixoY ? direcao.y * VelocidadePerseguicao : CorpoRigido.velocity.y;
        CorpoRigido.velocity = new Vector2(direcao.x * VelocidadePerseguicao, velY);

        if (PersonagemSpriteRenderer != null && Mathf.Abs(direcao.x) > 0.01f)
            PersonagemSpriteRenderer.flipX = direcao.x < 0f;
    }

    void MovimentoPingPong()
    {
        Vector2 eixo = Direcao.normalized;
        if (eixo == Vector2.zero) return;

        float deslocamento = Vector2.Dot((Vector2)transform.position - PosicaoInicial, eixo);
        if (deslocamento >= Distancia) direcaoPatrulha = -1f;
        else if (deslocamento <= 0f) direcaoPatrulha = 1f;

        Vector2 velocidadePatrulha = eixo * VelocidadeMovimento * direcaoPatrulha;
        float velY = PerseguirNoEixoY ? velocidadePatrulha.y : CorpoRigido.velocity.y;
        CorpoRigido.velocity = new Vector2(velocidadePatrulha.x, velY);

        if (PersonagemSpriteRenderer != null)
            PersonagemSpriteRenderer.flipX = direcaoPatrulha < 0f;
    }

    public void Atordoar(float duracao)
    {
        EstaAtordoado = true;

        if (CorpoRigido != null)
        {
            CorpoRigido.gravityScale = GravidadeAtordoado;
        }

        CancelInvoke("VoltarAndar");
        Invoke("VoltarAndar", duracao);
    }

    void VoltarAndar()
    {
        EstaAtordoado = false;
        PosicaoInicial = transform.position;

        if (CorpoRigido != null)
        {
            CorpoRigido.velocity = Vector2.zero;
            CorpoRigido.gravityScale = PerseguirNoEixoY ? 0f : GravidadeQuandoTerrestre;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, MaximoPerto);
    }
}