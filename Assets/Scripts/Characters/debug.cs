using System;
using UnityEngine;

public class debug : MonoBehaviour
{
    void Update()
    {
        // Check if any key is pressed during this frame
        if (Input.anyKeyDown)
        {
            // Iterate through all possible KeyCode values to determine which key(s) were pressed
            foreach (KeyCode keyCode in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(keyCode))
                {
                    // Log each key that is pressed down this frame
                    Debug.Log("KeyCode down: " + keyCode);
                }
            }
        }
    }
}
