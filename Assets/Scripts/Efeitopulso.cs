using UnityEngine;

public class EfeitoPulso : MonoBehaviour
{
    static Sprite spriteCirculo;

    Vector2 tamanhoInicial;
    Vector2 tamanhoFinal;
    Color corInicial;
    Color corFinal;
    float duracao = 0.5f;
    float tempo;
    SpriteRenderer render;

    public static Sprite SpriteCirculo()
    {
        if (spriteCirculo == null) spriteCirculo = CriarSpriteCirculo(64);
        return spriteCirculo;
    }

    static Sprite CriarSpriteCirculo(int tamanho)
    {
        Texture2D textura = new Texture2D(tamanho, tamanho, TextureFormat.RGBA32, false);
        textura.wrapMode = TextureWrapMode.Clamp;
        textura.filterMode = FilterMode.Bilinear;

        float raio = tamanho * 0.5f;
        Color[] pixels = new Color[tamanho * tamanho];
        for (int y = 0; y < tamanho; y++)
        {
            for (int x = 0; x < tamanho; x++)
            {
                float dx = x + 0.5f - raio;
                float dy = y + 0.5f - raio;
                float distancia = Mathf.Sqrt(dx * dx + dy * dy);
                float alfa = Mathf.Clamp01((raio - distancia) / 2f);
                pixels[y * tamanho + x] = new Color(1f, 1f, 1f, alfa);
            }
        }
        textura.SetPixels(pixels);
        textura.Apply();

        return Sprite.Create(textura, new Rect(0, 0, tamanho, tamanho), new Vector2(0.5f, 0.5f), tamanho);
    }

    public static GameObject CriarSprite(string nome, Vector3 posicao, Vector2 tamanho, Color cor, int ordem)
    {
        GameObject objeto = new GameObject(nome);
        objeto.transform.position = posicao;
        objeto.transform.localScale = new Vector3(tamanho.x, tamanho.y, 1f);

        SpriteRenderer sr = objeto.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteCirculo();
        sr.color = cor;
        sr.sortingOrder = ordem;
        return objeto;
    }

    public static EfeitoPulso Criar(Vector3 posicao, Vector2 tamIni, Vector2 tamFim, Color corIni, Color corFim, float tempoTotal, int ordem)
    {
        GameObject objeto = CriarSprite("EfeitoPulso", posicao, tamIni, corIni, ordem);
        EfeitoPulso efeito = objeto.AddComponent<EfeitoPulso>();
        efeito.tamanhoInicial = tamIni;
        efeito.tamanhoFinal = tamFim;
        efeito.corInicial = corIni;
        efeito.corFinal = corFim;
        efeito.duracao = tempoTotal;
        return efeito;
    }

    void Awake()
    {
        render = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        tempo += Time.deltaTime;
        float t = duracao > 0.0001f ? Mathf.Clamp01(tempo / duracao) : 1f;

        Vector2 tamanho = Vector2.Lerp(tamanhoInicial, tamanhoFinal, t);
        transform.localScale = new Vector3(tamanho.x, tamanho.y, 1f);
        if (render != null) render.color = Color.Lerp(corInicial, corFinal, t);

        if (t >= 1f) Destroy(gameObject);
    }
}