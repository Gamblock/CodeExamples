using UnityEngine;

namespace UnlockGames.BA.DialogueSystem
{
    public class AlwaysLookAtCamera : MonoBehaviour
    {
        private Camera _mainCamera;
        private void Start()
        {
            _mainCamera = Camera.main;
        }

        private void Update()
        {
            transform.LookAt(_mainCamera.transform.position, Vector3.down);
        }
    }
}