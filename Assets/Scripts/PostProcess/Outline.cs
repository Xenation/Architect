using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.XR;

namespace Architect {
	[System.Serializable]
	[PostProcess(typeof(OutlineRenderer), PostProcessEvent.AfterStack, "Custom/Outlines")]
	public sealed class Outline : PostProcessEffectSettings {

		[Range(0f, 1f), Tooltip("Outline Effect Intensity")]
		public FloatParameter blend = new FloatParameter { value = 1f };

		[Range(0, 4), Tooltip("The number of times the resolution is halved for the blur")]
		public IntParameter blurDownscale = new IntParameter { value = 1 };

		[Range(1, 8), Tooltip("The number of blur passes executed (higher = more blurry)")]
		public IntParameter blurPasses = new IntParameter { value = 4 };

		public override bool IsEnabledAndSupported(PostProcessRenderContext context) {
			return enabled.value && blend.value > 0f;
		}

	}
	
	public sealed class OutlineRenderer : PostProcessEffectRenderer<Outline> {

		private bool isValid = true;

		private Shader outlineUnlit;
		private Shader blur;
		private Shader outlineCompositor;

		private int outlineColorID;
		private int blurTexID;
		private int tmpTexID;
		private int blurSizeID;
		private int outlineTexID;
		private int blendID;

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
			blurTexID = Shader.PropertyToID("_BlurTex");
			tmpTexID = Shader.PropertyToID("_TmpTex");
			blurSizeID = Shader.PropertyToID("_BlurSize");
			outlineTexID = Shader.PropertyToID("_OutlineTex");
			blendID = Shader.PropertyToID("_OutlineBlend");

			// Create the materials used
			outlineUnlitMaterial = new Material(outlineUnlit);
			blurMaterial = new Material(blur);
			outlineCompositorMaterial = new Material(outlineCompositor);
		}

		public override void Render(PostProcessRenderContext context) {
			if (!isValid) { // Avoid applying anything if some shaders are missing
				context.command.Blit(context.source, context.destination);
				return;
			}

			// Fix for exit play mode destroying materials
			if (outlineUnlitMaterial == null) {
				outlineUnlitMaterial = new Material(outlineUnlit);
			}
			if (blurMaterial == null) {
				blurMaterial = new Material(blur);
			}
			if (outlineCompositorMaterial == null) {
				outlineCompositorMaterial = new Material(outlineCompositor);
			}

			// Prepare blur RT descriptor
			RenderTextureDescriptor blurDescriptor;
			//if (context.camera.stereoEnabled) {
			//	blurDescriptor = XRSettings.eyeTextureDesc;
			//	blurDescriptor.depthBufferBits = 0;
			//} else {
			//	blurDescriptor = new RenderTextureDescriptor(context.camera.pixelWidth, context.camera.pixelHeight);
			//}
			blurDescriptor = new RenderTextureDescriptor(context.camera.pixelWidth * ((context.camera.stereoEnabled) ? 2 : 1), context.camera.pixelHeight);
			blurDescriptor.width = blurDescriptor.width >> settings.blurDownscale;
			blurDescriptor.height = blurDescriptor.height >> settings.blurDownscale;

			context.command.BeginSample("Outline");

			// Render the outlined objects in the outline RT
			//context.GetScreenSpaceTemporaryRT(context.command, outlineTexID);
			context.command.GetTemporaryRT(outlineTexID, new RenderTextureDescriptor(context.camera.pixelWidth * ((context.camera.stereoEnabled) ? 2 : 1), context.camera.pixelHeight));
			context.command.SetRenderTarget(outlineTexID);
			context.command.ClearRenderTarget(true, true, Color.clear);
			foreach (Outlined outlined in OutlinedManager.I.outlinedObjects) {
				if (!outlined.enabled) continue;
				context.command.SetGlobalColor(outlineColorID, outlined.outlineColor);
				foreach (Renderer renderer in outlined.GetComponentsInChildren<Renderer>()) { // TODO use more optimized method
					context.command.DrawRenderer(renderer, outlineUnlitMaterial);
				}
			}

			// Blur the outlined RT
			context.command.GetTemporaryRT(blurTexID, blurDescriptor, FilterMode.Bilinear);
			context.command.GetTemporaryRT(tmpTexID, blurDescriptor, FilterMode.Bilinear);
			context.command.Blit(outlineTexID, blurTexID); // Copy outline RT to blur RT
			context.command.SetGlobalVector(blurSizeID, new Vector4(1.5f / blurDescriptor.width, 1.5f / blurDescriptor.height, 0f, 0f));

			for (int i = 0; i < settings.blurPasses; i++) { // Blur passes
				context.command.Blit(blurTexID, tmpTexID, blurMaterial, 0); // Horizontal
				context.command.Blit(tmpTexID, blurTexID, blurMaterial, 1); // Vertical
			}
			context.command.ReleaseTemporaryRT(tmpTexID);

			// Composite the main RT with the blur RT
			context.command.SetGlobalFloat(blendID, settings.blend);
			context.command.Blit(context.source, context.destination, outlineCompositorMaterial, 0);
			context.command.ReleaseTemporaryRT(blurTexID);
			context.command.ReleaseTemporaryRT(outlineTexID);

			context.command.EndSample("Outline");
		}

	}
}
