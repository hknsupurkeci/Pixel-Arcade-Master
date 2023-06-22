using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeelScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Invoke("RayCast", 0.7f);
    }
    void RayCast()
    {
        // Burada sald�r� mant��� ve kodunu ekleyebilirsiniz
        Vector2 down = -transform.up;

        Vector2 origin = transform.position;
        float raycastDistance = 0.8f;

        RaycastHit2D[] hits = Physics2D.RaycastAll(origin, down, raycastDistance);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null && hit.collider.CompareTag("Player"))
            {
                Debug.Log("Speel damage!");
                hit.collider.gameObject.GetComponent<GetHit>().Hit();
                break;
                // D��man� vurdu�unuzda yapmak istedi�iniz i�lemleri buraya ekleyin
            }
        }
        Destroy(this.gameObject);
    }
}
