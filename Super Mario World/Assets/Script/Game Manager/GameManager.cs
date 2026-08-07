using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
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

    // Propriedade pública para outros scripts consultarem se o jogo está pausado
    public bool EstaPausado => estaPausado;

    void Start()
    {
        ConfigurarAudioSources();
        IniciarMusicaFase();
    }

    void Update()
    {
        // Só aceita o comando se a flag podePausar for verdadeira
        if (podePausar && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
        {
            AlternarPause();
        }
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
        MarioController mario = FindObjectOfType<MarioController>();
        if (mario != null)
        {
            mario.enabled = estado;
        }
    }
}
