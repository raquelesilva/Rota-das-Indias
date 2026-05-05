using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckEndLevel1 : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            SceneManager.LoadScene(2);
            Debug.Log("Level 1 Completed!");
        }
    }
}