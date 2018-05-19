using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherPlayer : PlayerController {

    protected override void Start()
    {
        _velocity = velocity;
        _health = health;

        healthUI.text = _health.ToString();

        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        audioSource = GetComponent<AudioSource>();
    }

    protected override void Update()
    {

    }
}