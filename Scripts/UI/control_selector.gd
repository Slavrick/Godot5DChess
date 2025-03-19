extends HBoxContainer


@export var input_name : String
@export var input_description : String

var input_key_code
var waiting_for_key = false

func _ready():
	$Label.text = input_description
	$Button.pressed.connect(_on_button_pressed)
	call_deferred("set_button_text")

func set_button_text():
	var bind_event = InputMap.action_get_events(input_name)
	if bind_event.size() == 0:
		$Button.text = "NO BINDING"
	else:
		var button = bind_event[0] as InputEventKey
		$Button.text = button.as_text_physical_keycode()

func _input(event):
	if event is InputEventMouseMotion:
		return
	if event is InputEventMouseButton:
		return
	if not waiting_for_key or event.pressed:
		return
	waiting_for_key = false
	InputMap.action_erase_events(input_name)
	InputMap.action_add_event(input_name,event)
	$Button.text = event.as_text_keycode()

func _on_button_pressed():
	waiting_for_key = true
	$Button.text = "Press Key To Bind"
