using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class VictoryUI : MonoBehaviour
{
    public TextMeshProUGUI victoryText;
    public KeyCode restartKey = KeyCode.Return;
    public KeyCode menuKey = KeyCode.Escape;
    public GameObject victoryUI;

    // Start is called before the first frame update
    void Start()
    {
        victoryUI.SetActive(false);
    }
    private void Update()
    {
        if (victoryUI.activeSelf && Input.GetKeyDown(restartKey)) // Check if victory UI is shown and restart key is pressed
        {
            RestartGame();
        }
        if (victoryUI.activeSelf && Input.GetKeyDown(menuKey)) // Check if victory UI is shown and menu key is pressed
        {
            GoToMainMenu();
        }
    }

    public void ShowVictoryUI()
    {
        victoryText.text = "You have defeated Ornstein...\nThis Time!\nPress Enter to Restart.\nPress ESC to go to the Main Menu";
        victoryUI.SetActive(true);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
