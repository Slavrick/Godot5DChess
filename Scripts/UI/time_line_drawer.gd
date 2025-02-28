extends Control

enum DRAWMODE {
	FULL,
	WHITE,
	BLACK
}

@export var layer := 0
@export var board_margin := 20
@export var draw_mode := DRAWMODE.FULL
# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	place_children()


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass


func place_children():
	match draw_mode:
		DRAWMODE.WHITE:
			return
		DRAWMODE.BLACK:
			return
		_:
			pass
	for child in get_children():
		var ply = child.multiverse_position.y
		if child.color:
			ply *= 2
		else:
			ply *= 2
			ply += 1
		print_debug(ply)
		child.position = Vector2(ply,0) * Vector2(child.functional_width() + board_margin,child.functional_height() + board_margin)


func _draw():
	var width = 0
	for child in get_children():
		width += child.functional_width()
	
	draw_rect(Rect2(0,0,width,20),Color.PURPLE,true)
	
