extends Path2D

@export var origin : Coord5
@export var dest : Coord5
@export var arrow_color : Color = Color(.1,.9,.1,.6)
@export var rotation_curve : Curve

func _ready() -> void:
	curve = Curve2D.new()
	get_coordinate()
	VisualSettings.game_changed.connect(get_coordinate)
	VisualSettings.view_changed.connect(on_view_changed)
	

func _draw():
	draw_polyline(curve.get_baked_points(), arrow_color, 30.0,true)
	draw_circle(curve.get_point_position(1),15,arrow_color,true)

func set_arrow():
	pass


func get_coordinate():
	if origin == null or dest == null:
		return
	curve.clear_points()
	curve.add_point(VisualSettings.position_of_coordinate(origin))
	curve.add_point(VisualSettings.position_of_coordinate(dest))
	calc_inout()
	queue_redraw()


func calc_inout():
	var pos1 = curve.get_point_position(0)
	var pos2 = curve.get_point_position(1)
	#var d = min((pos2 - pos1).length(),10000.0)
	#d = d / 10000.0
	#var rot = rotation_curve.sample(d)
	#print_debug(rot)
	var out = (pos2 - pos1) / 2
	curve.set_point_out(0,out.rotated(.3))


func on_view_changed(perspective,view):
	get_coordinate()


func equals(arrow : Node) -> bool:
	return origin.Equals(arrow.origin) and dest.Equals(arrow.dest)
