using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;

namespace Architect {
	[System.Serializable]
	[PostProcess(typeof(OutlineRenderer), PostProcessEvent.AfterStack, "Custom/Outlines")]
	public sealed class Outline : PostProcessEffectSettings {

		[Range(0f, 1f), Tooltip("Outline Effect Intensity")]
		public FloatParameter blend = new FloatParameter { value = 0.5f };

		[Tooltip("Outline Color")]
		public ColorParameter outlineColor = new ColorParameter { value = Color.green };

		public override bool IsEnabledAndSupported(PostProcessRenderContext context) {
			return enabled.value && blend.value > 0f;
		}

	}

	public sealed class OutlineRenderer : PostProcessEffectRenderer<Outline> {

		private struct CameraRT {
			public RenderTexture main;
			public RenderTexture outline;
			public RenderTargetBinding outlinePassBinding;
			public RenderTargetIdentifier outlineColorBufferIdentifier;
			public RenderTextureDescriptor blurDescriptor;
		}

		private bool isValid = true;

		private Dictionary<Camera, CameraRT> renderTextures;

		private Shader outlineUnlit;
		private Shader blur;
		private Shader outlineCompositor;

		private int outlineColorID;
		private int blurPassTexID;
		private int tmpTexID;
		private int blurSizeID;
		private int outlineTexID;

		private Material outlineUnlitMaterial;
		private Material blurMaterial;
		private Material outlineCompositorMaterial;

		public override void Init() {
			// Find the used shaders
			outlineUnlit = Shader.Find("Hidden/Custom/OutlineUnlit");
			blur = Shader.Find("Hidden/Custom/Blur");
			outlineCompositor = Shader.Find("Hidden/Custom/OutlineCompositor");
			if (outlineUnlit == null || blur == null || outlineCompositor == null) {
				isValid = false;
				return;
			}

			// Find the used shader properties
			outlineColorID = Shader.PropertyToID("_OutlineColor");
			blurPassTexID = Shader.PropertyToID("_BlurTex");
			tmpTexID = Shader.PropertyToID("_TmpTex");
			blurSizeID = Shader.PropertyToID("_BlurSize");
			outlineTexID = Shader.PropertyToID("_OutlineTex");

			// Create the materials used
			outlineUnlitMaterial = new Material(outlineUnlit);
			blurMaterial = new Material(blur);
			outlineCompositorMaterial = new Material(outlineCompositor);

			InitializeRenderTextures();
		}

		private void InitializeRenderTextures() {
			renderTextures = new Dictionary<Camera, CameraRT>();
		}

		public override void Render(PostProcessRenderContext context) {
			if (!isValid) {
				context.command.Blit(context.source, context.destination);
				return;
			}

			CameraRT rt;
			if (!renderTextures.TryGetValue(context.camera, out rt)) {
				rt = InitializeCameraRT(context.camera);
			}

			// Render the outlined objects in the outline RT
			context.command.SetRenderTarget(rt.outlinePassBinding);
			context.command.ClearRenderTarget(true, true, Color.clear);
			foreach (Outlined outlined in OutlinedManager.I.outlinedObjects) {
				if (!outlined.enabled) continue;
				context.command.SetGlobalColor(outlineColorID, outlined.outlineColor);
				foreach (Renderer renderer in outlined.GetComponentsInChildren<Renderer>()) { // TODO by more optimized method
					context.command.DrawRenderer(renderer, outlineUnlitMaterial);
				}
			}

			// Blur the outlined RT
			context.command.GetTemporaryRT(blurPassTexID, rt.blurDescriptor, FilterMode.Bilinear);
			context.command.GetTemporaryRT(tmpTexID, rt.blurDescriptor, FilterMode.Bilinear);
			context.command.Blit(rt.outlineColorBufferIdentifier, blurPassTexID); // Copy outline RT to blur RT

			context.command.SetGlobalVector(blurSizeID, new Vector4(1.5f / rt.blurDescriptor.width, 1.5f / rt.blurDescriptor.height, 0f, 0f));

			for (int i = 0; i < 4; i++) { // Blur passes
				context.command.Blit(blurPassTexID, tmpTexID, blurMaterial, 0); // Horizontal
				context.command.Blit(tmpTexID, blurPassTexID, blurMaterial, 1); // Vertical
			}

			// Composite the main RT with the blur RT
			context.command.SetGlobalTexture(outlineTexID, blurPassTexID);
			context.command.Blit(context.source, context.destination, outlineCompositorMaterial);
		}

		private CameraRT InitializeCameraRT(Camera camera) {
			RenderTexture mainRT = camera.targetTexture;
			// Create a render texture with no depth
			RenderTextureDescriptor desc = camera.targetTexture.descriptor;
			desc.depthBufferBits = 0;
			RenderTexture outlineRT = new RenderTexture(desc);
			// Create the bindings, identifiers and descriptors
			RenderTargetSetup rtSetup = new RenderTargetSetup(outlineRT.colorBuffer, mainRT.depthBuffer);
			RenderTargetBinding outlineBinding = new RenderTargetBinding(rtSetup);
			RenderTargetIdentifier outlineColorIdentifier = new RenderTargetIdentifier(outlineRT.colorBuffer);
			RenderTextureDescriptor blurDescriptor = outlineRT.descriptor;
			blurDescriptor.height = blurDescriptor.height >> 1;
			blurDescriptor.width = blurDescriptor.width >> 1;
			// Create the 
			CameraRT rt = new CameraRT { main = mainRT, outline = outlineRT, outlinePassBinding = outlineBinding, outlineColorBufferIdentifier = outlineColorIdentifier, blurDescriptor = blurDescriptor };
			renderTextures.Add(camera, rt);
			return rt;
		}

		public override void Release() {
			ReleaseRenderTextures();
		}

		private void ReleaseRenderTextures() {
			foreach (CameraRT rt in renderTextures.Values) {
				rt.outline.Release();
			}
			renderTextures = null;
		}

	}
}
