using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    private bool shiftOn;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftShift) && shiftOn)
        {
            GetComponentInChildren<Animator>().SetTrigger("Dash");
            shiftOn = false;
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
}
