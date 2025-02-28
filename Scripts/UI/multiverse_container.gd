extends Control


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	place_timelines()


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	pass


func place_timelines():
	for child in get_children():
		child.position.y = child.layer * 1500
	
