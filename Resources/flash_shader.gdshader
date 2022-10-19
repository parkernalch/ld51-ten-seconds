shader_type canvas_item;
render_mode unshaded;

uniform float flash_amount: hint_range(0.0, 1.0) = 0.5;

void fragment()
{
	COLOR = texture(TEXTURE, UV);
	if (COLOR.a > 0.0)
	{
		COLOR = mix(COLOR, vec4(1), flash_amount);
	}
}