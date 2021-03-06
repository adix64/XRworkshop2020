﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThridPersonCamCtrl : MonoBehaviour
{
    public Transform player;
    float pitch = 0f, yaw = 0f;
    public float distToTarget = 5f, distToTargetSmooth = 5f;
    public Vector3 lookOffset, lookOffsetSmooth;

    public float smoothingSpeed = 10f;
    public float rightOffset = 1.2f;
    public float fwdOffset = 1.2f;

    bool overTheShoulder = false;
    // Start is called before the first frame update
    void Start()
    {
     
    }

    // camera este ultimul obiect din scena de actualizat per frame,
    void LateUpdate() // in loc de Update, ca sa nu tremure obiectul din cauza decalarii cadrelor
    {
        pitch -= Input.GetAxis("Mouse Y");
        yaw += Input.GetAxis("Mouse X");
        pitch = Mathf.Clamp(pitch, -5f, 45f); // limitam rotatia ca sa nu se dea camera peste cap
                                              //https://www.researchgate.net/profile/Jorma_Laaksonen/publication/305684696/figure/fig1/AS:391458059243523@1470342282144/The-yaw-pitch-and-roll-angles-in-the-human-head-motion-11.png

        transform.rotation = Quaternion.Euler(pitch, yaw, 0f); // unghiurile axelor camerei cu axele lumii
      
        distToTargetSmooth = Mathf.Lerp(distToTargetSmooth, distToTarget, Time.deltaTime * smoothingSpeed);
        lookOffsetSmooth = Vector3.Lerp(lookOffsetSmooth, lookOffset, Time.deltaTime * smoothingSpeed);
        transform.position = player.position // urmarim playerul, ne punem de la pozitia sa...
                            - transform.forward * distToTargetSmooth //...in spate; camera se uita de-alungul axei forward
                            + lookOffsetSmooth;

        if (Input.GetButtonDown("Fire2"))
            overTheShoulder = !overTheShoulder;
        
        if (overTheShoulder)
            OverTheShoulderLook();
        else
            DefaultLook();
    }

    void OverTheShoulderLook()
    {
        lookOffset = transform.right * rightOffset + transform.forward * fwdOffset;
        //distToTarget = 2f;
    }
    void DefaultLook()
    {
        lookOffset = Vector3.zero;
        //distToTarget = 5f;
    }

}
