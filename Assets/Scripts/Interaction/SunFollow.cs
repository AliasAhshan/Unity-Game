using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunFollow : MonoBehaviour
{
    public Transform player;       // Reference to the player transform
    public Vector3 offset;         // Offset distance between the sun and the player

    [Range(0.1f, 10f)]
    public float followSpeed = 2f; // Speed at which the sun follows the player

    private Vector3 targetPosition;

    void Update()
    {
        // Calculate the target position with the offset
        targetPosition = player.position + offset;

        // Smoothly move the sun towards the target position
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
    }
}