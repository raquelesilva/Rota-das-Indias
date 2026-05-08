using UnityEngine;

public class gateopen : MonoBehaviour
{
    private bool check = false;
    public float test;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (check == true && gameObject.transform.position.y > test)
        {
            gameObject.transform.position -= new Vector3(0, Time.deltaTime * 1.5f, 0);
        }

    }
    public void opengate()
    {
        check = true;
    }
}
