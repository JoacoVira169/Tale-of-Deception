using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dash : MonoBehaviour
{
    private bool shiftOn;
    private Animator animaciones;
    public bool isRunning;
    public bool isDash; 

    private void Awake()
    {
        animaciones = GetComponentInChildren<Animator>();
        if (animaciones == null)
        {
            Debug.LogError("Dash requires an Animator on this object or one of its children.", this);
            enabled = false;
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftShift) && shiftOn)
        {
            if (isRunning)
            {
                isRunning = false; 
            }
            
            isDash = true; 
            

            animaciones.SetTrigger("Dash");
            shiftOn = false;
            
            StartCoroutine(ApagarDash()); 
        }

        if(Input.GetKeyUp(KeyCode.LeftShift))
        {
            StartCoroutine(tiempoShift());
            shiftOn = true;
        }
    }

    IEnumerator tiempoShift()
    {
        yield return new WaitForSeconds(0.5f);
        shiftOn = false;
    }

    IEnumerator ApagarDash()
    {
        yield return new WaitForSeconds(0.6f); 
        isDash = false; 
    }
}