using UnityEngine;
using System.Collections;

public class NewMonoBehaviourScript1 : MonoBehaviour
{
    public GameObject puzzle;
    public GameObject player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void startpuzzle()
    {

        player.SetActive(false);
        puzzle.SetActive(true);



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
}
