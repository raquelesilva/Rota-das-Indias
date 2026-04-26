using UnityEngine;

namespace Unity.FantasyKingdom
{
    public class RotateCamera : MonoBehaviour
    {
        [SerializeField] float cameraSpeed = 1.0f;
        [SerializeField] Transform lookAt;

        void Start()
        {
            transform.LookAt(lookAt);
        }

        void Update()
        {
            transform.Rotate(0, Time.deltaTime * cameraSpeed, 0);
        }
    }
}