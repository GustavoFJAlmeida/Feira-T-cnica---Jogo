using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class IntroDeFase : MonoBehaviour
{
    public static bool EmIntro;

    [Header("Imagem cheia de tela (ex: Tela01, Tela02)")]
    public Image ImagemIntro;
    public float TempoDeImagem = 5f;

    [Header("Tela preta (efeito de \"acordando da caverna\")")]
    public Image TelaPreta;
    public float TempoDeFadeOut = 2f;

    [Header("Opcional")]
    public bool PausarJogoDuranteImagem = false; 

    void Start()
    {
        EmIntro = true;

        if (TelaPreta != null)
        {
            Color cor = TelaPreta.color;
            cor.a = 0f;
            TelaPreta.color = cor;
            TelaPreta.gameObject.SetActive(false);
        }

        if (ImagemIntro != null) ImagemIntro.gameObject.SetActive(true);

        if (PausarJogoDuranteImagem) Time.timeScale = 0f;

        StartCoroutine(Rotina());
    }

    IEnumerator Rotina()
    {
        if (PausarJogoDuranteImagem)
            yield return new WaitForSecondsRealtime(TempoDeImagem);
        else
            yield return new WaitForSeconds(TempoDeImagem);

        if (PausarJogoDuranteImagem) Time.timeScale = 1f;

        if (ImagemIntro != null) ImagemIntro.gameObject.SetActive(false);

        if (TelaPreta != null)
        {
            TelaPreta.gameObject.SetActive(true);
            Color cor = TelaPreta.color;
            cor.a = 1f;
            TelaPreta.color = cor;

            float tempo = 0f;
            while (tempo < TempoDeFadeOut)
            {
                tempo += Time.deltaTime;
                cor.a = Mathf.Lerp(1f, 0f, tempo / TempoDeFadeOut);
                TelaPreta.color = cor;
                yield return null;
            }

            cor.a = 0f;
            TelaPreta.color = cor;
            TelaPreta.gameObject.SetActive(false);
        }

        EmIntro = false;
    }
}