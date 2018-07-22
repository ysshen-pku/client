using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Utility;

public class Charctor : MonoBehaviour {

    [SerializeField] private MouseLook m_MouseLook;

    private Camera m_Camera;


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        RotateView();
    }

    private void RotateView()
    {
        m_MouseLook.LookRotation(transform, m_Camera.transform);
    }

}
