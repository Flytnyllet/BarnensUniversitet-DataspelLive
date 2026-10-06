using UnityEngine;

public class RestartButton : MonoBehaviour
{
	[SerializeField] private UIManager uiManager;

	public void RestartGame()
	{
		uiManager.isGameOver = false;
		uiManager.isGameWon = false;
		Time.timeScale = 1;

		UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
	}
}