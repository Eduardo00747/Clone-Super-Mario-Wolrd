using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectOptions : MonoBehaviour
{
    // Menu principal
    public GameObject mainMenu;

    // Menu de players
    public GameObject playerMenu;

    // Referência do cursor
    public CursorManage cursorManage;

    private bool playerMenuOpened = false;

    private void Start()
    {
        mainMenu.SetActive(true);
        playerMenu.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            // Menu principal
            if (!playerMenuOpened)
            {
                int currentOption = cursorManage.GetCurrentIndex();

                // Mario A, B ou C
                if (currentOption == 0 || currentOption == 1 || currentOption == 2)
                {
                    OpenPlayerMenu();
                }
            }
            // Menu de players
            else
            {
                int currentOption = cursorManage.GetCurrentIndex();

                // 1 Player Game ou 2 Player Game
                if (currentOption == 0 || currentOption == 1)
                {
                    StartGame();
                }
            }
        }
    }

    private void StartGame()
    {
        SceneManager.LoadScene("Abertura");
    }

    private void OpenPlayerMenu()
    {
        playerMenuOpened = true;

        // Desativa menu principal
        mainMenu.SetActive(false);

        // Ativa menu player
        playerMenu.SetActive(true);

        // Troca movimentação do cursor
        cursorManage.EnablePlayerMenuCursor();
    }
}
