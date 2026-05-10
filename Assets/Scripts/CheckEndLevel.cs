using CoreSystems.Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckEndLevel : MonoBehaviour
{
    private bool levelCompleted = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (levelCompleted)
            {
                SceneManager.LoadScene(2);
            }
            else
            {
                NotificationManager.instance.SetInfoMessage("Precisas de acabar o n�vel para seguires em frente");
            }
        }
    }

    public void CompleteLevel()
    {
        Debug.Log("potato");
        levelCompleted = true;
    }

    public void ChangeSceneWhenLevelCompleted(int sceneIndex)
    {
        if (levelCompleted)
        {
            SceneManager.LoadScene(sceneIndex);
        }
    }

    public void ChangeToNextScene()
    {
        if (levelCompleted)
        {
            int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
            SceneManager.LoadScene(nextSceneIndex);
        }
    }
}