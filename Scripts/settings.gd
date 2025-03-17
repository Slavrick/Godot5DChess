extends Control

signal close


func _ready() -> void:
	$VBoxContainer/whitesquare/ColorPicker.color_changed.connect(change_light_color)
	$VBoxContainer/blacksquare/ColorPicker.color_changed.connect(change_dark_color)
	$VBoxContainer/whitemultiversesquare/ColorPicker.color_changed.connect(white_multiverse_color_change)
	$VBoxContainer/blackmultiversesquare/ColorPicker.color_changed.connect(black_multiverse_color_change)
	
	VisualSettings.load_user_settings()
	set_colorpickers()
	
	$VBoxContainer/return.pressed.connect(returntomain)
	$VBoxContainer/Pallets/DefaultPalette.pressed.connect(set_pallet.bind(VisualSettings.default_palette))
	$VBoxContainer/Pallets/GrayScalePalette.pressed.connect(set_pallet.bind(VisualSettings.grayscale_palette))
	$VBoxContainer/Pallets/GlassPalette.pressed.connect(set_pallet.bind(VisualSettings.glass_palette))
	$VBoxContainer/Pallets/WoodPalette.pressed.connect(set_pallet.bind(VisualSettings.wood_palette))
	$VBoxContainer/Pallets/CheckersPalette.pressed.connect(set_pallet.bind(VisualSettings.checkers_palette))
	
	
	$VBoxContainer/TLVertMargin/SpinBox.value_changed.connect(TLMarginChanged)
	$VBoxContainer/BoardPadding/SpinBox.value_changed.connect(board_padding_changed)
	$VBoxContainer/BoardMargin/SpinBox.value_changed.connect(board_margin_changed)


func change_light_color( color : Color):
	VisualSettings.light_square_color = color


func change_dark_color( color : Color ):
	VisualSettings.dark_square_color = color


func white_multiverse_color_change( color : Color ):
	VisualSettings.white_multiverse_color = color


func black_multiverse_color_change( color : Color ):
	VisualSettings.black_multiverse_color = color


func set_colorpickers():
	$VBoxContainer/whitesquare/ColorPicker.color = VisualSettings.light_square_color
	$VBoxContainer/blacksquare/ColorPicker.color = VisualSettings.dark_square_color
	$VBoxContainer/whitemultiversesquare/ColorPicker.color = VisualSettings.white_multiverse_color
	$VBoxContainer/blackmultiversesquare/ColorPicker.color = VisualSettings.black_multiverse_color



func returntomain():
	close.emit()
	VisualSettings.save_user_settings()


func set_pallet(pallet_dicitionary : Dictionary):
	VisualSettings.load_dictionary(pallet_dicitionary)
	set_colorpickers()


func TLMarginChanged(value : float):
	VisualSettings.timeline_vertical_margin = value


func board_margin_changed(value: float):
	VisualSettings.board_horizontal_margin = value


func board_padding_changed(value:float):
	VisualSettings.board_padding = value
