using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        animator.StopPlayback();
    }

    public void moveToGame()
    {
        animator.Play("Cam-menutogame");
    }

    public void moveToMenu()
    {
        animator.Play("Cam-gametomenu");
    }
}
