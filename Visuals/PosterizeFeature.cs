using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PosterizeFeature : ScriptableRendererFeature
{
    class PosterizePass : ScriptableRenderPass
    {
        Material material;
        RTHandle cameraColorTarget;
        RTHandle tempRT;

        public float levels = 4;

        public PosterizePass(Material mat)
        {
            material = mat;
            renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            cameraColorTarget = renderingData.cameraData.renderer.cameraColorTargetHandle;
            tempRT = RTHandles.Alloc(
                renderingData.cameraData.cameraTargetDescriptor,
                name: "_TempPosterizeRT"
            );
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (material == null) return;

            CommandBuffer cmd = CommandBufferPool.Get("PosterizePass");

            material.SetFloat("_PosterizeLevels", levels);

            Blitter.BlitCameraTexture(cmd, cameraColorTarget, tempRT, material, 0);
            Blitter.BlitCameraTexture(cmd, tempRT, cameraColorTarget);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            if (tempRT != null)
                RTHandles.Release(tempRT);
        }
    }

    [System.Serializable]
    public class PosterizeSettings
    {
        public Material material;
        [Range(2, 16)] public float levels = 4;
    }

    public PosterizeSettings settings = new PosterizeSettings();
    PosterizePass pass;

    public override void Create()
    {
        if (settings.material == null) return;
        pass = new PosterizePass(settings.material);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (settings.material == null) return;
        pass.levels = settings.levels;
        renderer.EnqueuePass(pass);
    }
}
