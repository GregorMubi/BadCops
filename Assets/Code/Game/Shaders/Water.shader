Shader "BadCops/Water"
{
    Properties
    {
		_Color("Color", Color) = (1, 1, 1, 1)
		_EdgeColor("Edge Color", Color) = (1, 1, 1, 1)
		_DepthFactor("Depth Factor", float) = 1.0

		_NoiseTex("Noise Texture", 2D) = "white" {}
		_WaveSpeed("Wave Speed", float) = 1.0
		_WaveAmplitude("Wave Amplitude", float) = 1.0
		_ExtraHeight("Extra Height", float) = 1.0

    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

		Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
				float4 noiseCoord : TEXCOORD1;
            };

            struct v2f
			{
				float4 screenPos : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

			sampler2D _CameraDepthTexture;

			float4  _Color;
			float4  _EdgeColor;
			float  _DepthFactor;

			float _WaveSpeed;
			float _WaveAmplitude;
			float _ExtraHeight;
			sampler2D _NoiseTex;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.screenPos = ComputeScreenPos(o.vertex);

				float4 noiseTex = tex2Dlod(_NoiseTex, float4(v.noiseCoord.xy, 0, 0));
				o.vertex.x += cos(_Time.y * _WaveSpeed * noiseTex) * _WaveAmplitude;
				o.vertex.y += sin(_Time.y * _WaveSpeed * noiseTex) * _WaveAmplitude + _ExtraHeight;

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {

				float4 depthSample = SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, i.screenPos);
				float depth = LinearEyeDepth(depthSample).r;
				float foamLine = 1 - saturate(_DepthFactor * (depth - i.screenPos.w));
				
				//return float4(foamLine, foamLine, foamLine, 0.5);

				float4 col = _Color + foamLine * _EdgeColor;
                return col;
            }
            ENDCG
        }
    }
}
