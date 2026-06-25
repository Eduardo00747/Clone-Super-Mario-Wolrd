using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovimentAnim : MonoBehaviour
{
    // Mario que será movimentado
    public Transform marioMap;

    // Ponto inicial
    public Transform ponto0;

    // Ponto final
    public Transform ponto1;

    // Tempo que leva para se mover
    public float moveDuration = 1f;

    public Animator marioAnimator;

    private void Start()
    {
        // Garante que começa no ponto 0
        marioMap.position = ponto0.position;

        // Inicia a animação
        StartCoroutine(MoveMario());
    }

    private IEnumerator MoveMario()
    {
        // Espera 0.2 segundos
        yield return new WaitForSeconds(0.2f);

        Vector3 startPosition = ponto0.position;
        Vector3 targetPosition = ponto1.position;

        float elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;

            float t = elapsedTime / moveDuration;

            marioMap.position = Vector3.Lerp(
                startPosition,
                targetPosition,
                t
            );

            yield return null;
        }

        // Garante posição final exata
        marioMap.position = targetPosition;

        // Ativa animação Idle
        marioAnimator.SetBool("MarioIdleAnim", true);
    }
}
