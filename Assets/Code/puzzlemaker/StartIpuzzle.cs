using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class NewMonoBehaviourScript1 : MonoBehaviour
{
    public GameObject puzzle;
    public GameObject player;
    public List<UnityEvent> Hinting;
    private int count;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void startpuzzle()
    {
        count = Hinting.Count;
        player.SetActive(false);
        puzzle.SetActive(true);
        if (Hinting.Count > 0)
            StartCoroutine(hinting(6f));



    }
    public void stoppuzzle()
    {
        StartCoroutine(stopping(3f));

    }
    IEnumerator stopping(float delay)
    {
        yield return new WaitForSeconds(delay);
        player.SetActive(true);
        puzzle.SetActive(false);
    }
    IEnumerator hinting(float delay)
    {
        yield return new WaitForSeconds(delay);

        count--;
        Hinting[count]?.Invoke();
        Hinting.RemoveAt(count);
        if (Hinting.Count > 0)
            StartCoroutine(hinting(6f));



    }
}
