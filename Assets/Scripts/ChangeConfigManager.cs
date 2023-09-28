using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using Newtonsoft.Json;
using SimpleFileBrowser;
using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.SceneManagement;

public class PlantsDef
{
    public string name { get; set; }
    
    [JsonProperty("3DFile")]
    [CanBeNull] public string file { get; set; }
    [CanBeNull] public string color { get; set; }
    [CanBeNull] public string figure { get; set; }

    [JsonIgnore] public TMP_Dropdown dropdownColor { get; set; }
    
    [JsonIgnore] public TMP_Dropdown dropdownFigures { get; set; }
}

public class Plants
{
    public Dictionary<string, PlantsDef> point { get; set; }
    public Dictionary<string, PlantsDef> area { get; set; }
}
public class Config
{
    public int[] Scene_size { get; set; }
    
    public int ratio_pixel_meter { get; set; }
    
    public Plants plants { get; set; }

    public Dictionary<string, string> color { get; set; }
    
    public List<string> link { get; set; }
    
}

//reconnaissance forme & multi outputs & opti 

public class ChangeConfigManager : MonoBehaviour
{
    private Config _config = new Config();
    
    [SerializeField]
    private GameObject _textNamePlant;
    
    [SerializeField]
    private GameObject _dropDown;
    
    [SerializeField]
    private GameObject _popupError;
    
    private int nb;
    
    public void ExitToMenu()
    {
        SceneManager.LoadScene("Openning");
    }

    private void Start()
    {
        FileBrowser.AskPermissions = true;
        FileBrowser.RequestPermission();
    }

    public void ClosePopup() => GameObject.Find("PopupError").SetActive(false);

    private void ActivePopup(string error)
    {
        _popupError.SetActive(true);
        GameObject.Find("TextError").GetComponent<TextMeshProUGUI>().text = error;
    }
    public void Write()
    {
        var jsonSetting = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
        };
        
        if (UpdateDropDownOptions())
        {
            ActivePopup("Impossible de sauvegarder, ne mettez pas le même duo couleur et forme à deux endroits différents");
            return;
        }
        
       
        var stringWrite = JsonConvert.SerializeObject(_config, jsonSetting);
        FileBrowserHelpers.WriteTextToFile(FileBrowser.Result[0], stringWrite);
    }

    private bool UpdateDropDownOptions()
    {
        List<string> list = new List<string>();
        foreach (var areaKey in _config.plants.area.Keys)
            list.Add(_config.plants.area[areaKey].figure+_config.plants.area[areaKey].color);
        
        foreach (var areaKey in _config.plants.point.Keys)
            list.Add(_config.plants.point[areaKey].figure+_config.plants.point[areaKey].color);


        for (int i = 0; i < list.Count; i++)
        {
            for (int j = i+1; j < list.Count; j++)
            {
                if (list[i] == list[j])
                    return true;
            }
        }
        return false;
    }
    
    private void DropDownColorValueChanged(TMP_Dropdown dropdown)
    {
        var name = dropdown.name.Split("_");
        int value = dropdown.value;
        
        var color = name[0] == "area" ?  _config.plants.area[name[1]].dropdownColor.options[value].text : _config.plants.point[name[1]].dropdownColor.options[value].text;
        if (name[0] == "area")
            _config.plants.area[name[1]].color = color;
        else
            _config.plants.point[name[1]].color = color;
    }

    private void DropDownLinkValueChanged(TMP_Dropdown dropdown)
    {
        var name = dropdown.name.Split("_");
        int value = dropdown.value;
        
        var figure = name[0] == "area" ?  _config.plants.area[name[1]].dropdownFigures.options[value].text : _config.plants.point[name[1]].dropdownFigures.options[value].text;
        if (name[0] == "area")
            _config.plants.area[name[1]].figure = figure;
        else
            _config.plants.point[name[1]].figure = figure;
    }
    
    private void InitPagePoint()
    {
        foreach (var plantName in _config.plants.point.Keys)
        {
            var panel = GameObject.Find("ContentScrollView");
            var goText = Instantiate(_textNamePlant, panel.transform, true);
            var dropDownColor = Instantiate(_dropDown, panel.transform, true).GetComponent<TMP_Dropdown>();
            var dropDownFigures = Instantiate(_dropDown, panel.transform, true).GetComponent<TMP_Dropdown>();
            
            var panelTransform = panel.transform.position;
            print(panelTransform.y);
            goText.transform.position = new Vector3(0,  panelTransform.y + 300 - nb * 75, 0);
            goText.GetComponent<TextMeshProUGUI>().text = _config.plants.point[plantName].name;
            
            dropDownColor.transform.position = new Vector3( 200,  panelTransform.y + 300 - nb * 75, 0);
            dropDownFigures.transform.position = new Vector3( 400,  panelTransform.y + 300 - nb * 75, 0);

            List<string> colorsValues = new List<string>();
            List<string> figuresValues = new List<string>();
            
            foreach (var colorKey in _config.color.Keys)
                colorsValues.Add(_config.color[colorKey]);

            foreach (var figures in _config.link)
                figuresValues.Add(figures);
            
            _config.plants.point[plantName].dropdownColor = dropDownColor;
            dropDownColor.name =  "point_" + plantName + "_color";
            dropDownColor.AddOptions(colorsValues);
            dropDownColor.onValueChanged.AddListener(delegate
            {
                DropDownColorValueChanged(dropDownColor); 
            });
            
            _config.plants.point[plantName].dropdownFigures = dropDownFigures;
            dropDownFigures.name =  "point_" + plantName + "_link";
            dropDownFigures.AddOptions(figuresValues);
            dropDownFigures.onValueChanged.AddListener(delegate
            {
                DropDownLinkValueChanged(dropDownFigures); 
            });

            _config.plants.point[plantName].figure ??= dropDownFigures.options[0].text;
            dropDownFigures.value = figuresValues.IndexOf(_config.plants.point[plantName].figure);
            
            _config.plants.point[plantName].color ??= dropDownColor.options[0].text;
            dropDownColor.value = colorsValues.IndexOf(_config.plants.point[plantName].color);
            
            nb++;
        }
    }
    
    private void InitPageArea()
    {
        foreach (var plantName in _config.plants.area.Keys)
        {
            var panel = GameObject.Find("ContentScrollView");
            var goText = Instantiate(_textNamePlant, panel.transform, true);
            var dropdownColor = Instantiate(_dropDown, panel.transform, true).GetComponent<TMP_Dropdown>();
            var dropDownFigures = Instantiate(_dropDown, panel.transform, true).GetComponent<TMP_Dropdown>();

            var panelTransform = panel.transform.position;
            goText.transform.position = new Vector3(0,  panelTransform.y + 300 - nb * 75, 0);
            goText.GetComponent<TextMeshProUGUI>().text = _config.plants.area[plantName].name;
            
            dropdownColor.transform.position = new Vector3(200,  panelTransform.y + 300 - nb * 75, 0);
            dropDownFigures.transform.position = new Vector3( 400,  panelTransform.y + 300 - nb * 75, 0);

            List<string> colorsValues = new List<string>();
            List<string> figuresValues = new List<string>();
            
            foreach (var colorKey in _config.color.Keys)
                colorsValues.Add(_config.color[colorKey]);
            
            foreach (var figures in _config.link)
                figuresValues.Add(figures);
            
            _config.plants.area[plantName].dropdownColor = dropdownColor;
            dropdownColor.name =  "area_" + plantName;
            dropdownColor.AddOptions(colorsValues);
            dropdownColor.onValueChanged.AddListener(delegate
            {
                DropDownColorValueChanged(dropdownColor); 
            });
            
            _config.plants.area[plantName].dropdownFigures = dropDownFigures;
            dropDownFigures.name =  "area_" + plantName + "_link";
            dropDownFigures.AddOptions(figuresValues);
            dropDownFigures.onValueChanged.AddListener(delegate
            {
                DropDownLinkValueChanged(dropDownFigures); 
            });

            _config.plants.area[plantName].figure ??= dropDownFigures.options[0].text;
            dropDownFigures.value = figuresValues.IndexOf(_config.plants.area[plantName].figure);
            
            _config.plants.area[plantName].color ??= dropdownColor.options[0].text;
            dropdownColor.value = colorsValues.IndexOf(_config.plants.area[plantName].color);
            
            nb++;
        }
    }

    private void DeserializeConfig(string json)
    {
       
        _config = JsonConvert.DeserializeObject<Config>(json);

        var content = GameObject.Find("ContentScrollView").transform;
        
        while (content.childCount > 0) {
            DestroyImmediate(content.GetChild(0).gameObject);
        }

        InitPagePoint();
        InitPageArea();
    }
    
    public void LoadExistingConfigFileEvent()
    {
        FileBrowser.SetFilters(true, new FileBrowser.Filter("config", ".json"));
        FileBrowser.SetDefaultFilter(".json");
        FileBrowser.SetExcludedExtensions(".lnk", ".tmp", ".zip", ".rar", ".exe");
	
        StartCoroutine(LoadExistingConfigFile());
    }
    
    IEnumerator LoadExistingConfigFile()
    {
        yield return FileBrowser.WaitForLoadDialog(FileBrowser.PickMode.FilesAndFolders, false, null, null, "Load Files and Folders", "Load configFile");
        
        if (FileBrowser.Success)
        {
            
            var json = FileBrowserHelpers.ReadTextFromFile(FileBrowser.Result[0]);
            print(json);
            DeserializeConfig(json);
        }
    }
}
