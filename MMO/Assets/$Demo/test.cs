using UnityEngine;
using System.Collections;

public class test : MonoBehaviour
{

    public RenderTexture RT = null;
    public void OnRenderImage(RenderTexture source2, RenderTexture destination)
    {
        RT = source2;
        Graphics.Blit(source2, destination);
    }
}