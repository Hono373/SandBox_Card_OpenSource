using System;
using UnityEngine;

[Serializable]
public class OutlineConfig
{
    public Shader outlineShader;
    public Color outlineColor = new Color(1f, 0.8f, 0f);
    public float outlineWidth = 4f;
}

