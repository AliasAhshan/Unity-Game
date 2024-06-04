using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyeFollow : MonoBehaviour
{
    public GameObject player;
    public GameObject leftEyeball;
    public GameObject rightEyeball;

    public float eyeRadius = 0.5f; // Adjust this to fit within the eye components

    void Start()
    {
        player = GameObject.Find("Player 2");

        // Find the left and right eyeball GameObjects by name
        leftEyeball = transform.Find("left eyeball").gameObject;
        rightEyeball = transform.Find("right eyeball").gameObject;
    }

    void Update()
    {
        FollowPlayer();
    }

    void FollowPlayer()
    {
        Vector3 playerPos = player.transform.position;

        // Update position for the left eyeball
        Vector3 leftEyeCenter = transform.Find("left eye").position;
        Vector2 leftDirection = (playerPos - leftEyeCenter).normalized * eyeRadius;
        Vector3 leftTargetPosition = leftEyeCenter + new Vector3(leftDirection.x, leftDirection.y, 0);
        leftEyeball.transform.position = leftTargetPosition;

        // Update position for the right eyeball
        Vector3 rightEyeCenter = transform.Find("right eye").position;
        Vector2 rightDirection = (playerPos - rightEyeCenter).normalized * eyeRadius;
        Vector3 rightTargetPosition = rightEyeCenter + new Vector3(rightDirection.x, rightDirection.y, 0);
        rightEyeball.transform.position = rightTargetPosition;
    }
}