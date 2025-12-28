
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Menus
{
    internal class Story : MonoBehaviour
    {
        public void Back()
        {
            SceneManager.LoadScene("WorldPlay");
        }
    }
}
