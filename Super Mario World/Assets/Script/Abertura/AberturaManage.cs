using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AberturaManage : MonoBehaviour
{
    // Objeto Text Box
    public RectTransform textBox;

    // Tempo da animação
    public float animationDuration = 0.5f;

    private void Start()
    {
        // Começa zerado
        textBox.localScale = Vector3.zero;

        // Inicia animação
        StartCoroutine(ShowTextBox());
    }

    private IEnumerator ShowTextBox()
    {
        // Espera 0.3 segundos
        yield return new WaitForSeconds(0.3f);

        Vector3 startScale = Vector3.zero;

        Vector3 targetScale = new Vector3(
            5.987126f,
            4.238373f,
            4.238373f
        );

        float elapsedTime = 0f;

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;

            float t = elapsedTime / animationDuration;

            textBox.localScale = Vector3.Lerp(
                startScale,
                targetScale,
                t
            );

            yield return null;
        }

        // Garante o valor final exato
        textBox.localScale = targetScale;

        // Aguardar um pequeno atraso antes de mudar para a cena "Fase 1"
        yield return new WaitForSeconds(13f);

        // Mudar para a cena "Fase 1"
        SceneManager.LoadScene("OverWorld");
    }
}
