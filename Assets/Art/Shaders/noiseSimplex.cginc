// Simplex noise generation code
// This is an example implementation of Simplex noise in HLSL

// Permutation table
static const float2 grad2[8] = {
    float2(1, 1), float2(-1, 1), float2(1, -1), float2(-1, -1),
    float2(1, 0), float2(-1, 0), float2(0, 1), float2(0, -1)
};

float2 fade(float2 t) {
    return t * t * t * (t * (t * 6 - 15) + 10);
}

float grad(int hash, float2 p) {
    return dot(grad2[hash & 7], p);
}

float snoise(float2 p) {
    const float F2 = 0.366025403f; // 0.5 * (sqrt(3.0) - 1.0)
    const float G2 = 0.211324865f; // (3.0 - sqrt(3.0)) / 6.0
    
    float2 s = p + float2(F2, F2) * dot(p, float2(F2, F2));
    float2 i = floor(s);
    float2 x0 = p - i + float2(G2, G2) * dot(i, float2(G2, G2));
    
    float2 i1;
    if (x0.x > x0.y) i1 = float2(1, 0); else i1 = float2(0, 1);
    
    float2 x1 = x0 - i1 + G2;
    float2 x2 = x0 - float2(1.0, 1.0) + 2.0 * G2;
    
    int h0 = (int)i.x + (int)i.y;
    int h1 = h0 + (int)i1.x + (int)i1.y;
    int h2 = h0 + 1;
    
    float n0 = grad(h0, x0);
    float n1 = grad(h1, x1);
    float n2 = grad(h2, x2);
    
    float t0 = 0.5 - dot(x0, x0);
    float t1 = 0.5 - dot(x1, x1);
    float t2 = 0.5 - dot(x2, x2);
    
    float noise = 0.0;
    if (t0 >= 0.0) noise += t0 * t0 * t0 * t0 * n0;
    if (t1 >= 0.0) noise += t1 * t1 * t1 * t1 * n1;
    if (t2 >= 0.0) noise += t2 * t2 * t2 * t2 * n2;
    
    return 70.0 * noise;
}
