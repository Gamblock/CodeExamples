using System;
using System.Collections;
using System.Collections.Generic;
using InControl;
using UnityEngine;
using UnityEngine.Serialization;
using Touch = UnityEngine.Touch;

public class CharacterRotator : MonoBehaviour
{
    public float touchSensitivity = 1f;
    public bool rotationRestricted;
    public Vector2 angleRange = new Vector2(-90, 90);
    [FormerlySerializedAs("voidEventChannelSo")] public VoidEventChannelSO characterRotatedChannel;
    private float angleY = 0;
    private bool isRotated;

    private void Update()
    {
        if (TouchManager.TouchCount < 1)
        {
            return;
        }

        var touch = TouchManager.GetTouch(0);
        if (touch.phase != TouchPhase.Moved)
        {
            return;
        }

        var deltaX = touch.deltaPosition.x;
        if (rotationRestricted)
        {
            angleY = Mathf.Clamp(angleY + touchSensitivity * deltaX, angleRange.x, angleRange.y);
            transform.localRotation = Quaternion.Euler(transform.localRotation.x, angleY, transform.localRotation.z);
        }
        else
        {
            transform.rotation *= Quaternion.Euler(0, touchSensitivity * deltaX, 0);
        }
        
        if (characterRotatedChannel && isRotated)
        {
            characterRotatedChannel.RaiseEvent();
        }

        if (Math.Abs(deltaX) > 8)
        {
            isRotated = true;
        }
    }
}
