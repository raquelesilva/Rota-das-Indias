using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Events;
public class fade : MonoBehaviour
{
    public UnityEvent TriggerEndScene;

    public void fading()
    {
        StartCoroutine(FadeToFull());
    }
    IEnumerator FadeToFull()
    {



        Color c = gameObject.GetComponent<Image>().color;
        StartCoroutine(Narration(1f));
        while (c.a < 1.0f)
        {
            c.a += Time.deltaTime; // Adjust speed by multiplying Time.deltaTime
            gameObject.GetComponent<Image>().color = c;
            yield return null;
        }
    }
    IEnumerator Narration(float delay)
    {
        yield return new WaitForSeconds(delay);
        TriggerEndScene?.Invoke();
    }
}
