shader_type canvas_item;
uniform sampler2D screen_texture : hint_screen_texture, repeat_disable, filter_nearest;
uniform float pixel_size : hint_range(1, 8, 0.5) = 3;
uniform float horizontal_pixel_resolution : hint_range(0, 6, 0.25) = 1.0;
uniform float curvature : hint_range(-3,3, 0.05) = 1.0;
uniform float color_resolution : hint_range(1,16,1.0) = 1.0;
uniform float brightness : hint_range(0,5,0.05) = 0;
uniform float space = 1.33;
uniform float _offset = 2.21;
uniform bool three_channels = true;

float gray(vec3 vector) {
	return (vector.r+vector.g+vector.b)/3.0;
}

vec3 pixelize(vec2 uv, vec2 pixelSize) {
	vec3 color = vec3(0);
	uv *= pixelSize;
	uv = round(uv);
	uv /= pixelSize;
	
	color = texture(screen_texture, uv).rgb;
	
	//const int numDirs = 10;
	//vec2 dirs[numDirs] = {
		//vec2(0, 0.5),
		//vec2(0, 1),
		//vec2(0.25, 0.5),
		//vec2(0.5,0),
		//vec2(0.5,0.5),
		//vec2(0.5,1),
		//vec2(0.75, 0.5),
		//vec2(1,0),
		//vec2(1,0.5),
		//vec2(1,1)
	//};
	//for (int i = 0; i < numDirs; i++) {
		//if (gray(texture(screen_texture, uv - dirs[i]/pixelSize).rgb) < 0.9) continue;
		//color = max(color, texture(screen_texture, uv - dirs[i]/pixelSize).rgb);
		////if (gray(texture(screen_texture, uv - dirs[i]/pixelSize).rgb) > gray(color))
		////max means that red bleeds
		////color += texture(screen_texture, uv - dirs[i]/pixelSize).rgb;
	//}
	////color /= float(numDirs);
	
	return color;
}
vec2 curveRemap(vec2 uv, vec2 curve) {
	curve = vec2(pow(2.0, curve.x));
    uv = uv * 2.0 - 1.0;
    vec2 offset = abs(uv.yx) / curve;
    uv = uv + uv * offset * offset;
    uv = uv * 0.5 + 0.5;
    return uv;
}

float chunk(float value, float intervals) {
	value *= intervals;
	value = round(value);
	value /= intervals ;
	return value;
}

vec3 chunk3(vec3 color, float color_res) {
	//vec3 color_bands = floor(color * color_res) / (color_res - 1.0);
	//return color_bands;
	return vec3(chunk(color.r, color_res), chunk(color.g, color_res), chunk(color.b, color_res));
}

void fragment() {
	vec3 color = vec3(1);
	vec2 uv_warp = curveRemap(SCREEN_UV, vec2(curvature * SCREEN_PIXEL_SIZE.x/SCREEN_PIXEL_SIZE.y, curvature));
	color = pixelize(uv_warp, 1.0/SCREEN_PIXEL_SIZE/pow(2.0, pixel_size)).rgb;

	float shadows = gray(color);
	color /= shadows;//remove shadows
	color = clamp(color, vec3(0.0), vec3(1.0));
	color = chunk3(color, color_resolution);
	color *= shadows;
	
	color += shadows*brightness;
	color = clamp(color, vec3(0.0), vec3(1.0));
	
	float xpixelSize = 1.0/SCREEN_PIXEL_SIZE.x/pow(2.0, pixel_size);
	
	if (three_channels) {
		float channels[] = {color.r, color.g, color.b};
		for (int i = 0; i < 3; i++) {
				//sin based channels
			float scanMask = (cos((uv_warp - vec2((float(i)/3.0 + _offset)/xpixelSize, 0.0)) * TAU * horizontal_pixel_resolution * xpixelSize)*3.0 - 2.0).x;
			scanMask *= smoothstep(0.0, 0.15, fract(uv_warp.y * 1.0/SCREEN_PIXEL_SIZE.y/pow(2.0, pixel_size) + 0.5));
			scanMask *= smoothstep(1.0, 0.85, fract(uv_warp.y * 1.0/SCREEN_PIXEL_SIZE.y/pow(2.0, pixel_size) + 0.5));
			//scanMask *= mix(0.05, 1.0, channels[i]);
			scanMask *= channels[i];
			scanMask = clamp(scanMask, 0.0, 1.0);
			channels[i] = scanMask;
		}
		color = vec3(channels[0], channels[1], channels[2]);
	} else {
		float scanMask = abs(sin(uv_warp * PI * horizontal_pixel_resolution * xpixelSize)).x;
		scanMask *= abs(sin(uv_warp * PI * 1.0/SCREEN_PIXEL_SIZE/pow(2.0, pixel_size))).y - (1.0 - shadows);
		scanMask = max(scanMask, 0.0);
		color /= shadows;
		color *= scanMask;
	}

	float boundsMask = step(0.0, 1.0 - uv_warp.y);//bottom
	boundsMask *= step(0, uv_warp.y);//top
	boundsMask *= step(0.0, 1.0 - uv_warp.x);//right
	boundsMask *= step(0, uv_warp.x);//left
	color *= boundsMask;

	COLOR = vec4(color, 1.0);
	//COLOR = vec4(vec3(0.75), 1.0);
}
//shader_type canvas_item;
//
//uniform vec2 size = vec2(32.0, 28.0);
//// If you intend to read from mipmaps with `textureLod()` LOD values greater than `0.0`,
//// use `filter_nearest_mipmap` instead. This shader doesn't require it.
//uniform sampler2D screen_texture : hint_screen_texture, repeat_disable, filter_nearest;
//
//void fragment() {
        //vec2 norm_size = size * SCREEN_PIXEL_SIZE;
        //bool half = mod(SCREEN_UV.y / 2.0, norm_size.y) / norm_size.y < 0.5;
        //vec2 uv = SCREEN_UV + vec2(norm_size.x * 0.5 * float(half), 0.0);
        //vec2 center_uv = floor(uv / norm_size) * norm_size;
        //vec2 norm_uv = mod(uv, norm_size) / norm_size;
        //center_uv += mix(vec2(0.0, 0.0),
                         //mix(mix(vec2(norm_size.x, -norm_size.y),
                                 //vec2(0.0, -norm_size.y),
                                 //float(norm_uv.x < 0.5)),
                             //mix(vec2(0.0, -norm_size.y),
                                 //vec2(-norm_size.x, -norm_size.y),
                                 //float(norm_uv.x < 0.5)),
                             //float(half)),
                         //float(norm_uv.y < 0.3333333) * float(norm_uv.y / 0.3333333 < (abs(norm_uv.x - 0.5) * 2.0)));
//
        //COLOR = textureLod(screen_texture, center_uv, 0.0);
//}