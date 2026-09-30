using UnityEngine;

public class Inimigo : MonoBehaviour, IAtordoavel
{
    SpriteRenderer PersonagemSpriteRenderer;
    Vector2 PosicaoInicial;

    public float Distancia;
    public float VelocidadeMovimento;
    public Vector2 Direcao;
    public float GravidadeAoAndar = 0f;  
    public float GravidadeAtordoado = 3f; 

    Rigidbody2D CorpoRigido;
    bool EstaAtordoado;
    float direcaoAtual = 1f;

    void Start()
    {
        PersonagemSpriteRenderer = GetComponent<SpriteRenderer>();
        CorpoRigido = GetComponent<Rigidbody2D>();
        PosicaoInicial = transform.position;
        EstaAtordoado = false;

        if (CorpoRigido != null)
        {
            CorpoRigido.bodyType = RigidbodyType2D.Dynamic;
            CorpoRigido.freezeRotation = true;
            CorpoRigido.gravityScale = GravidadeAoAndar;
        }
    }

    void Update()
    {
        if (EstaAtordoado || CorpoRigido == null) return;
        MovimentoPingPong();
    }

    void MovimentoPingPong()
    {
        Vector2 eixo = Direcao.normalized;
        if (eixo == Vector2.zero) return;

        float deslocamento = Vector2.Dot((Vector2)transform.position - PosicaoInicial, eixo);

        if (deslocamento >= Distancia) direcaoAtual = -1f;
        else if (deslocamento <= 0f) direcaoAtual = 1f;

        Vector2 velocidadePatrulha = eixo * VelocidadeMovimento * direcaoAtual;

        float velY = GravidadeAoAndar > 0f ? CorpoRigido.velocity.y : velocidadePatrulha.y;
        CorpoRigido.velocity = new Vector2(velocidadePatrulha.x, velY);

        if (PersonagemSpriteRenderer != null)
            PersonagemSpriteRenderer.flipX = direcaoAtual < 0f;
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
            CorpoRigido.gravityScale = GravidadeAoAndar;
        }
    }
}