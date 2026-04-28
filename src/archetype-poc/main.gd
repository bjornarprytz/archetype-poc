extends Node2D

@onready var archetype: ArchetypeNode = $ArchetypeNode



# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	ArchetypeInterop.register(archetype)
	ArchetypeInterop.action_requested.connect(_on_action_request)
	ArchetypeInterop.game_error.connect(_on_error)
	ArchetypeInterop.start()
	var cards = ArchetypeInterop.get_atoms(ArchetypeAtomKinds.CARD)
	
	for card in cards:
		print(card)
	

func _on_error(message: String):
	print("Error: %s" % message)

func _on_action_request(action: String, available: Dictionary) -> void:
	print("Action requested: %s" % action)

	print("Available actions:")
	for key in available.keys():
		print(" - %s" % key)
