// �s�N�Z�� �V�F�[�_�[�� 1 �����K�E�X �u���[ �t�B���^�[��K�p���܂��B
// ����̓u���[�� �|�X�g�v���Z�X�ɂ���āA�ŏ��͐��������ɁA
// ���͐��������Ƀu���[��K�p���邽�߂Ɍv 2 ��g�p����܂��B

sampler TextureSampler : register(s0);

#define SAMPLE_COUNT 15

float2 SampleOffsets[SAMPLE_COUNT];
float SampleWeights[SAMPLE_COUNT];


float4 PixelShaderFunction(float2 texCoord : TEXCOORD0) : COLOR0
{
    float4 c = 0;
    
    // �d�ݕt�����ꂽ�����̃C���[�W �t�B���^�[ �^�b�v���������܂��B
    for (int i = 0; i < SAMPLE_COUNT; i++)
    {
        c += tex2D(TextureSampler, texCoord + SampleOffsets[i]) * SampleWeights[i];
    }
    
    return c;
}


technique GaussianBlur
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
