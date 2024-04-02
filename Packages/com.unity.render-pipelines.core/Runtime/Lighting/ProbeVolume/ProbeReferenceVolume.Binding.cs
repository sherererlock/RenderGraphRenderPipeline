using System;
using UnityEngine.Rendering;

namespace UnityEngine.Rendering
{
    public partial class ProbeReferenceVolume
    {
        internal static class ShaderIDs
        {
            // Adaptive Probe Volume
            public static readonly int _APVResIndex = Shader.PropertyToID("_APVResIndex");
            public static readonly int _APVResCellIndices = Shader.PropertyToID("_APVResCellIndices");
            public static readonly int _APVResL0_L1Rx = Shader.PropertyToID("_APVResL0_L1Rx");
            public static readonly int _APVResL1G_L1Ry = Shader.PropertyToID("_APVResL1G_L1Ry");
            public static readonly int _APVResL1B_L1Rz = Shader.PropertyToID("_APVResL1B_L1Rz");

            public static readonly int _APVResL2_0 = Shader.PropertyToID("_APVResL2_0");
            public static readonly int _APVResL2_1 = Shader.PropertyToID("_APVResL2_1");
            public static readonly int _APVResL2_2 = Shader.PropertyToID("_APVResL2_2");
            public static readonly int _APVResL2_3 = Shader.PropertyToID("_APVResL2_3");

            public static readonly int _APVResValidity = Shader.PropertyToID("_APVResValidity");
            public static readonly int _SkyOcclusionTexL0L1 = Shader.PropertyToID("_SkyOcclusionTexL0L1");
            public static readonly int _SkyShadingDirectionIndicesTex = Shader.PropertyToID("_SkyShadingDirectionIndicesTex");
            public static readonly int _SkyPrecomputedDirections = Shader.PropertyToID("_SkyPrecomputedDirections");
        }


        ComputeBuffer m_EmptyIndexBuffer = null;
        ComputeBuffer m_EmptyDirectionsBuffer = null;

        /// <summary>
        /// Bind the global APV resources
        /// </summary>
        /// <param name="cmdBuffer">Command buffer</param>
        /// <param name="isProbeVolumeEnabled">True if APV is enabled</param>
        public void BindAPVRuntimeResources(CommandBuffer cmdBuffer, bool isProbeVolumeEnabled)
        {
            bool needToBindNeutral = true;
            var refVolume = ProbeReferenceVolume.instance;

            // Do this only if probe volume is enabled
            if (isProbeVolumeEnabled && m_ProbeReferenceVolumeInit)
            {
                ProbeReferenceVolume.RuntimeResources rr = refVolume.GetRuntimeResources();

                bool validResources = rr.index != null && rr.L0_L1rx != null && rr.L1_G_ry != null && rr.L1_B_rz != null;
                validResources &= (refVolume.shBands == ProbeVolumeSHBands.SphericalHarmonicsL2 && rr.L2_0 != null) || refVolume.shBands == ProbeVolumeSHBands.SphericalHarmonicsL1;

                if (validResources)
                {
                    cmdBuffer.SetGlobalBuffer(ShaderIDs._APVResIndex, rr.index);
                    cmdBuffer.SetGlobalBuffer(ShaderIDs._APVResCellIndices, rr.cellIndices);

                    cmdBuffer.SetGlobalTexture(ShaderIDs._APVResL0_L1Rx, rr.L0_L1rx);
                    cmdBuffer.SetGlobalTexture(ShaderIDs._APVResL1G_L1Ry, rr.L1_G_ry);
                    cmdBuffer.SetGlobalTexture(ShaderIDs._APVResL1B_L1Rz, rr.L1_B_rz);
                    cmdBuffer.SetGlobalTexture(ShaderIDs._APVResValidity, rr.Validity);

                    cmdBuffer.SetGlobalTexture(ShaderIDs._SkyOcclusionTexL0L1, rr.SkyOcclusionL0L1 ?? (RenderTargetIdentifier)TextureXR.GetBlackTexture3D());
                    cmdBuffer.SetGlobalTexture(ShaderIDs._SkyShadingDirectionIndicesTex, rr.SkyShadingDirectionIndices ?? (RenderTargetIdentifier)TextureXR.GetBlackTexture3D());
                    cmdBuffer.SetGlobalBuffer(ShaderIDs._SkyPrecomputedDirections, rr.SkyPrecomputedDirections);

                    if (refVolume.shBands == ProbeVolumeSHBands.SphericalHarmonicsL2)
                    {
                        cmdBuffer.SetGlobalTexture(ShaderIDs._APVResL2_0, rr.L2_0);
                        cmdBuffer.SetGlobalTexture(ShaderIDs._APVResL2_1, rr.L2_1);
                        cmdBuffer.SetGlobalTexture(ShaderIDs._APVResL2_2, rr.L2_2);
                        cmdBuffer.SetGlobalTexture(ShaderIDs._APVResL2_3, rr.L2_3);
                    }

                    needToBindNeutral = false;
                }
            }

            if (needToBindNeutral)
            {
                // Lazy init the empty buffer. We use sizeof(uint3) so that this buffer can be
                // used with uint and uint3 bindings without triggering validation errors.
                if (m_EmptyIndexBuffer == null)
                    m_EmptyIndexBuffer = new ComputeBuffer(1, sizeof(uint) * 3, ComputeBufferType.Structured);

                if(m_EmptyDirectionsBuffer == null)
                {
                    m_EmptyDirectionsBuffer = new ComputeBuffer(1, 3 * sizeof(float), ComputeBufferType.Structured);
                }

                cmdBuffer.SetGlobalBuffer(ShaderIDs._APVResIndex, m_EmptyIndexBuffer);
                cmdBuffer.SetGlobalBuffer(ShaderIDs._APVResCellIndices, m_EmptyIndexBuffer);

                cmdBuffer.SetGlobalTexture(ShaderIDs._APVResL0_L1Rx, TextureXR.GetBlackTexture3D());

                cmdBuffer.SetGlobalTexture(ShaderIDs._APVResL1G_L1Ry, TextureXR.GetBlackTexture3D());
                cmdBuffer.SetGlobalTexture(ShaderIDs._APVResL1B_L1Rz, TextureXR.GetBlackTexture3D());
                cmdBuffer.SetGlobalTexture(ShaderIDs._APVResValidity, TextureXR.GetBlackTexture3D());

                cmdBuffer.SetGlobalTexture(ShaderIDs._SkyOcclusionTexL0L1, TextureXR.GetBlackTexture3D());
                cmdBuffer.SetGlobalTexture(ShaderIDs._SkyShadingDirectionIndicesTex, TextureXR.GetBlackTexture3D());
                cmdBuffer.SetGlobalBuffer(ShaderIDs._SkyPrecomputedDirections, m_EmptyDirectionsBuffer);

                if (refVolume.shBands == ProbeVolumeSHBands.SphericalHarmonicsL2)
                {
                    cmdBuffer.SetGlobalTexture(ShaderIDs._APVResL2_0, TextureXR.GetBlackTexture3D());
                    cmdBuffer.SetGlobalTexture(ShaderIDs._APVResL2_1, TextureXR.GetBlackTexture3D());
                    cmdBuffer.SetGlobalTexture(ShaderIDs._APVResL2_2, TextureXR.GetBlackTexture3D());
                    cmdBuffer.SetGlobalTexture(ShaderIDs._APVResL2_3, TextureXR.GetBlackTexture3D());
                }
            }
        }

        /// <summary>
        /// Update the constant buffer used by Probe Volumes in shaders.
        /// </summary>
        /// <param name="cmd">A command buffer used to perform the data update.</param>
        /// <param name="probeVolumeOptions">probe volume options from the active volume stack</param>
        /// <param name="taaFrameIndex">TAA frame index</param>
        /// <returns>True if successful</returns>
        public bool UpdateShaderVariablesProbeVolumes(CommandBuffer cmd, ProbeVolumesOptions probeVolumeOptions, int taaFrameIndex)
        {
            bool enableProbeVolumes = DataHasBeenLoaded();

            if (enableProbeVolumes)
            {
                ProbeVolumeShadingParameters parameters;
                parameters.normalBias = probeVolumeOptions.normalBias.value;
                parameters.viewBias = probeVolumeOptions.viewBias.value;
                parameters.scaleBiasByMinDistanceBetweenProbes = probeVolumeOptions.scaleBiasWithMinProbeDistance.value;
                parameters.samplingNoise = probeVolumeOptions.samplingNoise.value;
                parameters.weight = probeVolumeOptions.intensityMultiplier.value;
                parameters.leakReductionMode = probeVolumeOptions.leakReductionMode.value;
                parameters.frameIndexForNoise = taaFrameIndex * (probeVolumeOptions.animateSamplingNoise.value ? 1 : 0);
                parameters.reflNormalizationLowerClamp = 0.005f;
                parameters.reflNormalizationUpperClamp = probeVolumeOptions.occlusionOnlyReflectionNormalization.value ? 1.0f : 7.0f;

                parameters.minValidNormalWeight = probeVolumeOptions.minValidDotProductValue.value;
                parameters.skyOcclusionIntensity = skyOcclusion ? probeVolumeOptions.skyOcclusionIntensityMultiplier.value : 0.0f;
                parameters.skyOcclusionShadingDirection = skyOcclusion && skyOcclusionShadingDirection;
                UpdateConstantBuffer(cmd, parameters);
            }

            return enableProbeVolumes;
        }
    }
}
