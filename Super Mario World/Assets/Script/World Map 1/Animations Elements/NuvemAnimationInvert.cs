using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NuvemAnimationInvert : MonoBehaviour
{
    [Header("Configurações de Velocidade (Tempo)")]
    [Tooltip("Tempo (em segundos) entre cada passo. Quanto MENOR o valor, MAIS RÁPIDO a nuvem se move.")]
    [SerializeField] private float tempoEntrePassos = 0.5f;

    [Header("Configurações de Distância")]
    [Tooltip("O quanto a nuvem vai se deslocar (teletransportar) em cada passo.")]
    [SerializeField] private float distanciaDoPasso = 0.5f;

    [Tooltip("Quantidade de passos que ela dará para cada lado.")]
    [SerializeField] private int quantidadeDePassos = 4;

    void Start()
    {
        // Inicia a rotina infinita de movimentação da nuvem
        StartCoroutine(RotinaDaNuvem());
    }

    private IEnumerator RotinaDaNuvem()
    {
        // Loop infinito para a nuvem ficar indo e voltando o jogo todo
        while (true)
        {
            // 1. Move para a ESQUERDA
            for (int i = 0; i < quantidadeDePassos; i++)
            {
                // Move a nuvem instantaneamente para a esquerda
                transform.position -= new Vector3(distanciaDoPasso, 0, 0);
                
                // Espera o tempo definido no Inspector antes de dar o próximo passo
                yield return new WaitForSeconds(tempoEntrePassos);
            }

            // 2. Move para a DIREITA
            for (int i = 0; i < quantidadeDePassos; i++)
            {
                // Move a nuvem instantaneamente para a direita
                transform.position += new Vector3(distanciaDoPasso, 0, 0);
                
                // Espera o tempo definido no Inspector antes de dar o próximo passo
                yield return new WaitForSeconds(tempoEntrePassos);
            }
        }
    }
}
