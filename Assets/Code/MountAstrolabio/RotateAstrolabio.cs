using UnityEngine;

public class RotateAstrolabio : MonoBehaviour
{
    #region Variables

    private Camera camera;

    [SerializeField] private float speed;

    [SerializeField] private bool inverted;

    #endregion

    private void Start()
    {
        camera = Camera.main;
    }

    private void OnMouseDown()
    {
        Debug.Log("DOWN");
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnMouseDrag()
    {
        Debug.Log("Drag");
        float mouseY = Input.GetAxis("Mouse Y");
       
        float rotation = mouseY * speed * Time.deltaTime;

        transform.Rotate(Vector3.up, inverted ? rotation : -rotation, Space.World);
    }
}