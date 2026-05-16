shader_type Pages;
render_mode unshaded, cull_front;

uniform bool enable = false;
uniform vec4 outline_color : source_color = vec4(1.0, 0.84, 0.0, 1.0); // Gold outline
uniform float outline_thickness = 0.04;

void vertex() {
	if (enable) {
		VERTEX += NORMAL * outline_thickness;
	} else {
		VERTEX = vec3(0.0); // Collapse outline geometry completely if disabled
	}
}

void fragment() {
	if (enable) {
		ALBEDO = outline_color.rgb;
		ALPHA = outline_color.a;
	}
}