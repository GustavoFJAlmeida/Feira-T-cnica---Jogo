using System.Collections;
using UnityEngine;

public class MeteoroArcano : MonoBehaviour
{
    [HideInInspector] public Vector2 Alvo;              
    [HideInInspector] public float Raio = 1.7f;        
    [HideInInspector] public float AlturaDoImpacto = 2.4f; 
    [HideInInspector] public float TempoAviso = 1.2f;   
    [HideInInspector] public int Dano = 30;
    public float AlturaQueda = 12f;
    public float TempoQueda = 0.2f;
    public Color CorAviso = new Color(0.65f, 0.2f, 1f, 0.30f);
    public Color CorPreenchimento = new Color(1f, 0.3f, 0.85f, 0.55f);
    public Color CorMeteoro = new Color(1f, 0.6f, 1f, 1f);
    public Color CorExplosao = new Color(1f, 0.55f, 1f, 0.9f);

    void Start()
    {
        StartCoroutine(Rotina());
    }

    IEnumerator Rotina()
    {
        Vector3 chao = new Vector3(Alvo.x, Alvo.y, 0f);
        Vector2 tamanhoAviso = new Vector2(Raio * 2f, Raio * 0.55f);

        GameObject aviso = EfeitoPulso.CriarSprite("MeteoroAviso", chao + Vector3.up * 0.05f, tamanhoAviso, CorAviso, 4);
        aviso.transform.parent = transform;
        SpriteRenderer renderAviso = aviso.GetComponent<SpriteRenderer>();

        GameObject preenchimento = EfeitoPulso.CriarSprite("MeteoroPreenchimento", chao + Vector3.up * 0.05f, new Vector2(0.01f, 0.01f), CorPreenchimento, 5);
        preenchimento.transform.parent = transform;

        float tempo = 0f;
        while (tempo < TempoAviso)
        {
            tempo += Time.deltaTime;
            float progresso = Mathf.Clamp01(tempo / TempoAviso);
            preenchimento.transform.localScale = new Vector3(tamanhoAviso.x * progresso, tamanhoAviso.y * progresso, 1f);

            if (renderAviso != null)
            {
                Color cor = CorAviso;
                cor.a = Mathf.Lerp(CorAviso.a, Mathf.Min(1f, CorAviso.a * 2.2f), progresso);
                renderAviso.color = cor;
            }
            yield return null;
        }
        Destroy(preenchimento);

        Vector3 origemQueda = chao + Vector3.up * AlturaQueda;
        Vector3 destinoQueda = chao + Vector3.up * (Raio * 0.4f);
        GameObject meteoro = EfeitoPulso.CriarSprite("Meteoro", origemQueda, new Vector2(Raio * 0.9f, Raio * 0.9f), CorMeteoro, 60);
        meteoro.transform.parent = transform;

        tempo = 0f;
        while (tempo < TempoQueda)
        {
            tempo += Time.deltaTime;
            float progresso = Mathf.Clamp01(tempo / TempoQueda);
            meteoro.transform.position = Vector3.Lerp(origemQueda, destinoQueda, progresso * progresso);
            yield return null;
        }
        Destroy(meteoro);
        Destroy(aviso);

        // 3) Explosão
        Explodir(chao);

        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }

    void Explodir(Vector3 chao)
    {
        Vector2 centroDano = new Vector2(chao.x, chao.y + AlturaDoImpacto * 0.5f);
        Collider2D[] acertados = Physics2D.OverlapBoxAll(centroDano, new Vector2(Raio * 2f, AlturaDoImpacto), 0f);

        for (int i = 0; i < acertados.Length; i++)
        {
            Personagem jogador = acertados[i].GetComponentInParent<Personagem>();
            if (jogador != null)
            {
                if (!jogador.EstaEmDashAtivo) jogador.ReceberDano(Dano, chao);
                break;
            }
        }

        Color corSumindo = new Color(CorExplosao.r, CorExplosao.g, CorExplosao.b, 0f);

        EfeitoPulso.Criar(chao + Vector3.up * 0.1f,
            new Vector2(Raio * 1.2f, Raio * 0.3f), new Vector2(Raio * 3f, Raio * 0.8f),
            CorExplosao, corSumindo, 0.45f, 61);

        EfeitoPulso.Criar(new Vector3(chao.x, chao.y + AlturaDoImpacto * 0.4f, chao.z),
            new Vector2(Raio, Raio), new Vector2(Raio * 2.4f, AlturaDoImpacto * 1.4f),
            CorExplosao, corSumindo, 0.35f, 62);
    }
}