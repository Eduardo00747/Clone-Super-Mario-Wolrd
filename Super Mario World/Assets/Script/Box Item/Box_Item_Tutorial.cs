using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box_Item_Tutorial : MonoBehaviour
{
private Rigidbody2D rb;
    public float forcaDeRecuo = 5f;
    public float tempoDeEspera = 2f; // Tempo de espera antes de resetar a posição
    private SpriteRenderer spriteRenderer;
    private Vector2 startPosition; // Posição inicial da caixa
    [SerializeField] private bool isRecuando = false; // Flag para verificar se a caixa está em movimento

    [Header("Configurações do Tutorial")]
    [Tooltip("Tempo em segundos após a colisão para o jogo pausar.")]
    [SerializeField] private float tempoAtePausar = 0.2f;

    [Header("Configurações da Caixa de Texto (UI)")]
    [Tooltip("Arraste o RectTransform da caixa de texto (Pai do TextMeshPro) para cá.")]
    [SerializeField] private RectTransform textBox;

    [Tooltip("Duração da animação de abertura do texto.")]
    [SerializeField] private float animationDuration = 0.5f;

    [Tooltip("Duração da animação de fechamento do texto.")]
    [SerializeField] private float closeAnimationDuration = 0.5f;

    [Tooltip("Tempo adicional em segundos após a animação antes de liberar os botões de despause.")]
    [SerializeField] private float tempoAdicionalBloqueio = 0.5f;

    [Tooltip("Escala final que a caixa de texto deve alcançar.")]
    [SerializeField] private Vector3 targetScale = new Vector3(5.987126f, 4.238373f, 4.238373f);

    private Coroutine coroutineAnimacaoTexto;
    private bool textoEstaAberto = false;
    private bool podeDespausar = false; // Controla se Space e O já podem ser usados para despausar

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startPosition = rb.position;
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Garante que a caixa de texto comece invisível (com escala zero)
        if (textBox != null)
        {
            textBox.localScale = Vector3.zero;
        }
    }

    void Update()
    {
        GameManager manager = FindObjectOfType<GameManager>();

        if (manager == null) return;

        // Checa se o texto do tutorial está visível e o jogo pausado
        if (textoEstaAberto && manager.EstaPausado)
        {
            // Se já passou o tempo de bloqueio e o jogador apertar Space ou O
            if (podeDespausar && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.O)))
            {
                podeDespausar = false;
                manager.AlternarPause(); // Despausa o jogo via GameManager
            }
        }
        // Se o jogo for despausado (seja por Enter, Space ou O), roda o fechamento do balão
        else if (textBox != null && textoEstaAberto && !manager.EstaPausado)
        {
            if (coroutineAnimacaoTexto != null)
            {
                StopCoroutine(coroutineAnimacaoTexto);
            }
            coroutineAnimacaoTexto = StartCoroutine(HideTextBox());
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isRecuando)
        {
            // Verifique se o impacto foi na parte inferior
            ContactPoint2D contact = collision.contacts[0];
            if (contact.normal.y > 0.5f)
            {
                // Mova a caixa para cima usando MovePosition
                Vector2 newPosition = rb.position + Vector2.up * forcaDeRecuo * Time.fixedDeltaTime;
                rb.MovePosition(newPosition);

                // Inicia o pause do jogo após 0.2s mantendo a música de fundo
                StartCoroutine(PausarJogoComDelay());

                // Inicia a coroutine para resetar a posição da caixa
                StartCoroutine(ResetPositionAfterDelay());
            }
        }
    }

    private IEnumerator PausarJogoComDelay()
    {
        // Espera 0.2 segundos antes de congelar a física
        yield return new WaitForSeconds(tempoAtePausar);

        // Pausa o jogo mantendo a música através do GameManager
        GameManager manager = FindObjectOfType<GameManager>();
        if (manager != null)
        {
            manager.PausarSemMusica();

            // Bloqueia a tecla Enter durante a animação + tempo adicional (0.5s)
            float tempoTotalBloqueio = animationDuration + tempoAdicionalBloqueio;
            manager.BloquearEntradaPause(tempoTotalBloqueio);

            // Inicia o tempo de trava interno para Space e O
            StartCoroutine(AguardarBloqueioBotoes(tempoTotalBloqueio));
        }

        // Inicia a animação de abertura da caixa de texto
        if (textBox != null)
        {
            if (coroutineAnimacaoTexto != null)
            {
                StopCoroutine(coroutineAnimacaoTexto);
            }
            coroutineAnimacaoTexto = StartCoroutine(ShowTextBox());
        }
    }

    private IEnumerator AguardarBloqueioBotoes(float tempoBloqueio)
    {
        podeDespausar = false;
        yield return new WaitForSecondsRealtime(tempoBloqueio);
        podeDespausar = true;
    }

    private IEnumerator ShowTextBox()
    {
        textoEstaAberto = true;
        Vector3 startScale = textBox.localScale;
        float elapsedTime = 0f;

        // Utiliza Time.unscaledDeltaTime pois o jogo está pausado (Time.timeScale = 0)
        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;

            float t = elapsedTime / animationDuration;

            textBox.localScale = Vector3.Lerp(
                startScale,
                targetScale,
                t
            );

            yield return null;
        }

        // Garante o valor final exato
        textBox.localScale = targetScale;
    }

    private IEnumerator HideTextBox()
    {
        Vector3 currentScale = textBox.localScale;
        float elapsedTime = 0f;

        // Como o jogo já foi despausado, utiliza Time.unscaledDeltaTime
        while (elapsedTime < closeAnimationDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;

            float t = elapsedTime / closeAnimationDuration;

            textBox.localScale = Vector3.Lerp(
                currentScale,
                Vector3.zero,
                t
            );

            yield return null;
        }

        // Garante que a escala termine exatamente zerada
        textBox.localScale = Vector3.zero;
        textoEstaAberto = false;
    }

    IEnumerator ResetPositionAfterDelay()
    {
        isRecuando = true;

        // Usa WaitForSecondsRealtime para contar o tempo real com o jogo pausado
        yield return new WaitForSecondsRealtime(tempoDeEspera);

        // Resetar a posição da caixa para a posição inicial
        rb.MovePosition(startPosition);

        // Reinicia a flag de movimento
        isRecuando = false;
    }
}
