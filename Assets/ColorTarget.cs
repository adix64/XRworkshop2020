using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorTarget : MonoBehaviour
{
    public Transform player;
    MeshRenderer meshRenderer;
    Transform camTransform;
    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        camTransform = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 toPlayer = (transform.position - player.position).normalized;
        float dotProd = Vector3.Dot(camTransform.forward, toPlayer); //[-1..1]
        dotProd = dotProd * 0.5f + 0.5f; //[0..1]
        dotProd = Mathf.Pow(dotProd, 8f);
        Color newColor = Color.Lerp(Color.black, Color.yellow, dotProd);
        //var mat = meshRenderer.material;
        //mat.SetColor("_EmissionColor", newColor);
        meshRenderer.material.color = newColor;
    }
}
