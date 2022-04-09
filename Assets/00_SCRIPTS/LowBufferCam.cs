using UnityEngine;
using UnityEngine.Rendering;

namespace StarwarsGraphics
{
	[ImageEffectAllowedInSceneView]
	[ExecuteInEditMode, RequireComponent(typeof(Camera))]
    public class LowBufferCam : MonoBehaviour
    {
        public Camera cam;
		//private CommandBuffer hookBuffer;

		/*
        void OnEnable()
        {
			//SetCameraBuffer();
#if !UNITY_EDITOR
			//rectBgPlane.gameObject.SetActive(false);
#endif
		}
		*/
		/*
        void SetCameraBuffer()
        {
            int screenCopyID = Shader.PropertyToID("_ScreenCopyTexture");
            int halfCopyID = Shader.PropertyToID("_HalfCopyTexture");

            if (cam == null)
                cam = GetComponent<Camera>();

            if (hookBuffer == null)
                hookBuffer = new CommandBuffer { name = "Grab Half Buffer" };

            hookBuffer.Clear();
            cam.RemoveCommandBuffer(CameraEvent.AfterForwardOpaque, hookBuffer);
            
            hookBuffer.GetTemporaryRT(screenCopyID, -1, -1, 0, FilterMode.Bilinear);
            hookBuffer.Blit(BuiltinRenderTextureType.CurrentActive, screenCopyID);

            hookBuffer.GetTemporaryRT(halfCopyID, -2, -2, 0, FilterMode.Bilinear);
            hookBuffer.Blit(screenCopyID, halfCopyID);
            hookBuffer.ReleaseTemporaryRT(screenCopyID);

            hookBuffer.SetGlobalTexture("_GrabHalfBuffer", halfCopyID);

            cam.AddCommandBuffer(CameraEvent.AfterForwardOpaque, hookBuffer);
		}
		*/
		public RectTransform rectBgPlane;
		
		/*
		public GameObject objBack;
		public Material blurCam;
		private Texture2D tex;
		private RenderTexture tempDown1;
		//private RenderTexture tempDown2;
		private RenderTexture tempUp1;
		//private RenderTexture tempUp2;
		public UnityEngine.UI.Image imgRewardBgPlane;
		private const string strBluredTex = "_BluredTex";
		private Rect tmpRect;
		*/

#if UNITY_EDITOR
		/*
		void OnPostRender()
		{
			if (tex == null)
			{
				tex = new Texture2D(Screen.width, Screen.height);
				blurCam.SetFloat("_SampleScale", 1.5f);
			}
			//objBack.SetActive(false);
			//if (GameMgr.ins != null) GameMgr.ins.mgrUI.panelMove.SetActive(false);
			
			tex.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
			tex.Apply();
			tempDown1 = new RenderTexture(tex.width / 3, tex.height / 3, 0, RenderTextureFormat.Default);
			Graphics.Blit(tex, tempDown1, blurCam, 0);
			
			//tempDown2 = new RenderTexture(tempDown1.width / 2, tempDown1.height / 2, 0, RenderTextureFormat.Default);
			//Graphics.Blit(tempDown1, tempDown2, blurCam, 0);

			//tempUp2 = new RenderTexture(tempDown2.width, tempDown2.height, 0, RenderTextureFormat.Default);
			//Graphics.Blit(tempDown2, tempUp2, blurCam, 1);

			//tempUp1 = new RenderTexture(tempDown1.width, tempDown1.height, 0, RenderTextureFormat.Default);
			//Graphics.Blit(tempUp2, tempUp1, blurCam, 1);
			
			//tempDown1.Release();
			//tempDown2.Release();
			//tempUp2.Release();
			
			//imgRewardBgPlane.material.SetTexture(strBluredTex, tempUp1);
			
			tempUp1 = new RenderTexture(tempDown1.width, tempDown1.height, 0, RenderTextureFormat.Default);
			Graphics.Blit(tempDown1, tempUp1, blurCam, 0);
			tempDown1.Release();
			
			imgRewardBgPlane.material.SetTexture(strBluredTex, tempUp1);

			//objBack.SetActive(true);
			//if (GameMgr.ins != null) GameMgr.ins.mgrUI.panelMove.SetActive(true);
		}
		*/
#endif

		/*
		void OnDisable()
        {
            if (cam != null)
            {
                if (hookBuffer != null)
                {
                    cam.RemoveCommandBuffer(CameraEvent.AfterForwardOpaque, hookBuffer);
                }
            }
        }
		*/
	}
}