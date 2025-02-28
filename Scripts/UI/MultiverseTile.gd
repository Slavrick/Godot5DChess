extends Panel

@export var time := 0
@export var layer := 0
@export var color := true

var base_text = "%dT%dL"


func _ready():
	$MultiverseLabel.text = base_text % [time,layer]
	add_theme_stylebox_override("panel", get_theme().get_stylebox("panelblack","Panel"))
	if color:
		add_theme_stylebox_override("panel", get_theme().get_stylebox("panelwhite","Panel"))


func _process(delta):
	pass
