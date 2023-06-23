# LayerInformation

Gabriel Masson

---

# Documentation

```plantuml
@startuml
class LayerInfoClass {
    + LayerInfoClass(layer_:GameObject, color:Color)
}
class LayersInformation {
    + ShowLayerBtn() : void
    ChangePosition(culture:GameObject, destination:float, scale:float, setActive:bool) : IEnumerator
}
class Position {
    + x : int <<get>> <<set>>
    + y : int <<get>> <<set>>
    + rank : int <<get>> <<set>>
}
LayerInfoClass --> "position" Position
LayerInfoClass --> "parent" GameObject
LayerInfoClass --> "layer" GameObject
MonoBehaviour <|-- LayersInformation
LayerInfoClass +-- Position
@enduml

```

# Fonctions

### _IEnumerator_ ChangePosition(GameObject culture, float destination, float scale, bool setActive)

#### Permet de changer la position d'un layer

<blockquote>

_paramètres_ :\
&emsp; culture : Le gameObject d'un layer\
&emsp; destination : position en y d'un layer\
&emsp; scale : la taille cible du layer\
&emsp; setActive : `true` si le layer doit apparaître, `false` sinon

</blockquote>

<p>&nbsp;</p>