using System;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public bool isGameOver;
    public bool isGameWon;
    
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject gameWonPanel;

    private void Update()
    {
        if (isGameOver)
            ShowGameOverPanel();
        
        if (isGameWon)
            ShowGameWonPanel();
        
        if (isGameWon || isGameOver)
            Time.timeScale = 0;
    }
    
    public void ShowGameOverPanel()
    {
        gameOverPanel.SetActive(true);
    }
    
    public void ShowGameWonPanel()
    {
        gameWonPanel.SetActive(true);
    }
}
