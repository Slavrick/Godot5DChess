extends Control

signal close


func _ready() -> void:
	$VBoxContainer/whitesquare/ColorPicker.color_changed.connect(change_light_color)
	$VBoxContainer/blacksquare/ColorPicker.color_changed.connect(change_dark_color)
	$VBoxContainer/whitemultiversesquare/ColorPicker.color_changed.connect(white_multiverse_color_change)
	$VBoxContainer/blackmultiversesquare/ColorPicker.color_changed.connect(black_multiverse_color_change)
	
	
	$VBoxContainer/whitesquare/ColorPicker.color = VisualSettings.light_square_color
	$VBoxContainer/blacksquare/ColorPicker.color = VisualSettings.dark_square_color
	$VBoxContainer/whitemultiversesquare/ColorPicker.color = VisualSettings.white_multiverse_color
	$VBoxContainer/blackmultiversesquare/ColorPicker.color = VisualSettings.black_multiverse_color
	
	$VBoxContainer/return.pressed.connect(returntomain)

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass


func change_light_color( color : Color):
	VisualSettings.light_square_color = color

func change_dark_color( color : Color ):
	VisualSettings.dark_square_color = color


func white_multiverse_color_change( color : Color ):
	VisualSettings.white_multiverse_color = color

func black_multiverse_color_change( color : Color ):
	VisualSettings.black_multiverse_color = color


func returntomain():
	close.emit()
