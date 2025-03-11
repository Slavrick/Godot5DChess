extends TextureButton


signal piece_right_clicked

@export var piece_type := 0

const PIECE_SIZE = Vector2(128,128)

const piecedict = {
	0: Vector2(0,0), # Empty
	1: Vector2(128,0), #WPawn
	2: Vector2(256,0), #WKnight
	3: Vector2(384,0), 
	4: Vector2(512,0),
	5: Vector2(640,0),
	6: Vector2(768,0),
	7: Vector2(896,0),
	8: Vector2(1024,0),
	9: Vector2(1152,0),
	10: Vector2(1280,0),
	11: Vector2(0,128), #WBrawn
	12: Vector2(128,128),
	13: Vector2(256,128),
	14: Vector2(384,128),
	15: Vector2(512,128),
	16: Vector2(640,128),
	17: Vector2(768,128),
	18: Vector2(896,128),
	19: Vector2(1024,128),
	20: Vector2(1152,128),
	21: Vector2(1280,128),
	22: Vector2(0,256), #BUnicorn
	23: Vector2(128,256),
	24: Vector2(256,256),
	25: Vector2(384,256),
	26: Vector2(512,256),
}

var rank : int
var file : int


func _ready() -> void:
	texture_normal = texture_normal.duplicate()
	if piece_type < 0:
		piece_type = 0
	texture_normal.region = Rect2(piecedict[piece_type],PIECE_SIZE)


func _on_mouse_entered() -> void:
	modulate = Color.AQUA


func _on_mouse_exited() -> void:
	modulate = Color.WHITE


func _on_gui_input(event: InputEvent) -> void:
	if event is InputEventMouseButton and event.button_index == MOUSE_BUTTON_RIGHT and event.pressed:
		piece_right_clicked.emit()
