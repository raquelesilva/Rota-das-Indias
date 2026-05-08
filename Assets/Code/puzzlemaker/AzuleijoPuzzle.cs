using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class AzuleijoPuzzle : MonoBehaviour
{
    public GameObject puzzle;
    public GameObject player;
    public List<UnityEvent> hinting;
    private int count;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void StartPuzzle()
    {
        count = hinting.Count;
        player.SetActive(false);
        puzzle.SetActive(true);
        if (hinting.Count > 0)
        {
            StartCoroutine(Hinting(20f));
        }
    }

    public void StopPuzzle()
    {
        StartCoroutine(Stopping(3f));
    }

    IEnumerator Stopping(float delay)
    {
        yield return new WaitForSeconds(delay);
        player.SetActive(true);
        puzzle.SetActive(false);
    }

    IEnumerator Hinting(float delay)
    {
        yield return new WaitForSeconds(delay);

        count--;
        hinting[count]?.Invoke();
        hinting.RemoveAt(count);
        if (hinting.Count > 0)
        {
            StartCoroutine(Hinting(20f));
        }
    }
}