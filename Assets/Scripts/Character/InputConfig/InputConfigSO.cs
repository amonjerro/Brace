using System;
using System.Collections.Generic;
using UnityEngine;

namespace InputManagement
{
    [Serializable]
    [CreateAssetMenu(fileName = Constants.InputBufferSOFileName, menuName = Constants.ScriptableObjectsFolder + Constants.InputBufferSOFileName)]
    public class InputConfigSO : ScriptableObject
    {
        public List<PriorityPair> priorityPairs = new List<PriorityPair>();

        public Dictionary<EInput, int> MakePriorities()
        {
            Dictionary<EInput, int> result = new Dictionary<EInput, int> ();

            foreach (PriorityPair pair in priorityPairs) { 
                result.Add(pair.InputType, pair.Priority);
            }

            return result;
        }
    }

    [Serializable]
    public struct PriorityPair
    {
        [Tooltip("The input type")]
        public EInput InputType;

        [Tooltip("Lower values are treated as higher priority")]
        public int Priority;
    }
}