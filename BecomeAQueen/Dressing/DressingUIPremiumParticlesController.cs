using UnityEngine;
using UnityEngine.UI.Extensions;

namespace _Game.Scripts.LevelLogic.Dressing
{
    public class DressingUIPremiumParticlesController : MonoBehaviour
    {
        public UIParticleSystem onClickParticles;

        public void EmitOnClickParticles()
        {
            onClickParticles.StartParticleEmission();
        }
    }
}