using CoreSystems.Extensions.Attributes;
using UnityEngine;
using UnityEngine.Events;

public class CollisionEvents : MonoBehaviour
{
    [SerializeField] UnityEvent onTriggerEnter;
    [SerializeField] UnityEvent onTriggerExit;
    [HorizontalLine, Space(12)]
    [SerializeField] UnityEvent onCollisionEnter;
    [SerializeField] UnityEvent onCollisionExit;


    private void OnTriggerEnter(Collider other)
    {
        onTriggerEnter?.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        onTriggerExit?.Invoke();
    }

    private void OnCollisionEnter(Collision collision)
    {
        onCollisionEnter?.Invoke();
    }

    private void OnCollisionExit(Collision collision)
    {
        onCollisionExit?.Invoke();
    }
}
