using UnityEngine;
using TMPro;
using InputManagement;
using System.Collections.Generic;

/// <summary>
/// Creates a list on the side of the screen that prints out the contents of the input buffer
/// Exclusively meant as a debugging tool
/// </summary>
public class DebugInputBuffer : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI inputBufferInformation;
    
    [SerializeField]
    bool bIsShowing;

    /// <summary>
    /// Prints the state of the buffer onto the screen
    /// </summary>
    /// <param name="buffer">A player's input buffer</param>
    public void Print(InputBuffer buffer)
    {
        if (!bIsShowing) { 
            return;
        }

        Queue<InputBufferItem> items = buffer.GetInputQueue();
        string output = "";
        int counter = 0;
        foreach (InputBufferItem item in items) { 
            output += counter.ToString()+": "+ item.ToString() + "\n";
            counter++;
        }
        inputBufferInformation.text = output;
    }
   
}