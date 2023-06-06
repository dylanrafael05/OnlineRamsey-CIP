﻿//MATH
#define PI 3.14159265
#define eps float2(0.00001, 0.)

float snap(float v, float grid)
{
	return v - fmod(v, grid);
}
float2 snap(float2 v, float2 grid)
{
	return v - fmod(v, grid);
}

float fract(float v)
{
	return fmod(v, 1.);
}

float hash11(float p)
{
	p = fract(p * .1031);
	p *= p + 33.33;
	p *= p + p;
	return fract(p);
}

float2 fold(float2 p, float2 normal) //negative to positive only
{
	return p - 2. * min(0., dot(p, normal)) * normal;
}

float2 perp(float2 v)
{
	return cross(float3(v.x, v.y, 0.), float3(0., 0., -1.)).xy;
}

float2 rot(float2 v, float o)
{
	return cos(o) * v + sin(o) * perp(v);
}

//SDFs
float sdCircle(float2 p, float radius)
{
	return length(p) - radius;
}

float sdLine(float2 p, float4 points, float thickness)
{

	float2 lp = p - (points.xy + points.zw) * .5;
	float2 dir = normalize(points.zw - points.xy);
	lp = float2(dot(lp, dir), length(cross(float3(lp.x, lp.y, 0.), float3(dir.x, dir.y, 0.))));
	lp = abs(lp);
	lp.x = max(0., lp.x - length(points.zw - points.xy) * .5);
	return length(lp) - thickness;

}

float sdPartition(float2 p, float2 lineDir) //2D plane
{
	return dot(p, perp(lineDir));
}

float sdRightTri(float2 p, float2 corner, float2 dim) //x ~ [0, inf)
{

	p = p - corner;

	float2 top = float2(0., dim.y);
	float2 right = float2(dim.x, 0.);

	return max(sdPartition(p - top, normalize(right - top)), sdPartition(p, -normalize(right)));

}

float sdPolarLine(float2 radii, float thetaThickness, float2 p) //atan2(p) ~ [-thetaThickness*.5, thetaThickness*.5]
{
	float r = length(p);

	float2 space = float2(sin(thetaThickness * .5), cos(thetaThickness * .5));

	float2 start = radii.x * space * float2(-1., 1.);
	float2 end = radii.y * space;

	return sdPartition(p - start, normalize(end - start));

	float m = (end.y - start.y) / (end.x - start.x);

	float x = (start.x * m - start.y) / (m - p.y / p.x);
	float y = x * p.y / p.x;

	return r - length(float2(x, y)); //intersection on the other side of the polar ray
}

float sdSpikeBall(float2 r, float partitions, float2 p, float variation, float seed) //radii [lowRadius, highRadius]
{
	float randSeed = hash11(seed);
	p = rot(p, -(_Time.y*(1.+randSeed) + randSeed * 100.) * 2.);
	float2 polar = float2(length(p), atan2(p.y, p.x)+2.*PI);
	float ltheta = fmod(polar.y, 2. * PI / partitions) - PI / partitions;

	float cid = polar.y - ltheta;
	float sid = cid - PI / partitions;
	float eid = cid + PI / partitions;

	float sRa = r.x + (r.y - r.x) * step(0., fmod(sid * .5 * partitions / PI, 1.99999)-.5);
	float eRa = r.x + (r.y - r.x) * step(0., fmod(eid * .5 * partitions / PI, 1.99999) - .5);
	
	float2 rand = float2(hash11(sid), hash11(eid));
	float2 variationOut = variation * (float2(sin(_Time.y*18. + 600. * (rand.x+seed)), sin(_Time.y*18. + 600. * (rand.y+seed)))*.5+.5);

	float2 radii = float2(sRa, eRa) + variationOut;// float2(variation * hash11(sid), variation * hash11(eid));
	return sdPolarLine(radii, 2. * PI / partitions, polar.x * float2(sin(ltheta), cos(ltheta)));
}

//Higher level stuff 
float2 pixelate(float a, float2 uv, float multiplier)
{
	return snap(uv + a, .04 * multiplier) - a+eps.xxx; //eps for de-alignment cuz length(0) is so bad
}
float2 pixelate(float2 uv)
{
	return pixelate(2., uv, 1.0);
}