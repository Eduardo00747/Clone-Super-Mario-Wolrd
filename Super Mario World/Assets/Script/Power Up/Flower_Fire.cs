using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flower_Fire : MonoBehaviour
{
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Inicia a corrotina para permitir movimento após 0.3 segundos
        StartCoroutine(EnableMovementAfterDelay());
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Verificar se a colisão ocorreu com a tag "Player" ou "Borda"
        if (collision.gameObject.CompareTag("Player"))
        {
            // Destruir o objeto que possui este script (o Cogumelo)
            Destroy(gameObject);
        }

    }

    private IEnumerator EnableMovementAfterDelay()
    {
        // Aguarda 0.3 segundos
        yield return new WaitForSeconds(0.3f);

        // Desativa o isTrigger do Box Collider 2D
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        if (boxCollider != null)
        {
            boxCollider.isTrigger = false;
        }
    }
}
