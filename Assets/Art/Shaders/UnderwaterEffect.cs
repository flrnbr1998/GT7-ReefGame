using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class UnderwaterEffect : MonoBehaviour
{
    public Material _mat;

    [Range(0.001f, 0.1f)]
    public float _pixelOffset;
    [Range(0.1f, 10f)]
    public float _noiseScale;
    [Range(0.1f, 20f)]
    public float _noiseFrequency;
    [Range(0.1f, 30f)]
    public float _noiseSpeed;
    [Range(0.1f, 20f)]
    public float _timeScale;
    [Range(0.01f, 10.1f)]
    public float _maxBlur;
    [Range(0.01f, 10.0f)]
    public float _depthScale;
    [Range(0.01f, 1.0f)]
    public float _blurStartDepth;
    [Range(0.01f, 100.0f)]
    public float _blurEndDepth;
    [Range(-10.0f, 10.0f)]
    public float _waterLevel;
    [Range(0.1f, 20.0f)]
    public float _fadeDistance;
    public Color _underwaterColor = new Color(0, 0.5f, 1, 1);

    private float _time;

    // Start is called before the first frame update
    void Start()
    {
        UpdateShaderProperties();
        _time = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateShaderProperties();
        _time += Time.deltaTime * _timeScale;
        _mat.SetFloat("_NoiseOffset", _time);
    }

    private void UpdateShaderProperties()
    {
        _mat.SetFloat("_NoiseFrequency", _noiseFrequency);
        _mat.SetFloat("_NoiseSpeed", _noiseSpeed);
        _mat.SetFloat("_NoiseScale", _noiseScale);
        _mat.SetFloat("_PixelOffset", _pixelOffset);
        _mat.SetFloat("_MaxBlur", _maxBlur);
        _mat.SetFloat("_DepthScale", _depthScale);
        _mat.SetFloat("_BlurStartDepth", _blurStartDepth);
        _mat.SetFloat("_BlurEndDepth", _blurEndDepth);
        _mat.SetFloat("_WaterLevel", _waterLevel);
        _mat.SetFloat("_FadeDistance", _fadeDistance);
        _mat.SetColor("_UnderwaterColor", _underwaterColor);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        _mat.SetTexture("_CameraDepthTexture", Shader.GetGlobalTexture("_CameraDepthTexture"));
        Graphics.Blit(source, destination, _mat);
    }
}