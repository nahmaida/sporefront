using UnityEngine;
using UnityEngine.SceneManagement; 

namespace Assets.Scripts
{
    public class MainMenu : MonoBehaviour
    {
        // Метод для запуска игры
        public void PlayGame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex +1);
        }

        // Метод для выхода из игры
        public void ExitGame()
        {
            Debug.Log("Игра закрывается..."); 
            Application.Quit(); // Закрывает приложение
        }
    }
}