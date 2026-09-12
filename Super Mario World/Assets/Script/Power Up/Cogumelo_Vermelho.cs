using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cogumelo_Vermelho : MonoBehaviour
{
    public float moveSpeed = 2f; // Velocidade inicial de movimento
    private int direction = 1; // Direção inicial (1 para direita, -1 para esquerda)
    private Rigidbody2D rb;
    private bool canMove = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Inicia a corrotina para permitir movimento após 0.3 segundos
        StartCoroutine(EnableMovementAfterDelay());
    }

    void Update()
    {
        // Move o cogumelo na direção atual
        if (canMove)
        {
            rb.velocity = new Vector2(moveSpeed * direction, rb.velocity.y);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Verificar se a colisão ocorreu com a tag "Player" ou "Borda"
        if (collision.gameObject.CompareTag("Player"))
        {
            // Destruir o objeto que possui este script (o Cogumelo)
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Borda"))
        {
            // Inverte a direção quando colide com a tag "Borda"
            direction *= -1;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (canMove && collision.CompareTag("Barricada"))
        {
            // Inverte a direção quando colide com a tag "Barricada"
            direction *= -1;
        }
    }

    private IEnumerator EnableMovementAfterDelay()
    {
        // Aguarda 0.3 segundos
        yield return new WaitForSeconds(0.3f);

        // Permite o movimento
        canMove = true;

        // Desativa o isTrigger do Box Collider 2D
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider != null)
        {
            boxCollider.isTrigger = false;
        }
    }
}
