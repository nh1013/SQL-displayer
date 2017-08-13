using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class ControlPanel : MonoBehaviour
{
    public DBManager manager;
    public Dropdown  databaseDropdown;
    public Dropdown  createTableDropdown;
    public Dropdown  selectTableDropdown;
    private RectTransform selectedTable;

    // Use this for initialization
    void Start()
    {
        // get the database selections
        DirectoryInfo dir = new DirectoryInfo("Databases/");
        FileInfo[] info = dir.GetFiles("*.db");
        List<string> fileNames = new List<string> { };
        foreach (FileInfo file in info)
        {
            fileNames.Add(Path.GetFileNameWithoutExtension(file.ToString()));
        }
        databaseDropdown.AddOptions(fileNames);
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Change to the database selected by the control panel
    /// </summary>
    public void ChangeDatabase()
    {
        string newDBName = databaseDropdown.captionText.text;
        if (newDBName == "Select database")
        {
            Debug.Log("No database selected");
        }
        else
        {
            manager.ChangeDatabase(newDBName);
            createTableDropdown.value = 0;
            UpdateTableDropdown();
        }
    }

    /// <summary>
    /// Create the table selected by the control panel
    /// </summary>
    private void UpdateTableDropdown()
    {
        List<string> tableOptions = manager.GetTables();
        createTableDropdown.ClearOptions();
        List<string> defaultOption = new List<string> { "Select table" };
        createTableDropdown.AddOptions(defaultOption);
        createTableDropdown.AddOptions(tableOptions);
    }

    /// <summary>
    /// Create the table selected by the control panel
    /// </summary>
    public void CreateTable()
    {
        string newTableName = createTableDropdown.captionText.text;
        if (newTableName == "Select table")
        {
            Debug.Log("No table selected");
        }
        else
        {
            RectTransform table = manager.CreateTable(newTableName);
            if (table != null)
            {
                List<string> newOption = new List<string> { table.name };
                selectTableDropdown.AddOptions(newOption);
            }
        }
    }

    /// <summary>
    /// Destroy the table selected by the control panel
    /// </summary>
    public void DestroyTable()
    {
        string tableName = selectTableDropdown.captionText.text;
        GameObject table = GameObject.Find(tableName);
        if (table != null)
        {
            int index = selectTableDropdown.value;
            var options = selectTableDropdown.options;
            options.RemoveAt(index);
            selectTableDropdown.value = 0;
            Destroy(table);
        }
        else
        {
            Debug.Log("No valid table found");
        }
    }
}
