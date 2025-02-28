extends Control

@export var width = 100
@export var height = 100

@onready var tile = load("res://Scenes/UI/multiverse_tile.tscn")

var tile_pool := []

# Called when the node enters the scene tree for the first time.
func _ready():
	for x in range(10):
		for y in range(10):
			var new_tile = tile.instantiate()
			new_tile.position = Vector2(x * width, y * height)
			new_tile.size = Vector2(width,height)
			new_tile.time = x
			new_tile.layer = y
			new_tile.color = (x + y) % 2 == 1
			print((x + y) % 2 == 1)
			add_child(new_tile)

func draw_region(start : Vector2, end : Vector2):
	if start.x > end.x or start.y > end.y :
		return
	
	pass

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass
