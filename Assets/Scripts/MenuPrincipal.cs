using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuPrincipal : MonoBehaviour
{
    public string CenaPrimeiraFase = "Fase1";
    public string CenaLeaderboard = "Leaderboard";
    public string CenaMenu = "Menu";
    public Text TextoDificuldade; // opcional: mostra "Dificuldade: Normal"

    void Start()
    {
        Time.timeScale = 1f;
        AtualizarTexto();
    }

    public void JogarNormal()
    {
        EscolherNormal();
        Jogar();
    }

    public void JogarPaiDeFamilia()
    {
        EscolherPaiDeFamilia();
        Jogar();
    }

    public void Jogar()
    {
        GerenciadorPontuacao.Instancia.NovaPartida();
        SceneManager.LoadScene(CenaPrimeiraFase);
    }

    public void EscolherNormal()
    {
        ConfiguracaoJogo.DificuldadeAtual = Dificuldade.Normal;
        AtualizarTexto();
    }

    public void EscolherPaiDeFamilia()
    {
        ConfiguracaoJogo.DificuldadeAtual = Dificuldade.PaiDeFamilia;
        AtualizarTexto();
    }

    public void AlternarDificuldade()
    {
        if (ConfiguracaoJogo.ModoPaiDeFamilia) EscolherNormal();
        else EscolherPaiDeFamilia();
    }

    public void AbrirLeaderboard()
    {
        SceneManager.LoadScene(CenaLeaderboard);
    }

    public void VoltarAoMenu()
    {
        SceneManager.LoadScene(CenaMenu);
    }

    public void Sair()
    {
        Application.Quit();
    }

    void AtualizarTexto()
    {
        if (TextoDificuldade != null) TextoDificuldade.text = "Dificuldade: " + ConfiguracaoJogo.NomeDificuldade;
    }
}
