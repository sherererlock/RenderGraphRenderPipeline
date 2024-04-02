using UnityEngine;
using UnityEngine.Rendering;

public class SetupRPAssets : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public RenderPipelineAsset pipelineAsset;
    private void OnEnable()
    {
        if (pipelineAsset != null)
            GraphicsSettings.renderPipelineAsset = pipelineAsset;
    }

    private void OnValidate()
    {
        if (pipelineAsset != null)
            GraphicsSettings.renderPipelineAsset = pipelineAsset;
    }
}
