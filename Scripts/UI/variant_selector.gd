extends HBoxContainer

signal pressed

func _ready() -> void:
	$Button.pressed.connect(func():
		pressed.emit()
	)


func set_labels(title : String, description : String):
	$Button.text = title
	$RichTextLabel.text = description
