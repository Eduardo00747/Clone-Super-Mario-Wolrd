using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkidDetect : MonoBehaviour
{
[Header("Poeiras do Lado Esquerdo")]
    [Tooltip("Poeira 1 (Ativa primeiro)")]
    [SerializeField] private GameObject poeira1;
    [Tooltip("Poeira 2 (Ativa segundo)")]
    [SerializeField] private GameObject poeira2;
    [Tooltip("Poeira 3 (Ativa terceiro)")]
    [SerializeField] private GameObject poeira3;

    [Header("Poeiras do Lado Direito")]
    [Tooltip("Poeira 4 (Ativa primeiro)")]
    [SerializeField] private GameObject poeira4;
    [Tooltip("Poeira 5 (Ativa segundo)")]
    [SerializeField] private GameObject poeira5;
    [Tooltip("Poeira 6 (Ativa terceiro)")]
    [SerializeField] private GameObject poeira6;

    [Header("Configurações do Efeito")]
    [Tooltip("Tempo em segundos entre a ativação de cada poeira.")]
    [SerializeField] private float tempoEntrePoeiras = 0.08f;

    [Header("Estados de Derrapagem (Apenas Leitura)")]
    [SerializeField] private bool derrapagemEsquerda;
    [SerializeField] private bool derrapagemDireita;

    // Propriedades públicas para outros scripts lerem
    public bool DerrapagemEsquerda => derrapagemEsquerda;
    public bool DerrapagemDireita => derrapagemDireita;

    private MarioController marioController;
    
    // Controle das Coroutines para evitar execuções duplicadas
    private Coroutine coroutineEsquerda;
    private Coroutine coroutineDireita;

    void Start()
    {
        // Busca a referência do MarioController no objeto pai
        marioController = GetComponentInParent<MarioController>();

        if (marioController == null)
        {
            Debug.LogError("SkidDetect: MarioController não foi encontrado no objeto pai!");
        }

        // Garante que todas as poeiras comecem desativadas
        DesativarTodasPoeiras();
    }

    void Update()
    {
        if (marioController == null) return;

        ChecarDerrapagemPoeira();
    }

    private void ChecarDerrapagemPoeira()
    {
        bool estavaDerrapandoEsquerda = derrapagemEsquerda;
        bool estavaDerrapandoDireita = derrapagemDireita;

        // Se o Mario estiver derrapando
        if (marioController.EstaDerrapando)
        {
            // direcaoDerrapagem > 0 indica que ele estava indo para a DIREITA e trocou para a esquerda
            if (marioController.DirecaoDerrapagem > 0f)
            {
                derrapagemEsquerda = true;
                derrapagemDireita = false;
            }
            // direcaoDerrapagem < 0 indica que ele estava indo para a ESQUERDA e trocou para a direita
            else if (marioController.DirecaoDerrapagem < 0f)
            {
                derrapagemEsquerda = false;
                derrapagemDireita = true;
            }
        }
        else
        {
            // Se não está derrapando, zera ambos os estados
            derrapagemEsquerda = false;
            derrapagemDireita = false;
        }

        // LÓGICA DO LADO ESQUERDO
        if (derrapagemEsquerda && !estavaDerrapandoEsquerda)
        {
            // Inicia o ciclo sequencial de poeira 1, 2 e 3
            if (coroutineEsquerda != null) StopCoroutine(coroutineEsquerda);
            coroutineEsquerda = StartCoroutine(SequenciaPoeiraEsquerda());
        }
        else if (!derrapagemEsquerda && estavaDerrapandoEsquerda)
        {
            // Cancela a rotina e desativa as poeiras esquerdas
            if (coroutineEsquerda != null) StopCoroutine(coroutineEsquerda);
            DesativarPoeirasEsquerda();
        }

        // LÓGICA DO LADO DIREITO
        if (derrapagemDireita && !estavaDerrapandoDireita)
        {
            // Inicia o ciclo sequencial de poeira 4, 5 e 6
            if (coroutineDireita != null) StopCoroutine(coroutineDireita);
            coroutineDireita = StartCoroutine(SequenciaPoeiraDireita());
        }
        else if (!derrapagemDireita && estavaDerrapandoDireita)
        {
            // Cancela a rotina e desativa as poeiras direitas
            if (coroutineDireita != null) StopCoroutine(coroutineDireita);
            DesativarPoeirasDireita();
        }
    }

    // Rotina que ativa Poeira 1 -> Poeira 2 -> Poeira 3 em ordem
    private IEnumerator SequenciaPoeiraEsquerda()
    {
        if (poeira1 != null) poeira1.SetActive(true);
        yield return new WaitForSeconds(tempoEntrePoeiras);

        if (poeira2 != null) poeira2.SetActive(true);
        yield return new WaitForSeconds(tempoEntrePoeiras);

        if (poeira3 != null) poeira3.SetActive(true);
    }

    // Rotina que ativa Poeira 4 -> Poeira 5 -> Poeira 6 em ordem
    private IEnumerator SequenciaPoeiraDireita()
    {
        if (poeira4 != null) poeira4.SetActive(true);
        yield return new WaitForSeconds(tempoEntrePoeiras);

        if (poeira5 != null) poeira5.SetActive(true);
        yield return new WaitForSeconds(tempoEntrePoeiras);

        if (poeira6 != null) poeira6.SetActive(true);
    }

    private void DesativarPoeirasEsquerda()
    {
        if (poeira1 != null) poeira1.SetActive(false);
        if (poeira2 != null) poeira2.SetActive(false);
        if (poeira3 != null) poeira3.SetActive(false);
    }

    private void DesativarPoeirasDireita()
    {
        if (poeira4 != null) poeira4.SetActive(false);
        if (poeira5 != null) poeira5.SetActive(false);
        if (poeira6 != null) poeira6.SetActive(false);
    }

    private void DesativarTodasPoeiras()
    {
        DesativarPoeirasEsquerda();
        DesativarPoeirasDireita();
    }
}
