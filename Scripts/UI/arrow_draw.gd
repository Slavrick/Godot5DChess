extends Path2D

@export var origin : Coord5
@export var dest : Coord5
@export var arrow_color : Color = Color(.1,.9,.1,.5)

func _ready() -> void:
	curve = Curve2D.new()
	get_coordinate()
	VisualSettings.game_changed.connect(get_coordinate)

func _draw():
	print_debug(origin.color)
	draw_polyline(curve.get_baked_points(), arrow_color, 30.0)


func set_arrow():
	pass


func get_coordinate():
	if origin == null or dest == null:
		return
	curve.clear_points()
	curve.add_point(VisualSettings.position_of_coordinate(origin))
	curve.add_point(VisualSettings.position_of_coordinate(dest))
	queue_redraw()
