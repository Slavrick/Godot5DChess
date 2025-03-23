extends Camera2D


@export var game : Control

const SPEED = 1000
const MIN_ZOOM = Vector2(.01,.01)
const MAX_ZOOM = Vector2(7,7)

var zoomtweener : Tween
var pantweener : Tween

var desired_zoom = Vector2.ONE
var desired_position := Vector2.ONE
var camera_locked := false

var _previousPosition: Vector2 = Vector2(0, 0)
var _moveCamera: bool = false


func _ready() -> void:
	zoom = Vector2(.01,.01)
	desired_zoom = Vector2(.3,.3)
	zoomtweener = create_tween().set_ease(Tween.EASE_OUT)
	zoomtweener.tween_property(self,"zoom",desired_zoom,1).set_ease(Tween.EASE_OUT).set_trans(Tween.TRANS_CUBIC)
	VisualSettings.view_changed.connect(on_view_changed)
	if(game != null):
		game.MoveMade.connect(on_move_made)
		game.GameLoaded.connect(_on_menus_goto_present)
	


func _process(delta: float) -> void:
	if camera_locked:
		return
	if Input.is_action_pressed("CameraUp"):
		position.y -= SPEED * delta * (1/zoom.x)
	if Input.is_action_pressed("CameraDown"):
		position.y += SPEED * delta * (1/zoom.x)
	if Input.is_action_pressed("CameraLeft"):
		position.x -= SPEED * delta * (1/zoom.x)
	if Input.is_action_pressed("CameraRight"):
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
	camera_locked = true
	desired_position = pan_position
	if pantweener and pantweener.is_running():
		pantweener.kill()
	pantweener = create_tween().set_ease(Tween.EASE_IN_OUT)
	pantweener.tween_property(self,"position",desired_position,duration).set_ease(Tween.EASE_OUT).set_trans(Tween.TRANS_QUAD)
	await pantweener.finished
	camera_locked = false


func force_pan_to_tile(tile : Vector2,color : bool, duration : float):
	var tile_position = VisualSettings.position_of_multiverse_tile(tile)
	var pan_position = Vector2(tile_position)
	force_pan(pan_position,duration)


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


func get_multiverse_position():
	var vec = Vector2(position.x / VisualSettings.multiverse_tile_width , position.y/VisualSettings.multiverse_tile_height)
	vec.x = ceil(vec.x)
	vec.y = floor(vec.y)
	print_debug(vec)


#TODO doesn't work as well for black only or white only view
func on_move_made(tile : Vector2, color : bool):
	if VisualSettings.perspective:
		tile.x += .5
	else:
		tile.x -= .5
	if color:
		tile.y += .25
	else:
		tile.y += .75
	force_pan_to_tile(tile,true,.5)


func on_view_changed( perspective : bool, multiverse_view ):
	_on_menus_goto_present()


func _on_menus_goto_present() -> void:
	if game != null:
		var present = game.GetPresentTile()
		if present == null:
			return
		if VisualSettings.perspective:#TODO find a better way to do this
			present += Vector2(.5,.5)
		else:
			present += Vector2(.5,-.5)
		present = Vector2(present.y,present.x) #translate from L,T to game coords
		force_pan_to_tile(present,true,.5)
