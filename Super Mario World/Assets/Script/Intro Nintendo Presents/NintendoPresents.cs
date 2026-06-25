using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NintendoPresents : MonoBehaviour
{
    // Referências para os objetos que serão ativados e desativados
    public GameObject backgroundObject;
    public GameObject nintendoPresentsObject;

    // Tempo de atraso para ativar o objeto "Konami Intro" após ativar o objeto "Data"
    public float delay = 0.5f;  

    // Start is called before the first frame update
    void Start()
    {
        // Iniciar a corrotina para controlar a sequência de ativação e desativação
        StartCoroutine(ActivateAndDeactivateObjects());
    }

    // Corrotina para controlar a sequência de ativação e desativação
    private IEnumerator ActivateAndDeactivateObjects()
    {
        // Ativar o objeto "Background"
        backgroundObject.SetActive(true);

        // Ativar o objeto "Konami Intro"
        nintendoPresentsObject.SetActive(false);

        // Aguardar o tempo de atraso
        yield return new WaitForSeconds(delay);

        // Ativar o objeto "Konami Intro"
        nintendoPresentsObject.SetActive(true);

        // Aguardar um pequeno atraso antes de mudar para a cena "Fase 1"
        yield return new WaitForSeconds(4.0f);

        // Mudar para a cena "Fase 1"
        SceneManager.LoadScene("Intro Super Mario");
    }
}
