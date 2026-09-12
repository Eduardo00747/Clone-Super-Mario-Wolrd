using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
[Header("Referências")]
    [Tooltip("Referência ao script do MarioController.")]
    [SerializeField] private MarioController marioController;

    [Header("Configurações de Áudio")]
    [Tooltip("Música de fundo da fase (ex: Overworld).")]
    [SerializeField] private AudioClip musicaFase;

    [Tooltip("Efeito sonoro ao pausar/despausar o jogo (ex: smw_pause).")]
    [SerializeField] private AudioClip somPause;

    [Header("Componentes de Áudio")]
    [Tooltip("Audio Source dedicado para a música de fundo.")]
    [SerializeField] private AudioSource audioSourceMusica;

    [Tooltip("Audio Source dedicado para os efeitos sonoros.")]
    [SerializeField] private AudioSource audioSourceSFX;

    // Estado interno do pause
    private bool estaPausado = false;

    // Controle de permissão para acionar o botão Enter
    private bool podePausar = true;

    // Controle para garantir que a rotina de morte rode apenas uma vez
    private bool morteProcessada = false;

    // Propriedade pública para outros scripts consultarem se o jogo está pausado
    public bool EstaPausado => estaPausado;

    void Start()
    {
        // Se o MarioController não for atribuído no Inspector, busca na cena
        if (marioController == null)
        {
            marioController = FindObjectOfType<MarioController>();
        }

        ConfigurarAudioSources();
        IniciarMusicaFase();
    }

    void Update()
    {
        // Checa se o Mario morreu e a lógica de morte ainda não foi acionada
        if (marioController != null && marioController.EstaMorto && !morteProcessada)
        {
            morteProcessada = true;
            StartCoroutine(RotinaMorteMario());
            return;
        }

        // Só aceita o comando de pause manual se a flag podePausar for verdadeira e o Mario estiver vivo
        if (podePausar && !morteProcessada && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
        {
            AlternarPause();
        }
    }

    /// <summary>
    /// Corrotina que congela todo o cenário (Time.timeScale = 0) e executa o pulo de morte do Mario em tempo real.
    /// </summary>
    private IEnumerator RotinaMorteMario()
    {
        // 1. Bloqueia o pause manual do jogador
        podePausar = false;

        // 2. Para a música de fundo da fase
        if (audioSourceMusica != null)
        {
            audioSourceMusica.Stop();
        }

        // 3. Freeze Frame clássico do Super Mario World (pausa dramática antes do salto de morte)
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(0.4f);

        // 4. Parâmetros de física em tempo real para mover o Mario com o mundo pausado
        float velocidadeSubida = 12f; // Força inicial para cima
        float gravidade = 35f;        // Gravidade em tempo real
        float tempo = 0f;
        float duracaoMorte = 3f;

        // Anula qualquer velocidade do Rigidbody do Mario para não haver interferências externas
        Rigidbody2D rbMario = marioController.GetComponent<Rigidbody2D>();
        if (rbMario != null)
        {
            rbMario.velocity = Vector2.zero;
        }

        // 5. Move o Mario frame a frame usando Time.unscaledDeltaTime (enquanto todo o resto do jogo está pausado)
        while (tempo < duracaoMorte)
        {
            float dt = Time.unscaledDeltaTime;
            tempo += dt;

            // Calcula a queda/subida
            velocidadeSubida -= gravidade * dt;
            marioController.transform.position += new Vector3(0f, velocidadeSubida * dt, 0f);

            yield return null;
        }

        // 6. Restaura o tempo normal ao reiniciar ou mudar de tela
        Time.timeScale = 1f;

        // Exemplo para reiniciar a fase após a animação de morte:
        // Mudar para a cena "OverWorld""
        SceneManager.LoadScene("OverWorld");
        // UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    private void ConfigurarAudioSources()
    {
        AudioSource[] sources = GetComponents<AudioSource>();

        if (audioSourceMusica == null && sources.Length > 0)
        {
            audioSourceMusica = sources[0];
        }

        if (audioSourceSFX == null && sources.Length > 1)
        {
            audioSourceSFX = sources[1];
        }
    }

    private void IniciarMusicaFase()
    {
        if (audioSourceMusica != null && musicaFase != null)
        {
            audioSourceMusica.clip = musicaFase;
            audioSourceMusica.loop = true;
            audioSourceMusica.playOnAwake = false;
            audioSourceMusica.Play();
        }
    }

    public void AlternarPause()
    {
        estaPausado = !estaPausado;

        if (audioSourceSFX != null && somPause != null)
        {
            audioSourceSFX.PlayOneShot(somPause);
        }

        if (estaPausado)
        {
            Time.timeScale = 0f;
            ControledorMarioHabilitado(false); // Desabilita os comandos do Mario

            if (audioSourceMusica != null)
            {
                audioSourceMusica.Pause();
            }
        }
        else
        {
            Time.timeScale = 1f;
            ControledorMarioHabilitado(true); // Reabilita os comandos do Mario

            if (audioSourceMusica != null)
            {
                audioSourceMusica.UnPause();
            }
        }
    }

    // Método para pausar mantendo a música (usado pela caixa de tutorial)
    public void PausarSemMusica()
    {
        estaPausado = true;
        Time.timeScale = 0f;
        ControledorMarioHabilitado(false); // Desabilita os comandos do Mario
    }

    // Bloqueia a tecla de Enter por um determinado tempo em segundos
    public void BloquearEntradaPause(float tempoDeBloqueio)
    {
        StartCoroutine(BloquearPauseTemporariamente(tempoDeBloqueio));
    }

    private IEnumerator BloquearPauseTemporariamente(float duracao)
    {
        podePausar = false;
        yield return new WaitForSecondsRealtime(duracao);
        podePausar = true;
    }

    // Liga ou desliga o script MarioController
    private void ControledorMarioHabilitado(bool estado)
    {
        if (marioController != null)
        {
            marioController.enabled = estado;
        }
    }
}
