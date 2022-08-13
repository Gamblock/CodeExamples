using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnlockGames.BA.MiniGames.DressingUp.Mechanics
{
    public abstract class RotationInputAdapter
    {
        protected float _rotationSpeed;

        public void SetUpInitialSpeed(float rotationSpeed)
        {
            _rotationSpeed = rotationSpeed;
        }
        /// we add '-' to delta in calculation of both cases because staged npc is rotated towards us
        public abstract float CalculateRotation();
    }
}