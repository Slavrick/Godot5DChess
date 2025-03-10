extends Camera2D


var SPEED = 1000

@export var Multiverse_draw := Node2D

var zoomtweener : Tween
var desired_zoom = Vector2.ONE

var MAX_ZOOM = Vector2(7,7)
var MIN_ZOOM = Vector2(.01,.01)


func _ready() -> void:
	pass # Replace with function body.


func _process(delta: float) -> void:
	#print_debug(get_global_mouse_position())
	Multiverse_draw.camera_position = position
	if Input.is_key_pressed(KEY_W):
		position.y -= SPEED * delta * (1/zoom.x)
	if Input.is_key_pressed(KEY_S):
		position.y += SPEED * delta * (1/zoom.x)
	if Input.is_key_pressed(KEY_A):
		position.x -= SPEED * delta * (1/zoom.x)
	if Input.is_key_pressed(KEY_D):
		position.x += SPEED * delta * (1/zoom.x)
	

func _input(event):
	if event is InputEventMouseButton:
		if event.is_pressed():
			if event.button_index == MOUSE_BUTTON_WHEEL_UP:
				desired_zoom *= Vector2(1.5,1.5)
				if desired_zoom.x > MAX_ZOOM.x:
					desired_zoom = MAX_ZOOM
				if zoomtweener and zoomtweener.is_running():
					zoomtweener.kill()
				zoomtweener = create_tween().set_ease(Tween.EASE_OUT).set_trans(Tween.TRANS_ELASTIC)
				zoomtweener.tween_property(self,"zoom",desired_zoom,.5).set_trans(Tween.TRANS_CUBIC)
			if event.button_index == MOUSE_BUTTON_WHEEL_DOWN:
				desired_zoom *= Vector2(.66,.66)
				if desired_zoom.x < MIN_ZOOM.x:
					desired_zoom = MIN_ZOOM
				if zoomtweener and zoomtweener.is_running():
					zoomtweener.kill()
				zoomtweener = create_tween().set_ease(Tween.EASE_OUT).set_trans(Tween.TRANS_ELASTIC)
				zoomtweener.tween_property(self,"zoom",desired_zoom,.5).set_trans(Tween.TRANS_CUBIC)
