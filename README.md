# CASCADIA
Tic-Tac-Toe, but with more rules.

This is a digital client to play "Cascadia", a simple variation of Tic-Tac-Toe.
It is meant to be an entirely free project, released primarily to delight,
and secondarily as a portfolio project.

## CREDITS
Mint + THOSE WHO I WILL ATTRIBUTE WHEN I ASK THEM FOR THEIR PREFERRED ATTRIBUTION
Mutant Standard Emoji is utilized in this project. This project is not endorsed by them, and is non-commercial.

## CODING STYLE
### MONOBEHAVIOUR CLASSES
References to Objects should be done in private fields. Use `[SerializeReference]`/`[SerializeField]` as appropriate. If another class needs to reference a component like this, it should be done through a readonly public property.
`[Header("...")]` attributes should be used to split up code. The common categories are:
- "Internal Components": Private fields that reference Objects that are children of this Object
- "Exposed Properties": Public properties that expose Internal Components
- "Prefabs": Private fields that reference Objects meant for instantiation
- "Scene Managers": Private fields that reference Objects in the scenes, primarily Singleton UIs.