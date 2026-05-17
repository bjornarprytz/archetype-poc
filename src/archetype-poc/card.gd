class_name Card
extends Control

var id: int
var atom: CardAtom

@onready var header_text: RichTextLabel = %HeaderText
@onready var rules_text: RichTextLabel = %RulesText

func _ready() -> void:
	assert(atom != null, "Card atom required")
	var header = atom.get_definition_name()
	header_text.text = header
	
	var rules = RenderNode.parse(atom.get_rules_tree())
	
	rules_text.text = rules.flat_text()
