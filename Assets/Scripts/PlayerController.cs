using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 7f;

    [Header("Wiggle Settings")]
    public Transform visualChild;
    public float wiggleDuration = 0.15f;
    public Vector3 walkSquash = new Vector3(1.2f, 0.8f, 1.2f);
    public float tiltAmount = 10f;
    
    private Rigidbody rb;
    private Vector3 moveDirection;
    private Tween wiggleTween;
    private Tween tiltTween;
    private float currentTiltDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        if (visualChild == null) visualChild = this.transform;
    }

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        
        moveDirection = new Vector3(h, 0f, v).normalized;

        HandleWiggleAnimation();
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    void MovePlayer()
    {
        rb.velocity = new Vector3(moveDirection.x * speed, rb.velocity.y, moveDirection.z * speed);
    }

    void HandleWiggleAnimation()
    {
        if (moveDirection.magnitude > 0)
        {
            // Wiggle
            if (wiggleTween == null || !wiggleTween.IsActive())
            {
                wiggleTween = visualChild.DOScale(walkSquash, wiggleDuration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutQuad);
            }
            
            float targetTiltDirection = moveDirection.x > 0 ? -1f : 1f;
            if (moveDirection.x == 0) targetTiltDirection = 1f;
            
            // Tilt
            if (tiltTween == null || !tiltTween.IsActive() || currentTiltDirection != targetTiltDirection)
            {
                if (tiltTween != null && tiltTween.IsActive()) tiltTween.Kill();
                
                currentTiltDirection = targetTiltDirection;
                float targetTilt = currentTiltDirection * tiltAmount;
                
                tiltTween = visualChild.DOLocalRotate(new Vector3(0, 0, targetTilt), wiggleDuration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutQuad);
            }
        }
        else
        {
            // Stopped
            bool isWiggling = wiggleTween != null && wiggleTween.IsActive();
            bool isTilting = tiltTween != null && tiltTween.IsActive();
            
            if (isWiggling || isTilting)
            {
                if (isWiggling) wiggleTween.Kill();
                if (isTilting) tiltTween.Kill();
                
                visualChild.DOScale(Vector3.one, 0.1f);
                visualChild.DOLocalRotate(Vector3.zero, 0.1f);
            }
        }
    }
}
