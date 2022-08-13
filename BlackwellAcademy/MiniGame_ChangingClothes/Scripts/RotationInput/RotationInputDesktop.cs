using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnlockGames.BA.MiniGames.DressingUp.Mechanics
{
    public class RotationInputDesktop : RotationInputAdapter
    {
        private Vector2 _lastRecordedMousePos;
        public override float CalculateRotation()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _lastRecordedMousePos = Input.mousePosition;
            }

            if(Input.GetMouseButton(0))
            {
                Vector2 currentMousePos = Input.mousePosition;

                float xDelta = -(currentMousePos.x - _lastRecordedMousePos.x);
                float rotateBy = xDelta * _rotationSpeed * Time.deltaTime;

                _lastRecordedMousePos = currentMousePos;
                return rotateBy;
            }

            return 0;
        }
    }
}

