using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DestroyChildrenOverTime : MonoBehaviour
{
    [SerializeField] float timeToDestroy;
    MeshRenderer[] childrenMeshRenderers;
    float timeAlive;
    [SerializeField] float fadeTime;
    Material material;
    Color fadeColor;
    float currentAlpha;
    float lerp;
    float timeFading;
    // Use this for initialization
    void Start()
    {
        childrenMeshRenderers = GetComponentsInChildren<MeshRenderer>();
        timeAlive = 0;
        if (fadeTime > 0)
        {
            material = childrenMeshRenderers[0].material;
            fadeColor = material.color;
        }
        timeFading = 0f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        timeAlive += Time.fixedDeltaTime;
        if (timeAlive >= timeToDestroy)
        {
            Destroy(this.gameObject);
        }

        if (timeToDestroy - timeAlive <= fadeTime && fadeTime > 0)
        {
            timeFading += Time.deltaTime;
            currentAlpha = 1 - (timeFading / fadeTime);
            fadeColor.a = currentAlpha;
            foreach (MeshRenderer childMeshRenderer in childrenMeshRenderers)
            {
                childMeshRenderer.material.color = fadeColor;
            }
            
            
        }
    }
}
