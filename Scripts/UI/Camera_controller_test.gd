extends Camera2D


var SPEED = 1000

@export var Multiverse_draw := Node2D

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	Multiverse_draw.camera_position = position
	if Input.is_key_pressed(KEY_W):
		position.y -= SPEED * delta
	if Input.is_key_pressed(KEY_S):
		position.y += SPEED * delta
	if Input.is_key_pressed(KEY_A):
		position.x -= SPEED * delta
	if Input.is_key_pressed(KEY_D):
		position.x += SPEED * delta
	

func _input(event):
	if event is InputEventMouseButton:
		if event.is_pressed():
			# zoom in
			if event.button_index == MOUSE_BUTTON_WHEEL_UP:
				zoom += Vector2(.1,.1)
				# call the zoom function
			# zoom out
			if event.button_index == MOUSE_BUTTON_WHEEL_DOWN:
				zoom -= Vector2(.1,.1)
				# call the zoom function
