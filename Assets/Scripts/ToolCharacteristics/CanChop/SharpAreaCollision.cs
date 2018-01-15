using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharpAreaCollision : MonoBehaviour {
    public bool canSlice = true;
    
    IEnumerator EnableSlice(float time)
    {
        yield return new WaitForSeconds(time);

        canSlice = true;
    }

    
    private void OnCollisionEnter(Collision collision)
    {
        if (canSlice && collision.gameObject.GetComponent<CanBeChopped>())
        {
            canSlice = false;

            collision.gameObject.GetComponent<CanBeChopped>().BeginSlice(transform.position, transform.up);

            StartCoroutine(EnableSlice(0.5f));
        }
    }
}
