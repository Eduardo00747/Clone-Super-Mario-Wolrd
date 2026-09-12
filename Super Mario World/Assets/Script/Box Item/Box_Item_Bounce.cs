using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box_Item_Bounce : MonoBehaviour
{
    private Rigidbody2D rb;
    public float forcaDeRecuo = 5f;
    public float tempoDeEspera = 2f; // Tempo de espera antes de resetar a posição
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Vector2 startPosition; // Posição inicial da caixa
    private bool isRecuando = false; // Flag para verificar se a caixa está em movimento
    private bool mecanicaExecutada = false; // Flag para verificar se a mecânica já foi executada

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startPosition = rb.position;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator.SetBool("Break_Box", false); // Ativa a animação
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isRecuando && !mecanicaExecutada)
        {
            // Verifique se o impacto foi na parte inferior
            ContactPoint2D contact = collision.contacts[0];
            if (contact.normal.y > 0.5f)
            {
                // Mova a caixa para cima usando MovePosition
                Vector2 newPosition = rb.position + Vector2.up * forcaDeRecuo * Time.fixedDeltaTime;
                rb.MovePosition(newPosition);

                // Inicia a coroutine para resetar a posição após o tempo de espera
                StartCoroutine(ResetPositionAfterDelay());

                // Define a flag indicando que a mecânica foi executada
                mecanicaExecutada = true;
            }
        }
    }

    IEnumerator ResetPositionAfterDelay()
    {
        isRecuando = true;
        yield return new WaitForSeconds(tempoDeEspera);

        // Resetar a posição da caixa para a posição inicial
        rb.MovePosition(startPosition);

        // Reinicia a flag de movimento
        isRecuando = false;

        animator.SetBool("Break_Box", true); // Ativa a animação
    }
}
