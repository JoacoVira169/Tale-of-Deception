using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    private bool shiftOn;
    public bool isRunning;
    public bool isDash; 

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftShift) && shiftOn)
        {
            if (isRunning)
            {
                isRunning = false; 
            }
            
            isDash = true; 
            

            GetComponentInChildren<Animator>().SetTrigger("Dash");
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