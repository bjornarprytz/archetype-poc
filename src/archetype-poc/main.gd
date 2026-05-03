extends Node2D

@onready var card_factory: PackedScene = preload("res://card.tscn")
@onready var hand_factory: PackedScene = preload("res://hand.tscn")

@onready var archetype: ArchetypeNode = $ArchetypeNode
@onready var stuff: VBoxContainer = %Stuff

var _hands : Array[Hand] = []

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	ArchetypeInterop.register(archetype)
	ArchetypeInterop.action_requested.connect(_on_action_request)
	ArchetypeInterop.game_error.connect(_on_error)
	ArchetypeInterop.start()
	
	for zone_atom in ArchetypeInterop.get_all_zones():
		var hand = create_hand(zone_atom)
		stuff.add_child(hand)
		
		for card_atom in zone_atom.get_cards():
			var card = create_card(card_atom)
			hand.add_card(card)
	

func _on_error(message: String):
	print("Error: %s" % message)

func _on_action_request(action: String, available: Dictionary) -> void:
	print("Action requested: %s" % action)

	print("Available actions:")
	for key in available.keys():
		print(" - %s" % key)

func create_hand(zone_atom: ZoneAtom) -> Hand:
	var hand = hand_factory.instantiate() as Hand
	hand.atom = zone_atom
	hand.id = zone_atom.get_atom_id()
	
	return hand
	
func create_card(card_atom: CardAtom) -> Card:
	var c = card_factory.instantiate() as Card
	c.atom = card_atom
	c.id = card_atom.get_atom_id()
	
	return c
