using System;
using UnityEngine;

namespace Project.Scripts.Essentials
{
    [CreateAssetMenu(fileName = "GameConfig")]
    public class GameConfig : SingletonScriptableObject<GameConfig>
    {
        public int            TargetFrameRate ;
        
        public InputVariables  Input;
        public CameraVariables Camera;

    }

    [Serializable]
    public class InputVariables
    {
        public float DrawSelectionBoxThresholdSq = 6000;
    }
    
    [Serializable]
    public class CameraVariables
    {
        public float DragSpeed       = 25;
        public float MoveSensitivity = 0.05f;
    }
}