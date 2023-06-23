# ChangeConfigManager

Gabriel Masson
***

# Documentation

```plantuml
@startuml
class PlantsDef {
    + name : string <<get>> <<set>>
    + file : string <<get>> <<set>>
    + color : string <<get>> <<set>>
    + figure : string <<get>> <<set>>
}
class Plants {
}
class Config {
    + ratio_pixel_meter : int <<get>> <<set>>
}
class ChangeConfigManager {
    + ExitToMenu() : void
    + ClosePopup() : void
    + CloseMeta() : void
    + Write() : void
    + AddPlant() : void
    + LoadExistingConfigFileEvent() : void
    LoadExistingConfigFile() : IEnumerator
}
class "Dictionary`2"<T1,T2> {
}
class "List`1"<T> {
}
PlantsDef --> "dropdownColor" TMP_Dropdown
PlantsDef --> "dropdownFigures" TMP_Dropdown
Plants --> "point<string,PlantsDef>" "Dictionary`2"
Plants --> "area<string,PlantsDef>" "Dictionary`2"
Config --> "plants" Plants
Config --> "color<string,string>" "Dictionary`2"
Config --> "link<string>" "List`1"
MonoBehaviour <|-- ChangeConfigManager
ChangeConfigManager o-> "_config" Config
@enduml

```

# Fonctions

### _void_ DeserializeConfig(string json)

#### Désérialise le fichier json et l'assigne en tant que nouvelle configuration

<blockquote>

_paramètres_ :\
&emsp; json : Chemin d'accès du fichier de configuration en `json`

</blockquote>

<p>&nbsp;</p>

### _void_ InitPageArea()

#### Initialise le menu pour les plantes simple

<blockquote>

A exécuter **seulement** après l'analyse du fichier `json`

</blockquote>

<p>&nbsp;</p>

### _void_ InitPagePoint()

#### Initialise le menu pour les surfaces

<blockquote>

A exécuter **seulement** après l'analyse du fichier `json`

</blockquote>

<p>&nbsp;</p>

### _bool_ UpdateDropDownOptions()

#### Permet de savoir si il existe un duo de forme et couleur assigné à deux plantes différentes

<blockquote>

_returns_ :\
&emsp; bool : `true` si il existe un duo, `false`sinon

</blockquote>

<p>&nbsp;</p>

### _void_ ChangePlantMetaDatas(string key, bool area, TextMeshProUGUI btn)

#### Permet la modification d'une plante à partir d'un panel

<blockquote>

_paramètres_ :\
&emsp; key : clef de la plante d'un dictionnaire plant ou area (`Dictionary<string, PlantsDef>`)\
&emsp; area : `true` si la plante est une surface, sinon `false`\
&emsp; btn : le GameObject text du bouton qui permet la modification des paramètres

</blockquote>

<p>&nbsp;</p>

### _void_ AddPlant()

#### Permet d'ajouter une nouvelle plante à la configuration

<p>&nbsp;</p>

### _void_ Write()

#### Permet de sauvegarder la configuration

<p>&nbsp;</p>

### _void_ CloseMeta()

#### Permet de fermer le panel de modification de plantes

<p>&nbsp;</p>

### _void_ ClosePopup()

#### Permet de fermer le panel d'erreur