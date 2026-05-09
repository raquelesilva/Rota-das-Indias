using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class TriggerNarration : MonoBehaviour
{
    public UnityEvent TriggerEndScene;
    public UnityEvent Extra;
    public int ExpectedScene;
    public bool isactive;
    private int count;

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
    public void counting()
    {
        count++;
        if (count >= 3)
        {
            isactive = true;
            Extra?.Invoke();
        }
    }
}