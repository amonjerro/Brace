using UnityEngine;
using TMPro;
using InputManagement;
using System.Collections.Generic;

public class DebugInputBuffer : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI inputBufferInformation;
    
    [SerializeField]
    bool bIsShowing;

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