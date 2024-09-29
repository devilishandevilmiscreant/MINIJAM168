shader_type canvas_item;
uniform int text[16];//really just a 4x4 2d array

int chunk(float value, int intervals) {
	value *= float(intervals);
	value = floor(value);
	//value /= intervals;
	return int(value);
}

void fragment() {
	vec2 position = vec2(float(chunk(UV.x, 4)), float(chunk(UV.y, 4)));
	//[0][0] = 0, [0][1] = 4 = 0 + 1*4, [3][3] = 15 = 3 + 3*4
	int index = int(position.x) + int(position.y)*4;
	COLOR = vec4(vec3(1), COLOR.a * float(text[index]));
}