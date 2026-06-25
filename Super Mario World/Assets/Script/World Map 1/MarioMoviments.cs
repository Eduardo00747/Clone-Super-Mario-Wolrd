using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarioMoviments : MonoBehaviour
{
    public enum PathDirection
    {
        Forward,
        Backward
    }

    [Header("Componentes")]
    // Mario
    public Transform marioMap;
    // Animator
    public Animator marioAnimator;
    // Ponto atual onde Mario está parado
    public MapPoint currentPoint;

    [Header("Configurações de Movimento")]
    // Velocidade
    [Tooltip("Tempo que o Mario leva para ir de um ponto ao outro. (Maior = Mais lento)")]
    public float moveDuration = 0.3f;
    private bool isMoving = false;

    [Header("Estados do Terreno (Apenas Leitura)")]
    [SerializeField] private bool estaNaEscada = false;
    [SerializeField] private bool estaNaAgua = false;
    [SerializeField] private string animacaoCaminhadaAtual;

    private void Update()
    {
        if (isMoving)
            return;

        // ESQUERDA
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            TryMove(currentPoint.left, "MarioWalkLeft", PathDirection.Forward);
        }

        // DIREITA
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            TryMove(currentPoint.right, "MarioWalkRight", PathDirection.Backward);
        }

        // CIMA
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            TryMove(currentPoint.up, "MarioUp", PathDirection.Forward);
        }

        // BAIXO
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            TryMove(currentPoint.down, "MarioDown", PathDirection.Backward);
        }
    }

    private void TryMove(MapPoint targetPoint, string walkAnimation, PathDirection direction)
    {
        if (targetPoint == null)
            return;

        StartCoroutine(MoveToFinalPoint(targetPoint, walkAnimation, direction));
    }

    private IEnumerator MoveToFinalPoint(MapPoint startPoint, string walkAnimation, PathDirection direction)
    {
        isMoving = true;
        animacaoCaminhadaAtual = walkAnimation;

        // Sai do Idle
        marioAnimator.SetBool("MarioIdleAnim", false);

        // Se o movimento já começar dentro de um colisor especial (Escada ou Água)
        if (estaNaEscada)
        {
            if (animacaoCaminhadaAtual == "MarioUp") marioAnimator.SetBool("ClimbUp", true);
            else if (animacaoCaminhadaAtual == "MarioDown") marioAnimator.SetBool("ClimbDown", true);
            else marioAnimator.SetBool(walkAnimation, true);
        }
        else if (estaNaAgua)
        {
            if (animacaoCaminhadaAtual == "MarioUp") marioAnimator.SetBool("WaterUp", true);
            else if (animacaoCaminhadaAtual == "MarioDown") marioAnimator.SetBool("WaterDown", true);
            else marioAnimator.SetBool(walkAnimation, true);
        }
        else
        {
            marioAnimator.SetBool(walkAnimation, true);
        }

        MapPoint point = startPoint;

        while (point != null)
        {
            yield return StartCoroutine(MoveMario(marioMap.position, point.transform.position));

            if (point.isFinalPoint)
            {
                currentPoint = point;
                break;
            }

            if (direction == PathDirection.Forward) point = point.nextPoint;
            else point = point.previousPoint;
        }

        // Ao parar, limpa absolutamente todos os parâmetros para evitar travar na pose errada
        marioAnimator.SetBool(walkAnimation, false);
        marioAnimator.SetBool("ClimbUp", false);
        marioAnimator.SetBool("ClimbDown", false);
        marioAnimator.SetBool("WaterUp", false);
        marioAnimator.SetBool("WaterDown", false);
        marioAnimator.SetBool("MarioIdleAnim", true);

        animacaoCaminhadaAtual = null;
        isMoving = false;
    }

    private IEnumerator MoveMario(Vector3 startPos, Vector3 targetPos)
    {
        float elapsedTime = 0f;
        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / moveDuration;
            marioMap.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }
        marioMap.position = targetPos;
    }

    // DETECÇÃO DE COLISÃO (ESCADA E ÁGUA)
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Caso entre na Escada
        if (other.CompareTag("Escada Anim"))
        {
            estaNaEscada = true;
            if (isMoving) TrocarAnimacaoEscada(entrarNaEscada: true);
        }
        
        // Caso entre na Água
        if (other.CompareTag("Agua Anim"))
        {
            estaNaAgua = true;
            if (isMoving) TrocarAnimacaoAgua(entrarNaAgua: true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Caso saia da Escada
        if (other.CompareTag("Escada Anim"))
        {
            estaNaEscada = false;
            if (isMoving) TrocarAnimacaoEscada(entrarNaEscada: false);
        }

        // Caso saia da Água
        if (other.CompareTag("Agua Anim"))
        {
            estaNaAgua = false;
            if (isMoving) TrocarAnimacaoAgua(entrarNaAgua: false);
        }
    }

    // GERENCIADOR DE ANIMAÇÃO: ESCADA
    private void TrocarAnimacaoEscada(bool entrarNaEscada)
    {
        if (string.IsNullOrEmpty(animacaoCaminhadaAtual)) return;

        if (entrarNaEscada)
        {
            marioAnimator.SetBool(animacaoCaminhadaAtual, false);

            if (animacaoCaminhadaAtual == "MarioUp") marioAnimator.SetBool("ClimbUp", true);
            else if (animacaoCaminhadaAtual == "MarioDown") marioAnimator.SetBool("ClimbDown", true);
        }
        else
        {
            marioAnimator.SetBool("ClimbUp", false);
            marioAnimator.SetBool("ClimbDown", false);
            marioAnimator.SetBool(animacaoCaminhadaAtual, true);
        }
    }

    // GERENCIADOR DE ANIMAÇÃO: ÁGUA
    private void TrocarAnimacaoAgua(bool entrarNaAgua)
    {
        if (string.IsNullOrEmpty(animacaoCaminhadaAtual)) return;

        if (entrarNaAgua)
        {
            if (animacaoCaminhadaAtual == "MarioUp")
            {
                marioAnimator.SetBool("MarioUp", false);
                marioAnimator.SetBool("WaterUp", true);
            }
            else if (animacaoCaminhadaAtual == "MarioDown")
            {
                marioAnimator.SetBool("MarioDown", false);
                marioAnimator.SetBool("WaterDown", true);
            }
        }
        else
        {
            marioAnimator.SetBool("WaterUp", false);
            marioAnimator.SetBool("WaterDown", false);
            
            if (animacaoCaminhadaAtual == "MarioUp" || animacaoCaminhadaAtual == "MarioDown")
            {
                marioAnimator.SetBool(animacaoCaminhadaAtual, true);
            }
        }
    }
}
