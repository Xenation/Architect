using UnityEngine;
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

		public override void Render(PostProcessRenderContext context) {

			//RenderTexture tmp = context.GetScreenSpaceTemporaryRT();
			//context.camera.targetTexture = tmp;
			//int prevMask = context.camera.cullingMask;
			//context.camera.cullingMask = LayerMask.GetMask("Outline");
			//context.camera.Render();
			//context.camera.cullingMask = prevMask;
			//context.camera.targetTexture = null;
			
			//PropertySheet sheet = context.propertySheets.Get(Shader.Find("Hidden/Custom/Outline"));
			//sheet.properties.SetFloat("_Blend", settings.blend);
			//sheet.properties.SetTexture("_OutlineTex", tmp);
			//context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);

			//RenderTexture.ReleaseTemporary(tmp);
		}

	}
}
