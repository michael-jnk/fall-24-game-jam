using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeafStartMusic : MonoBehaviour
{
    public AudioSource audioSource;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        animator.Play("Leaf-start");
    }

    void Update()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("DonePlaying"))
        {
            audioSource.Play();
            Destroy(this.gameObject);
        }
    }
}
