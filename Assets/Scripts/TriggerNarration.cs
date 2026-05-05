using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class TriggerNarration : MonoBehaviour
{
    public UnityEvent TriggerEndScene;
    public int ExpectedScene;
    public bool isactive;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter(Collider other)
    {
        if (isactive) TriggerEndScene?.Invoke();

    }
    public void setactive()
    {
        isactive = true;
    }
    public void nextscene()
    {
        SceneManager.LoadScene(ExpectedScene);
    }
}
