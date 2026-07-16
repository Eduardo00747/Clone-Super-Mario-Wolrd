using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarioController : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    [Tooltip("Velocidade de movimento horizontal do Mario.")]
    [SerializeField] private float velocidade = 5f;

    [Header("Componentes de Animação")]
    [Tooltip("Arraste o componente Animator do Mario para cá.")]
    public Animator marioAnimator;

    // Componentes internos
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private float movimentoHorizontal;
    private bool estaAbaixado;

    void Start()
    {
        // Pega automaticamente o componente Rigidbody2D anexado ao Mario
        rb = GetComponent<Rigidbody2D>();

        // Pega automaticamente o SpriteRenderer anexado ao Mario
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Lê as entradas do jogador a cada quadro (Frame)
        DetectarEntradas();

        // Controla o estado das animações e para onde o Mario está olhando
        GerenciarAnimacoes();
    }

    void FixedUpdate()
    {
        // Executa a movimentação física de forma constante
        MoverMario();
    }

    private void DetectarEntradas()
    {
        // Reinicia os valores a cada frame
        movimentoHorizontal = 0f;
        estaAbaixado = false;

        // Verifica primeiro se o jogador quer abaixar (Tecla S)
        if (Input.GetKey(KeyCode.S))
        {
            estaAbaixado = true;
            // Retorna imediatamente para não permitir que ele ande enquanto abaixa
            return; 
        }

        // Se não estiver abaixado, permite ler os comandos de andar
        if (Input.GetKey(KeyCode.D))
        {
            movimentoHorizontal = 1f;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            movimentoHorizontal = -1f;
        }
    }

    private void MoverMario()
    {
        // Aplica a velocidade no eixo X, mas mantém a velocidade atual do eixo Y (para a gravidade funcionar)
        rb.velocity = new Vector2(movimentoHorizontal * velocidade, rb.velocity.y);
    }

    private void GerenciarAnimacoes()
    {
        // 1º ESTADO: Se a tecla S estiver pressionada, o Mario abaixa (independente se estava em Idle ou Walk)
        if (estaAbaixado)
        {
            marioAnimator.SetBool("isIdle", false);
            marioAnimator.SetBool("isWalk", false);
            marioAnimator.SetBool("isDown", true);
        }
        // 2º ESTADO: Se movimentoHorizontal for diferente de zero, o Mario está andando
        else if (movimentoHorizontal != 0f)
        {
            marioAnimator.SetBool("isIdle", false);
            marioAnimator.SetBool("isDown", false);
            marioAnimator.SetBool("isWalk", true);

            // Controla o Flip do sprite de acordo com a direção do movimento
            if (movimentoHorizontal > 0f)
            {
                spriteRenderer.flipX = false; // Olha para a direita (padrão)
            }
            else if (movimentoHorizontal < 0f)
            {
                spriteRenderer.flipX = true;  // Dá o flip para olhar para a esquerda
            }
        }
        // 3º ESTADO: Se não estiver abaixado nem andando, o Mario está parado (Idle)
        else
        {
            marioAnimator.SetBool("isWalk", false);
            marioAnimator.SetBool("isDown", false);
            marioAnimator.SetBool("isIdle", true);
        }
    }
}
