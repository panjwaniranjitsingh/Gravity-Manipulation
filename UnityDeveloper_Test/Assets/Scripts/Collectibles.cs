using UnityEngine;

public class Collectibles : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponentInParent<PlayerController>() != null)
        {
            gameObject.SetActive(false);
        }
    }
}
