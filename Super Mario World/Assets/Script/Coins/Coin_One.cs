using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin_One : MonoBehaviour
{
    void Start()
    {
        // Chama a função DestroyCoin após 0.3 segundos
        Invoke("DestroyCoin", 0.7f);
    }

    // Função para destruir o objeto
    void DestroyCoin()
    {
        // Destroi o objeto atual
        Destroy(gameObject);
    }
}
