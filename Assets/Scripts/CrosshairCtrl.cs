using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairCtrl : MonoBehaviour
{
    UnityEngine.UI.Image[] images;
    bool overTheShoulder = false;
    // Start is called before the first frame update
    void Start()
    {
        images = GetComponentsInChildren<UnityEngine.UI.Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire2"))
            overTheShoulder = !overTheShoulder;
        foreach(var i in images)
            i.enabled = overTheShoulder;
    }
}
