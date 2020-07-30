using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewTrigger : MonoBehaviour
{
    public Transform pietroi;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("PlayerLYR"))
        {
            var go = GameObject.Instantiate(pietroi);
            go.transform.position = transform.position + Vector3.up * 15f;

        }
    }
}
