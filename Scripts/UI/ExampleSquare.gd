extends Node2D

@export_group("Dimensions")
@export var SQUARE_WIDTH := 20
@export var board_height := 8
@export var board_width := 8
@export var margin := 20
@export_group("Colors")
@export var light_color := Color.BEIGE
@export var dark_color := Color.BLACK
@export var black_style_box : StyleBoxFlat
@export var white_style_box : StyleBoxFlat

var packed_piece = load("res://Scenes/UI/piece.tscn")

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	VisualSettings.visual_theme_changed.connect(color_changed)


func color_changed( _change : String):
	light_color = VisualSettings.light_square_color
	dark_color = VisualSettings.dark_square_color
	queue_redraw()

func _draw() -> void:
	for y in range(8):
		for x in range(8):
			var rect = Rect2(margin + x * SQUARE_WIDTH, margin + y * SQUARE_WIDTH,SQUARE_WIDTH,SQUARE_WIDTH)
			if (x + y) % 2 == 0:
				draw_rect(rect,light_color,true)
			else:
				draw_rect(rect,dark_color,true)
