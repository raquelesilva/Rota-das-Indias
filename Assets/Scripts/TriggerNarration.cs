using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class TriggerNarration : MonoBehaviour
{
    public UnityEvent TriggerEndScene;
    public int ExpectedScene;
    public bool isactive;

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