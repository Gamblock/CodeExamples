using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnlockGames.BA.MiniGames.DressingUp.Mechanics
{
    public class RotationInputMobile : RotationInputAdapter
    {
        public override float CalculateRotation()
        {
            if (Input.touchCount > 0)
            {
                Touch screenTouch = Input.GetTouch(0);

                if (screenTouch.phase == TouchPhase.Moved)
                {
                    return -screenTouch.deltaPosition.x * _rotationSpeed * Time.deltaTime;
                }
            }
            return 0;
        }
    }
}
