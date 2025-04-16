using System;
using System.Collections.Generic;
using UnityEngine;

namespace InputManagement
{
    /// <summary>
    /// Input configuration for move priority analysis. Fairly static.
    /// </summary>
    [Serializable]
    [CreateAssetMenu(fileName = Constants.InputBufferSOFileName, menuName = Constants.ScriptableObjectsFolder + Constants.InputBufferSOFileName)]
    public class InputConfigSO : ScriptableObject
    {
        public List<PriorityPair> priorityPairs = new List<PriorityPair>();

        // Create the priorities lookup map
        // Priorities indicate to the input buffer which input should be treated as active if multiple
        // unacknowledged inputs exist within the buffer. Lowest priority wins.
        public Dictionary<EInput, int> MakePriorities()
        {
            Dictionary<EInput, int> result = new Dictionary<EInput, int> ();

            foreach (PriorityPair pair in priorityPairs) { 
                result.Add(pair.InputType, pair.Priority);
            }

            return result;
        }
    }

    /// <summary>
    /// Struct for listing pairs of input to priorities in the Scriptable Object.
    /// </summary>
    [Serializable]
    public struct PriorityPair
    {
        [Tooltip("The input type")]
        public EInput InputType;

        [Tooltip("Lower values are treated as higher priority")]
        public int Priority;
    }
}