Shader "Custom/Mask"
{
	SubShader{

		Tags {
			"Queue" = "Transparent-2"
		}
		Cull Off

		Pass {
			Blend Zero One
		}
	}
}