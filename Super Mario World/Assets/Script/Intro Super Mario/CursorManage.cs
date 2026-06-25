using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CursorManage : MonoBehaviour
{
    // Cursor UI
    public RectTransform cursorObject;

    // Componente visual do cursor
    public RawImage cursorImage;

    // Posições do menu principal
    private Vector2[] mainMenuPositions;

    // Posições do menu de players
    private Vector2[] playerMenuPositions;

    // Posições atuais em uso
    private Vector2[] currentPositions;

    // Índice atual
    private int currentIndex = 0;

    // Estado atual do menu
    public bool isPlayerMenu = false;

    private void Start()
    {
        // Menu principal
        mainMenuPositions = new Vector2[]
        {
            new Vector2(-381f, -36f),
            new Vector2(-381f, -127f),
            new Vector2(-381f, -221f),
            new Vector2(-381f, -319f)
        };

        // Menu player
        playerMenuPositions = new Vector2[]
        {
            new Vector2(-381f, -36f),
            new Vector2(-381f, -127f)
        };

        // Começa no menu principal
        currentPositions = mainMenuPositions;

        // Define posição inicial
        cursorObject.anchoredPosition = currentPositions[currentIndex];

        // Cursor piscando
        StartCoroutine(BlinkCursor());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            MoveCursorDown();
        }

        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            MoveCursorUp();
        }
    }

    private void MoveCursorDown()
    {
        currentIndex++;

        if (currentIndex >= currentPositions.Length)
        {
            currentIndex = 0;
        }

        UpdateCursorPosition();
    }

    private void MoveCursorUp()
    {
        currentIndex--;

        if (currentIndex < 0)
        {
            currentIndex = currentPositions.Length - 1;
        }

        UpdateCursorPosition();
    }

    private void UpdateCursorPosition()
    {
        cursorObject.anchoredPosition = currentPositions[currentIndex];
    }

    // Troca para menu de players
    public void EnablePlayerMenuCursor()
    {
        isPlayerMenu = true;

        currentPositions = playerMenuPositions;

        currentIndex = 0;

        UpdateCursorPosition();
    }

    // Retorna índice atual
    public int GetCurrentIndex()
    {
        return currentIndex;
    }

    // Cursor piscando
    private IEnumerator BlinkCursor()
    {
        while (true)
        {
            cursorImage.enabled = false;

            yield return new WaitForSeconds(0.3f);

            cursorImage.enabled = true;

            yield return new WaitForSeconds(0.3f);
        }
    }
}
