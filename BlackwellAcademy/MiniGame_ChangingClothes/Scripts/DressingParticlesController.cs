using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnlockGames.BA.MiniGames.DressingUp
{
    public class DressingParticlesController : MonoBehaviour
    {
        public DressingUpViewModel viewModel;
        public List<DressingParticles> particlesList;
        public List<ParticleSystem> completedParticlesList;

        private void OnEnable()
        {
            viewModel.PreviewSelectedClothesElement += PlayParticles;
            viewModel.PlayerHasChosenClothesElement += PlayCompletedParticles;
        }
        

        private void OnDisable()
        {
            foreach (var completedParticles in completedParticlesList)
            {
                completedParticles.gameObject.SetActive(false);
            }
            foreach (var particles in particlesList)
            {
                particles.particles.gameObject.SetActive(false);
            }
            viewModel.PreviewSelectedClothesElement -= PlayParticles;
            viewModel.PlayerHasChosenClothesElement -= PlayCompletedParticles;
        }

        private void PlayParticles(string clothesElementName, ClothesType clothesType)
        {
            if (clothesType == ClothesType.None)
            {
                return;
            }
            var particlesToPlay = particlesList.Find(x => x.clothesType == clothesType);
            if (!particlesToPlay.particles.gameObject.activeSelf)
            {
                particlesToPlay.particles.gameObject.SetActive(true);
            }
            particlesToPlay?.particles.Play();
        }
        
        private void PlayCompletedParticles()
        {
            foreach (var completedParticles in completedParticlesList)
            {
                completedParticles.gameObject.SetActive(true);
                completedParticles.Play();
            }
        }
    }

    
    [Serializable]
    public class DressingParticles
    {
        public ClothesType clothesType;
        public ParticleSystem particles;
    }
}