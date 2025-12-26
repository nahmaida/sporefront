using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Menu
{
    internal class WorldMenu : MonoBehaviour
    {
        // Метод для запуска игры
        public void MainMenu()
        {
            SceneManager.LoadScene("MainMenu");
        }

        // Метод для выхода из игры
        public void Story()
        {
            SceneManager.LoadScene("Story");
        }
    }
}
