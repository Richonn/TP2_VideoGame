using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;

    [Tooltip("Vitesse de suivi (plus grand = plus collé).")]
    [SerializeField] private float smoothSpeed = 8f;

    [Tooltip("Décalage Z de la caméra (doit rester négatif en 2D).")]
    [SerializeField] private float zOffset = -10f;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 destination = new Vector3(target.position.x, target.position.y, zOffset);
        transform.position = Vector3.Lerp(transform.position, destination, smoothSpeed * Time.deltaTime);
    }

    public void SetTarget(Transform t) => target = t;
}
