using CoreSystems.Managers;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckEndLevel1 : MonoBehaviour
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
                NotificationManager.instance.SetInfoMessage("Precisas de acabar o nível para seguires em frente");
            }
        }
    }

    public void CompleteLevel()
    {
        levelCompleted = true;
    }
}