extends Camera2D


var SPEED = 1000
var MIN_ZOOM = Vector2(.01,.01)
var MAX_ZOOM = Vector2(7,7)

@export var Multiverse_draw := Node2D

var zoomtweener : Tween
var pantweener : Tween

var desired_zoom = Vector2.ONE
var desired_position := Vector2.ONE
var camera_locked := false


func _ready() -> void:
	zoom = Vector2(.01,.01)
	desired_zoom = Vector2(.5,.5)
	zoomtweener = create_tween().set_ease(Tween.EASE_OUT).set_trans(Tween.TRANS_ELASTIC)
	zoomtweener.tween_property(self,"zoom",desired_zoom,1).set_trans(Tween.TRANS_CUBIC)


func _process(delta: float) -> void:
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

func force_pan(pan_position : Vector2, duration : float) -> void:
	desired_position = pan_position
	if pantweener and pantweener.is_running():
		pantweener.kill()
	pantweener = create_tween().set_ease(Tween.EASE_IN_OUT)
	pantweener.tween_property(self,"position",desired_position,duration)


var _previousPosition: Vector2 = Vector2(0, 0);
var _moveCamera: bool = false;

func _unhandled_input(event: InputEvent):
	if event is InputEventMouseButton && event.button_index == MOUSE_BUTTON_LEFT:
		get_viewport().set_input_as_handled();
		if event.is_pressed():
			_previousPosition = event.position;
			_moveCamera = true;
		else:
			_moveCamera = false;
	elif event is InputEventMouseMotion && _moveCamera:
		get_viewport().set_input_as_handled();
		position += (_previousPosition - event.position) * (1/zoom.x);
		_previousPosition = event.position;
