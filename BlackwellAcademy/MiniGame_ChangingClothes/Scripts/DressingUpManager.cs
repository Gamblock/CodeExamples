using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnlockGames.BA.AnimationControl;
using UnlockGames.BA.CameraControl;
using UnlockGames.BA.MiniGames.DressingUp.Mechanics;
using UnlockGames.BA.NPC;

namespace UnlockGames.BA.MiniGames.DressingUp
{
    public class DressingUpManager : MonoBehaviour
    {
        [SerializeField] private DressingUpViewModel _dressingUpViewModel;
        [SerializeField] private DressingUpDatabase _dressingUpDatabase;
        [SerializeField] private DressingUpMiniGameController _dressingUpController;
        [SerializeField] private AnimationController _animationController;
        [SerializeField] private ExpositionCameraController _cameraController;
        [SerializeField] private CameraBlendDelegateController _cameraBlendDelegateController;
        
        [Space(5f)]
        [SerializeField] private List<GameObject> _objectsToHideInDressingUp;
        [SerializeField] private Transform _transformForDressingUp;
        [SerializeField] private Transform _transformForDressingUpCamera;
        [SerializeField] private AnimationControl.Animation _idleAnimationForStagedNpc;
        
        [Space(5f)]
        [Space(5f)] [SerializeField] private NpcOnLevel _playerPrefab;

        [Space(5f)] [SerializeField] private float _stagedNpcRotationSpeed = 17f;
        
        [Header("Rotation time")]
        [SerializeField] private float _stagedNpcRestoreRotationTime = 1f;
        [SerializeField] private float _stageRotationTime = 1f;
        
        [HideInInspector] public NpcOnLevel _stagedNpc;
        private AnimationControl.Animation _clothesConfirmedAnimation;
        private string _dressingUpGeneralCamPreset;
        
        private float _waitForAnimation = 3f;
        private float _timer;
        private Coroutine _npcReactionCoroutine;

        private bool _playerRotatingEnabled;
        
        private RotationInputAdapter _rotationInputAdapter;
        private Tween _rotationTween;

        private void OnEnable()
        {
            _dressingUpViewModel.StagingInstructionReceived += StageNpcInstructionReceived;
            _dressingUpViewModel.ManageStageItemsInstructionReceived += ShowHideObjectsInDressingUp;
            _dressingUpViewModel.DressingUpChoiceValidated += OnClothesChoiceValidated;
            _dressingUpViewModel.PreviewSelectedClothesElement += OnClothesElementChosen;

            PrepareForRotation();
        }

        private void OnDisable()
        {
            _dressingUpViewModel.StagingInstructionReceived -= StageNpcInstructionReceived;
            _dressingUpViewModel.ManageStageItemsInstructionReceived -= ShowHideObjectsInDressingUp;
            _dressingUpViewModel.DressingUpChoiceValidated -= OnClothesChoiceValidated;
            _dressingUpViewModel.PreviewSelectedClothesElement -= OnClothesElementChosen;
        }

        private void StageNpcInstructionReceived(StageNpcInstruction stageNpcInstruction)
        {
            UnstageNpc();
            if (stageNpcInstruction != null)
            {
                StageNpc(stageNpcInstruction);
            }
        }

        private void ShowHideObjectsInDressingUp(bool areObjectsActive)
        {
            foreach (var obj in _objectsToHideInDressingUp)
            {
                obj.SetActive(areObjectsActive);
            }
        }
        
        public async void StageNpc(StageNpcInstruction stageNpcInstruction)
        {
            // Record staged npc reaction-animation
            _clothesConfirmedAnimation = _animationController.GetAnimationByArticyId(stageNpcInstruction.animationToPlay);
            _dressingUpGeneralCamPreset = stageNpcInstruction.cameraPreset;
            
            // Spawn and prepare npc for staging
            _stagedNpc = Instantiate(_playerPrefab, _transformForDressingUp.position, _transformForDressingUp.rotation);

            _stagedNpc.npcModelHolder.npcMeshHolder.SetLightProbeAnchorOverride(null);
            
            _stagedNpc.movement.SetMovementState(false);
            _stagedNpc.movement.TeleportTo(_transformForDressingUp.position);
            
            _stagedNpc.npcAnimator.BodyAnimator.transform.localEulerAngles = Vector3.zero;

            _stagedNpc.npcClothesChanger.SetAsMainCharacter(false);
            
            // Set camera to look at staged npc
            _cameraController.SetCameraSettingsByTransform(_dressingUpGeneralCamPreset, _transformForDressingUpCamera);
            
            // Other settings
            _dressingUpController.cinemachineBrain.m_DefaultBlend.m_Time = _dressingUpController.dressingUpCameraBlendTime;
            _playerRotatingEnabled = true;
        }

        public void UnstageNpc()
        {
            if (_stagedNpc != null)
            {
                Destroy(_stagedNpc.gameObject);
            }
        }

        private void OnClothesElementChosen(string clothesElementArticyId, ClothesType clothesType)
        {
            ClothesElementData clothesElementData = new ClothesElementData(_dressingUpDatabase.clothesSynced[clothesElementArticyId]);
            _cameraBlendDelegateController.ChangeCameraByPreset(clothesElementData.cameraPreset, _transformForDressingUpCamera);
            
            PlayAnimationReaction(clothesElementData.animationReaction, clothesElementData.animationReactionDuration);
            Debug.Log($"<b><color=#b412ff>[OnClothesElementChosen, rotation -> {clothesElementData.yAngleOnStage}, itemName({_dressingUpDatabase.clothesSynced[clothesElementArticyId].name})]</color></b>");
            RotateStagedNpcToRotation(clothesElementData.yAngleOnStage, _stageRotationTime);
        }

        private void OnClothesChoiceValidated()
        {
            _playerRotatingEnabled = false;
            RotateStagedNpcToRotation(0f, _stagedNpcRestoreRotationTime);
            
            PlayAnimationReaction(_clothesConfirmedAnimation);
            _cameraBlendDelegateController.ChangeCameraByPreset(_cameraController.GetCameraPreset(_dressingUpGeneralCamPreset), _transformForDressingUpCamera);
        }
        
        private void PlayAnimationReaction(AnimationControl.Animation animationReaction, float limitedTime = 0)
        {
            if (_npcReactionCoroutine != null)
            {
                StopCoroutine(_npcReactionCoroutine);
            }

            if(limitedTime > 0)
                _npcReactionCoroutine = StartCoroutine(AnimationTimer(animationReaction));
            else
                _stagedNpc.npcAnimator.PlayBodyFaceAnimation(animationReaction);
        }

        private IEnumerator AnimationTimer(AnimationControl.Animation animationToPlay)
        {
            _stagedNpc.npcAnimator.PlayBodyFaceAnimation(animationToPlay);
            
            _timer = _waitForAnimation;
            while (_timer > 0)
            {
                yield return new WaitForSeconds(0.1f);
                _timer -= 0.1f;
            }
            
            _stagedNpc.npcAnimator.PlayBodyFaceAnimation(_idleAnimationForStagedNpc);
        }

        
        
        private void PrepareForRotation()
        {
            #if UNITY_EDITOR
                _rotationInputAdapter = new RotationInputDesktop();
            #elif UNITY_ANDROID || UNITY_IOS
                _rotationInputAdapter = new RotationInputMobile();
            #endif
            
            _rotationInputAdapter.SetUpInitialSpeed(_stagedNpcRotationSpeed);
        }

        private void Update()
        {
            if (!_playerRotatingEnabled)
            {
                return;
            }

            float rotation = _rotationInputAdapter.CalculateRotation();
            if (rotation != 0)
            {
                KillRotationTween();
                _stagedNpc.npcAnimator.BodyAnimator.transform.Rotate(Vector3.up, rotation);
            }
        }

        private void RotateStagedNpcToRotation(float yRot, float rotationTime)
        {
            KillRotationTween();
            _rotationTween = _stagedNpc.npcAnimator.BodyAnimator.transform.DOLocalRotate(new Vector3(0, yRot, 0), rotationTime, RotateMode.Fast);
        }

        private void KillRotationTween()
        {
            _rotationTween?.Kill();
        }
    }

    public enum ClothesType
    {
        Shirt,
        Pants,
        Shoes,
        Bag,
        Jewelery,
        None,
    }
}