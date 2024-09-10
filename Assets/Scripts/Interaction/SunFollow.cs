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

    // Store the initial Y position of the sun
    private float initialY;

    void Start()
    {
        // Save the initial Y position to keep the sun at the top
        initialY = transform.position.y;
    }

    void Update()
    {
        // Calculate the target position, only change X and Z, keep Y the same
        targetPosition = new Vector3(player.position.x + offset.x, initialY, player.position.z + offset.z);

        // Smoothly move the sun towards the target position
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
    }
}
