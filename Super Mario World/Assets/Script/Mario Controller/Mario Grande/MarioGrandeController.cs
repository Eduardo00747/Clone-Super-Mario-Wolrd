using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarioGrandeController : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    [Tooltip("Velocidade normal de caminhada do Mario.")]
    [SerializeField] private float velocidadeAndar = 5f;
    [Tooltip("Velocidade do Mario ao correr (pressionando a tecla de corrida).")]
    [SerializeField] private float velocidadeCorrer = 9f;
    [Tooltip("Força do pulo do Mario.")]
    [SerializeField] private float forcaPulo = 14f;
    [Tooltip("Escala de gravidade do Mario.")]
    [SerializeField] private float escalaGravidade = 3.5f;

    [Header("Configurações de Corrida (Dash/Sprint)")]
    [Tooltip("Tempo necessário segurando a tecla de corrida e se movendo para ativar a animação de corrida rápida.")]
    [SerializeField] private float tempoParaAnimacaoCorrer = 2.5f;

    [Header("Configurações de Derrapagem / Inércia")]
    [Tooltip("Aceleração gradual ao andar/correr (quanto maior, mais rápido atinge a velocidade máxima).")]
    [SerializeField] private float aceleracao = 25f;
    [Tooltip("Fator de desaceleração ao derrapar (menores valores fazem derrapar por mais tempo).")]
    [SerializeField] private float desaceleracaoDerrapagem = 12f;

    [Header("Detecção de Chão")]
    [Tooltip("Transform posicionado nos pés do Mario para checar colisão.")]
    [SerializeField] private Transform detectorChao;
    [Tooltip("Raio da esfera de detecção de chão.")]
    [SerializeField] private float raioDetector = 0.2f;
    [Tooltip("Selecione a Layer que representa o chão do seu cenário.")]
    [SerializeField] private LayerMask layerChao;

    [Header("Componentes de Animação")]
    [Tooltip("Arraste o componente Animator do Mario para cá.")]
    public Animator marioAnimator;

    
    [Header("Efeitos de Áudio")]
    [Tooltip("Som reproduzido ao realizar o pulo normal (Space).")]
    [SerializeField] private AudioClip somPuloNormal;
    [Tooltip("Som reproduzido ao realizar o Spin Attack (O).")]
    [SerializeField] private AudioClip somSpinJump;

    // Componentes internos
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;
    private float movimentoHorizontal;
    private bool estaAbaixado;
    private bool olhandoParaCima;
    private bool estaNoChao;

    // Controle de Corrida e Derrapagem
    private bool estaSegurandoCorrida; // Se está apertando a tecla K
    private bool estaCorrendo;          // Se ativou o modo corrida física
    private bool estaCorrendoRapido;    // Se passou dos 2.5s para trocar a animação
    private bool estaDerrapando;        // Indica se está executando a derrapagem
    private float direcaoDerrapagem = 0f; // Guarda para qual lado o Mario estava virado quando começou a derrapar
    private float cronometroCorrida = 0f;
    public bool EstaDerrapando => estaDerrapando;
    public float DirecaoDerrapagem => direcaoDerrapagem;

    // Controle do Spin Attack / Spin Jump
    private bool estaAtacando; // Controla se o pulo atual é um Spin Jump

    void Start()
    {
        // Pega automaticamente o componente Rigidbody2D anexado ao Mario
        rb = GetComponent<Rigidbody2D>();

        // Pega automaticamente o SpriteRenderer anexado ao Mario
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Pega automaticamente o AudioSource anexado ao Mario
        audioSource = GetComponent<AudioSource>();

        // Aplica a escala de gravidade definida no Inspector diretamente no Rigidbody2D
        rb.gravityScale = escalaGravidade;
    }

    void Update()
    {
        // Checa se o Mario está tocando o chão a cada frame
        ChecarChao();

        // Lê as entradas do jogador a cada quadro (Frame)
        DetectarEntradas();

        // Gerencia a contagem de tempo para a corrida
        GerenciarTimerCorrida();

        // Checa se o jogador trocou bruscamente de direção para derrapar
        ChecarDerrapagem();

        // Controla o estado das animações e para onde o Mario está olhando
        GerenciarAnimacoes();
    }

    void FixedUpdate()
    {
        // Executa a movimentação física de forma constante
        MoverMario();
    }

    private void ChecarChao()
    {
        if (detectorChao != null)
        {
            // Cria um círculo invisível na posição do "detectorChao" e checa se ele bateu em algo com a Layer do chão
            bool estavaNoChaoAnteriormente = estaNoChao;
            estaNoChao = Physics2D.OverlapCircle(detectorChao.position, raioDetector, layerChao);

            // Se o Mario acabou de tocar o solo vindo do ar, cancela o estado de Spin Attack
            if (estaNoChao && !estavaNoChaoAnteriormente)
            {
                estaAtacando = false;
            }
        }
    }

    private void DetectarEntradas()
    {
        // Reinicia os valores de movimento horizontal a cada frame
        movimentoHorizontal = 0f;
        estaAbaixado = false;
        olhandoParaCima = false;

        // Detecta se a tecla de corrida (K) está sendo mantida pressionada
        estaSegurandoCorrida = Input.GetKey(KeyCode.K);

        // 1. LÓGICA DO PULO NORMAL (Espaço): Permite pular se estiver no chão
        if (estaNoChao && Input.GetKeyDown(KeyCode.Space))
        {
            estaAtacando = false;
            rb.velocity = new Vector2(rb.velocity.x, forcaPulo);

            // Toca o som do pulo normal
            TocarSom(somPuloNormal);
        }

        // 2. LÓGICA DO SPIN ATTACK / SPIN JUMP (Tecla O): Pulo especial giratório
        if (estaNoChao && Input.GetKeyDown(KeyCode.O))
        {
            estaAtacando = true;
            rb.velocity = new Vector2(rb.velocity.x, forcaPulo);

            // Toca o som do Spin Jump
            TocarSom(somSpinJump);
        }

        // 3. Verifica se o jogador quer abaixar (Tecla S) - Só faz sentido abaixar no chão
        if (estaNoChao && Input.GetKey(KeyCode.S))
        {
            estaAbaixado = true;

            if (Input.GetKey(KeyCode.D))
            {
                spriteRenderer.flipX = false;
            }
            else if (Input.GetKey(KeyCode.A))
            {
                spriteRenderer.flipX = true;
            }

            return; 
        }

        // 4. Verifica se o jogador quer olhar para cima (Tecla W) - Só faz sentido no chão
        if (estaNoChao && Input.GetKey(KeyCode.W))
        {
            olhandoParaCima = true;

            if (Input.GetKey(KeyCode.D))
            {
                spriteRenderer.flipX = false;
            }
            else if (Input.GetKey(KeyCode.A))
            {
                spriteRenderer.flipX = true;
            }

            return;
        }

        // 5. Se não estiver abaixado nem olhando para cima, permite andar/correr
        if (Input.GetKey(KeyCode.D))
        {
            movimentoHorizontal = 1f;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            movimentoHorizontal = -1f;
        }
    }

    private void TocarSom(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    private void GerenciarTimerCorrida()
    {
        // Só conta o tempo de corrida se o jogador estiver se movendo e segurando a tecla K
        if (movimentoHorizontal != 0f && estaSegurandoCorrida)
        {
            estaCorrendo = true;
            cronometroCorrida += Time.deltaTime;

            // Se atingir ou ultrapassar 2.5s, ativa a animação especial de corrida rápida
            if (cronometroCorrida >= tempoParaAnimacaoCorrer)
            {
                estaCorrendoRapido = true;
            }
        }
        else
        {
            // Se parar de andar ou soltar a tecla K, reseta o temporizador e os estados de corrida
            estaCorrendo = false;
            estaCorrendoRapido = false;
            cronometroCorrida = 0f;
        }
    }

    private void ChecarDerrapagem()
    {
        // Só derrapa se estiver no chão e correndo (ou com velocidade acumulada alta)
        if (estaNoChao && (estaCorrendo || Mathf.Abs(rb.velocity.x) > velocidadeAndar))
        {
            // Se movendo rápido para a DIREITA e pressionando para a ESQUERDA
            if (rb.velocity.x > 1.5f && movimentoHorizontal < 0f)
            {
                if (!estaDerrapando)
                {
                    estaDerrapando = true;
                    direcaoDerrapagem = 1f; // Estava indo para a direita
                }
            }
            // Se movendo rápido para a ESQUERDA e pressionando para a DIREITA
            else if (rb.velocity.x < -1.5f && movimentoHorizontal > 0f)
            {
                if (!estaDerrapando)
                {
                    estaDerrapando = true;
                    direcaoDerrapagem = -1f; // Estava indo para a esquerda
                }
            }
            else if (Mathf.Abs(rb.velocity.x) <= 0.5f)
            {
                // A velocidade zerou, finaliza a derrapagem
                estaDerrapando = false;
            }
        }
        else
        {
            estaDerrapando = false;
        }
    }

    private void MoverMario()
    {
        float velocidadeAlvo = movimentoHorizontal * (estaCorrendo ? velocidadeCorrer : velocidadeAndar);

        if (estaDerrapando)
        {
            // Durante a derrapagem, aplica uma desaceleração gradativa até o Mario parar no chão antes de inverter o sentido
            float velocidadeNovaX = Mathf.MoveTowards(rb.velocity.x, 0f, desaceleracaoDerrapagem * Time.fixedDeltaTime);
            rb.velocity = new Vector2(velocidadeNovaX, rb.velocity.y);
        }
        else
        {
            // Movimento normal com aceleração/transição física suave
            float velocidadeNovaX = Mathf.MoveTowards(rb.velocity.x, velocidadeAlvo, aceleracao * Time.fixedDeltaTime);
            rb.velocity = new Vector2(velocidadeNovaX, rb.velocity.y);
        }
    }

    private void GerenciarAnimacoes()
    {
        // 1º ESTADO (Prioridade Máxima): Se NÃO estiver no chão, gerencia as animações no ar
        if (!estaNoChao)
        {
            marioAnimator.SetBool("isIdle", false);
            marioAnimator.SetBool("isWalk", false);
            marioAnimator.SetBool("isRun", false);
            marioAnimator.SetBool("isDown", false);
            marioAnimator.SetBool("lookUp", false);
            marioAnimator.SetBool("isSkid", false);

            if (estaAtacando)
            {
                // Ativa a animação MarioSpinAtackAnim via o parâmetro "isAtack"
                marioAnimator.SetBool("isJump", false);
                marioAnimator.SetBool("isJumpRun", false);
                marioAnimator.SetBool("isAtack", true);
            }
            else
            {
                marioAnimator.SetBool("isAtack", false);

                if (estaCorrendoRapido || (estaSegurandoCorrida && movimentoHorizontal != 0f))
                {
                    marioAnimator.SetBool("isJump", false);
                    marioAnimator.SetBool("isJumpRun", true);
                }
                else
                {
                    marioAnimator.SetBool("isJumpRun", false);
                    marioAnimator.SetBool("isJump", true);
                }
            }

            // Permite mudar a direção do olhar enquanto estiver no ar
            if (movimentoHorizontal > 0f)
            {
                spriteRenderer.flipX = false;
            }
            else if (movimentoHorizontal < 0f)
            {
                spriteRenderer.flipX = true;
            }
        }
        // 2º ESTADO: Se estiver DERRAPANDO no chão
        else if (estaDerrapando)
        {
            marioAnimator.SetBool("isIdle", false);
            marioAnimator.SetBool("isWalk", false);
            marioAnimator.SetBool("isRun", false);
            marioAnimator.SetBool("isDown", false);
            marioAnimator.SetBool("lookUp", false);
            marioAnimator.SetBool("isJump", false);
            marioAnimator.SetBool("isJumpRun", false);
            marioAnimator.SetBool("isAtack", false);
            marioAnimator.SetBool("isSkid", true);

            // Mantém o sprite virado para o lado em que ele já estava deslizando
            if (direcaoDerrapagem > 0f)
            {
                spriteRenderer.flipX = false;
            }
            else if (direcaoDerrapagem < 0f)
            {
                spriteRenderer.flipX = true;
            }
        }
        // 3º ESTADO: Se estiver no chão e a tecla S estiver pressionada, o Mario abaixa
        else if (estaAbaixado)
        {
            marioAnimator.SetBool("isIdle", false);
            marioAnimator.SetBool("isWalk", false);
            marioAnimator.SetBool("isRun", false);
            marioAnimator.SetBool("lookUp", false);
            marioAnimator.SetBool("isJump", false);
            marioAnimator.SetBool("isJumpRun", false);
            marioAnimator.SetBool("isAtack", false);
            marioAnimator.SetBool("isSkid", false);
            marioAnimator.SetBool("isDown", true);
        }
        // 4º ESTADO: Se estiver no chão e a tecla W estiver pressionada, o Mario olha para cima
        else if (olhandoParaCima)
        {
            marioAnimator.SetBool("isIdle", false);
            marioAnimator.SetBool("isWalk", false);
            marioAnimator.SetBool("isRun", false);
            marioAnimator.SetBool("isDown", false);
            marioAnimator.SetBool("isJump", false);
            marioAnimator.SetBool("isJumpRun", false);
            marioAnimator.SetBool("isAtack", false);
            marioAnimator.SetBool("isSkid", false);
            marioAnimator.SetBool("lookUp", true);
        }
        // 5º ESTADO: Movimento em Solo (Andar ou Correr)
        else if (movimentoHorizontal != 0f)
        {
            marioAnimator.SetBool("isIdle", false);
            marioAnimator.SetBool("isDown", false);
            marioAnimator.SetBool("lookUp", false);
            marioAnimator.SetBool("isJump", false);
            marioAnimator.SetBool("isJumpRun", false);
            marioAnimator.SetBool("isAtack", false);
            marioAnimator.SetBool("isSkid", false);

            if (estaCorrendoRapido)
            {
                marioAnimator.SetBool("isWalk", false);
                marioAnimator.SetBool("isRun", true);
            }
            else
            {
                marioAnimator.SetBool("isRun", false);
                marioAnimator.SetBool("isWalk", true);
            }

            if (movimentoHorizontal > 0f)
            {
                spriteRenderer.flipX = false;
            }
            else if (movimentoHorizontal < 0f)
            {
                spriteRenderer.flipX = true;
            }
        }
        // 6º ESTADO: Parado no chão (Idle)
        else
        {
            marioAnimator.SetBool("isWalk", false);
            marioAnimator.SetBool("isRun", false);
            marioAnimator.SetBool("isDown", false);
            marioAnimator.SetBool("lookUp", false);
            marioAnimator.SetBool("isJump", false);
            marioAnimator.SetBool("isJumpRun", false);
            marioAnimator.SetBool("isAtack", false);
            marioAnimator.SetBool("isSkid", false);
            marioAnimator.SetBool("isIdle", true);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (detectorChao != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(detectorChao.position, raioDetector);
        }
    }
}
