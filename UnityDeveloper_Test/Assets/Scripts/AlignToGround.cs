using System.Collections;
using UnityEngine;

public class AlignToGround : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            Transform target = transform.parent;
            Vector3 groundNormal = GetGroundNormal(target);
            Quaternion rotation = Quaternion.FromToRotation(target.up, groundNormal) * target.rotation;
            target.GetComponent<PlayerController>().RotatePlayer(rotation);
        }
    }

    private Vector3 GetGroundNormal(Transform target)
    {
        if (Physics.Raycast(target.position, -target.up, out RaycastHit hit, Mathf.Infinity))
        {
            return hit.normal;
        }

        return target.up;
    }
}
