using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;

[RequireComponent(typeof(Camera))]
public class HighlightsFX : MonoBehaviour 
{
	#region enums

	public enum HighlightType
	{
		Glow = 0,
		Solid = 1
	}

	public enum SortingType
	{
		Overlay = 3,
		DepthFiltered = 4,
	}

    public enum DepthInvertPass
    {
        StencilMapper = 5,
        StencilDrawer = 6
    }

	public enum FillType
	{
		Fill,
		Outline
	}
	public enum RTResolution
	{
		Quarter = 4,
		Half = 2,
		Full = 1
	}

    public enum BlurType
    {
        StandardGauss = 0,
        SgxGauss = 1,
    }

    public struct OutlineData
    {
        public Color color;
        public SortingType sortingType;
    }

    #endregion

    
    [Header("Outline Settings")]

    public HighlightType m_selectionType = HighlightType.Glow;
	public FillType m_fillType = FillType.Outline;
	public RTResolution m_resolution = RTResolution.Full;
    [Range(0f, 1f)]
    public float m_controlValue = 0.5f;
    public CameraEvent BufferDrawEvent = CameraEvent.BeforeImageEffects;

    [Header("BlurOptimized Settings")]

    public BlurType blurType = BlurType.StandardGauss;
    [Range(0, 2)]
    public int downsample = 0;
    [Range(0.0f, 10.0f)]
    public float blurSize = 3.0f;
    [Range(1, 4)]
    public int blurIterations = 2;
    

    private CommandBuffer m_commandBuffer;

    private int m_highlightRTID, m_blurredRTID, m_temporaryRTID;

    private List<Renderer> m_setObjectRenderers;
	private List<Renderer> m_objectRenders;

	public Material m_highlightMaterial;
	public Material m_blurMaterial;		
	public Camera m_camera;

	private int m_RTWidth = 512;
	private int m_RTHeight = 512;

    private RenderTexture m_highlightRT, m_blurredRT, m_temporaryRT;
	
	private bool isOutline = false;
	private int num;

	public void AddRender(Renderer render)
	{
		m_objectRenders.Add(render);
	}
	
	public void ShowOutLine()
	{
		if (isOutline == true) return;
		isOutline = true;
		m_camera.enabled = true;
		OnViewOutLine();
	}
	public void HideOutLine()
	{
		if (isOutline == false) return;
		isOutline = false;
		ClearOutlineData();
		m_camera.enabled = false;
	}

	private void OnViewOutLine() 
	{
		m_setObjectRenderers.Clear();
		for (num = 0; num < m_objectRenders.Count; num++)
		{ m_setObjectRenderers.Add(m_objectRenders[num]); }
		RecreateCommandBuffer();
	}
	
    private void ClearOutlineData()
    {
        m_setObjectRenderers.Clear();
        RecreateCommandBuffer();
	}
	
	public void Init()
	{
		m_setObjectRenderers = new List<Renderer>();
		m_objectRenders = new List<Renderer>();

		m_commandBuffer = new CommandBuffer();
		m_commandBuffer.name = "HighlightFX Command Buffer";

		m_highlightRTID = Shader.PropertyToID("_HighlightRT");
		m_blurredRTID = Shader.PropertyToID("_BlurredRT");
		m_temporaryRTID = Shader.PropertyToID("_TemporaryRT");

		//m_highlightMaterial = new Material(Shader.Find("Custom/Highlight"));
		//m_blurMaterial = new Material(Shader.Find("Hidden/FastBlur"));

		//m_camera.depthTextureMode = DepthTextureMode.Depth;
		m_RTWidth = -1;// (int)(Screen.width / (float)m_resolution);
		m_RTHeight = -1;// (int)(Screen.height / (float)m_resolution);
		m_camera.AddCommandBuffer(BufferDrawEvent, m_commandBuffer);
		m_camera.enabled = false;
		m_camera.renderingPath = RenderingPath.DeferredLighting;
		//Debug.Log(m_camera.renderingPath);
		//m_camera.renderingPath = RenderingPath.DeferredShading;
	}

	private void RecreateCommandBuffer()
    {
        m_commandBuffer.Clear();

        if (m_setObjectRenderers.Count == 0) return;
		
		// rendering into texture
		m_commandBuffer.GetTemporaryRT(m_highlightRTID, m_RTWidth, m_RTHeight, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32);
		m_commandBuffer.SetRenderTarget(m_highlightRTID, BuiltinRenderTextureType.CameraTarget);
		m_commandBuffer.ClearRenderTarget(false, true, Color.clear);
		m_commandBuffer.SetGlobalColor("_Color", Color.white); //외각선 색
		
		int i;
		for (i = 0; i < m_setObjectRenderers.Count; i++)
		{
			m_commandBuffer.DrawRenderer(m_setObjectRenderers[i], m_highlightMaterial, 0, (int)SortingType.Overlay);
		}
		
        // excluding from texture 
        m_commandBuffer.SetGlobalColor("_Color", Color.clear);
       
        float widthMod = 1.0f / (1.0f * (1 << downsample));

		int rtW = m_camera.pixelWidth;// -1;// m_RTWidth >> downsample;
		int rtH = m_camera.pixelHeight;//-1;// m_RTHeight >> downsample;
   
        m_commandBuffer.GetTemporaryRT(m_blurredRTID, rtW, rtH, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32);
		m_commandBuffer.GetTemporaryRT(m_temporaryRTID, rtW, rtH, 0, FilterMode.Bilinear, RenderTextureFormat.ARGB32);//ARGB32);
		
		m_commandBuffer.Blit(m_highlightRTID, m_temporaryRTID, m_blurMaterial, 0);

        int passOffs = blurType == BlurType.StandardGauss ? 0 : 2;
		float iterationOffs;
		float blurHorizParam;
		float blurVertParam;
		Vector4 vec4 = Vector4.zero;
        for (i = 0; i < blurIterations; i++)
        {
            iterationOffs = (i * 1.0f);
            blurHorizParam = blurSize * widthMod + iterationOffs;
            blurVertParam = -blurSize * widthMod - iterationOffs;
			vec4.x = blurHorizParam;
			vec4.y = blurVertParam;
			m_commandBuffer.SetGlobalVector("_Parameter", vec4);

            m_commandBuffer.Blit(m_temporaryRTID, m_blurredRTID, m_blurMaterial, 1 + passOffs);
			//m_commandBuffer.Blit(m_blurredRTID, m_temporaryRTID, m_blurMaterial, 2 + passOffs); //블러효과..
        }
		
		// occlusion

		if (m_fillType == FillType.Outline)
        {
            // Excluding the original image from the blurred image, leaving out the areal alone
            m_commandBuffer.SetGlobalTexture("_SecondaryTex", m_highlightRTID);
            m_commandBuffer.Blit(m_temporaryRTID, m_blurredRTID, m_highlightMaterial, 2);
            m_commandBuffer.SetGlobalTexture("_SecondaryTex", m_blurredRTID);
        }
        else
        {
            m_commandBuffer.SetGlobalTexture("_SecondaryTex", m_temporaryRTID);
        }

        // back buffer
        m_commandBuffer.Blit(BuiltinRenderTextureType.CameraTarget, m_highlightRTID);

        // overlay
        m_commandBuffer.SetGlobalFloat("_ControlValue", m_controlValue);
        m_commandBuffer.Blit(m_highlightRTID, BuiltinRenderTextureType.CameraTarget, m_highlightMaterial, (int)m_selectionType);
		
		m_commandBuffer.ReleaseTemporaryRT(m_temporaryRTID);
        m_commandBuffer.ReleaseTemporaryRT(m_blurredRTID);
        m_commandBuffer.ReleaseTemporaryRT(m_highlightRTID);
	}
}
