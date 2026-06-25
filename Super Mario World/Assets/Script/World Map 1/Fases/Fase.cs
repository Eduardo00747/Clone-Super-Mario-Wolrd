using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Fase : MonoBehaviour
{
    [Header("Configurações de Transição")]
    [Tooltip("Nome exato da cena (fase) para onde o Mario vai.")]
    [SerializeField] private string nomeDaCena;

    [Header("Status de detecção")]
    [SerializeField] private bool marioEstaNaFase = false;

    // Detecta quando o Mario entra na área da fase
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica se o objeto que entrou tem a tag "Player" ou o nome "Mario"
        if (other.CompareTag("MarioMap") || other.name == "Mario")
        {
            marioEstaNaFase = true;
        }
    }

    // Detecta quando o Mario sai da área da fase
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("MarioMap") || other.name == "Mario")
        {
            marioEstaNaFase = false;
        }
    }

    void Update()
    {
        // Se o Mario estiver no colisor e pressionar a tecla Enter (ou Return)
        if (marioEstaNaFase && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
        {
            CarregarFase();
        }
    }

    private void CarregarFase()
    {
        if (!string.IsNullOrEmpty(nomeDaCena))
        {
            SceneManager.LoadScene(nomeDaCena);
        }
        else
        {
            Debug.LogWarning("O nome da cena não foi definido no script da " + gameObject.name);
        }
    }
}
