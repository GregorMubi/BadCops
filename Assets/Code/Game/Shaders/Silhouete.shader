Shader "Silhouete/Silhouete"
{
    Properties
    {
		_Color ("Color", Color) = (1,1,1,1)
    }
    SubShader
    {
		// Grab the screen behind the object into _BackgroundTexture
		GrabPass
		{
			"_BackgroundTexture"
		}

		Pass
        {
			Stencil 
            {
                Ref 69
                Comp Equal
            }

			ZTest GEqual	  //to draw only behind walls
			Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
				float4 grabPos : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				// to get the correct texture coordinate
				o.grabPos = ComputeGrabScreenPos(o.vertex);
                return o;
            }

			float4 _Color;
			sampler2D _BackgroundTexture;

            fixed4 frag (v2f i) : SV_Target
            {
				half4 bgcolor = tex2Dproj(_BackgroundTexture, i.grabPos);
				float4 color = lerp(bgcolor, _Color, 0.2);
                return color;
            }
            ENDCG
        }
    }
}
