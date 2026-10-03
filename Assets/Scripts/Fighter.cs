using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighter : MonoBehaviour
{
    private Animator anim;
    private int noOfClicks;
    private int processedStateHash;
    private bool comboWindowProcessed;
    private const float comboTransitionTime = 0.7f;
    
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnClick();
        }

        AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);
        if (state.fullPathHash != processedStateHash)
        {
            processedStateHash = state.fullPathHash;
            comboWindowProcessed = false;
        }

        bool isAttackState = state.IsName("hit1") || state.IsName("hit2") || state.IsName("hit3");
        if (!isAttackState || comboWindowProcessed || state.normalizedTime < comboTransitionTime)
        {
            return;
        }

        comboWindowProcessed = true;
        if (state.IsName("hit1"))
        {
            if (noOfClicks >= 2)
            {
                anim.SetBool("hit1", false);
                anim.SetBool("hit2", true);
            }
            else
            {
                anim.SetBool("hit1", false);
                noOfClicks = 0;
            }
        }
        else if (state.IsName("hit2"))
        {
            if (noOfClicks >= 3)
            {
                anim.SetBool("hit2", false);
                anim.SetBool("hit3", true);
            }
            else
            {
                anim.SetBool("hit2", false);
                noOfClicks = 0;
            }
        }
        else if (state.IsName("hit3"))
        {
            anim.SetBool("hit3", false);
            noOfClicks = 0;
        }
    }

    void OnClick()
    {
        if (noOfClicks == 0)
        {
            noOfClicks = 1;
            anim.SetBool("hit1", true);
            return;
        }

        if (noOfClicks < 3)
        {
            noOfClicks++;
        }
    }
}
