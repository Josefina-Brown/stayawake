Shader "Hidden/PixelationEffect"
{
    Properties
    {
        _PixelSize ("Pixel Size", Range(1, 32)) = 8
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float _PixelSize;

            float4 frag(v2f_img i) : SV_Target
            {
                float2 uv = floor(i.uv * _PixelSize) / _PixelSize;
                return tex2D(_MainTex, uv);
            }
            ENDCG
        }
    }
}
