using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TableController : MonoBehaviour
{
    public Text headerPrefab;           // used to update the header row
    public GameObject rowPrefab;        // used to store cells
    public Text cellPrefab;             // used to insert more data

    private DBManager    manager;
    private ControlPanel controlPanel;
    public RectTransform fullContent;   // outer content rect
    public RectTransform dataSection;   // cells ScrollRect controller
    public RectTransform dataContent;   // cells rect
    public RectTransform headerRow;     // row which stores headers

    public bool debugMode = false;
    public string tableName;
    public string creationCommand;

    private RectTransform table;
    private float tableWidthMax = 400;
    private float tableHeightMax = 600;
    private float borderWidth = 20;
    private int rowCount = 0;

    // Use this for initialization
    void Start()
    {
        GameObject managerObject = GameObject.FindWithTag("Manager");
        if (managerObject != null)
        {
            manager = managerObject.GetComponent<DBManager>();
        }
        if (manager == null)
        {
            Debug.Log("Cannot find 'Manager' script");
        }
        GameObject controlPanelObject = GameObject.FindWithTag("ControlPanel");
        if (controlPanelObject != null)
        {
            controlPanel = controlPanelObject.GetComponent<ControlPanel>();
        }
        if (controlPanel == null)
        {
            Debug.Log("Cannot find 'ControlPanel' script");
        }
        table = gameObject.GetComponent<RectTransform>();
    }

    /// <summary>
    /// Destroy current header and remake with new fields
    /// </summary>
    public void SetupHeader(List<string> fields)
    {
        ClearHeader();
        foreach (string name in fields)
        {
            Text header = Instantiate(headerPrefab, headerRow);
            header.text = name;
        }
    }

    /// <summary>
    /// Add a single row to the table
    /// </summary>
    public void AddRow(List<string> data)
    {
        GameObject row = Instantiate(rowPrefab, dataContent);
        foreach (string info in data)
        {
            Text cell = Instantiate(cellPrefab, row.transform);
            cell.text = info;
        }
        rowCount++;

        // Increase the height of dataContent
        //dataContent.sizeDelta = new Vector2(dataContent.sizeDelta.x, 
        //    dataContent.sizeDelta.y + row.GetComponent<RectTransform>().sizeDelta.y);
    }

    /// <summary>
    /// Clear the header of fields
    /// </summary>
    public void ClearHeader()
    {
        for (int i = headerRow.childCount - 1; i >= 0; i--)
        {
            Destroy(headerRow.GetChild(i).gameObject);
        }
    }

    /// <summary>
    /// Clear the table of data
    /// </summary>
    public void ClearData()
    {
        for (int i = dataContent.childCount; i >= 0; i--)
        {
            Destroy(dataContent.GetChild(i).gameObject);
        }
        rowCount = 0;
    }
    /// <summary>
    /// Initialise the size of windows
    /// </summary>
    public void InitialiseWindow()
    {
        if (table == null)
        {
            table = gameObject.GetComponent<RectTransform>();
        }
        float headerWidth = 0;
        for (int i = headerRow.childCount - 1; i >= 0; i--)
        {
            headerWidth += headerRow.GetChild(i).GetComponent<RectTransform>().sizeDelta.x;
        }
        float dataHeight = 0;
        for (int i = dataContent.childCount - 1; i >= 0; i--)
        {
            dataHeight += dataContent.GetChild(i).GetComponent<RectTransform>().sizeDelta.y;
        }

        // table should be as wide as the fields require, as tall as data require
        headerRow.sizeDelta = new Vector2(headerWidth, headerRow.sizeDelta.y);
        dataContent.sizeDelta = new Vector2(headerWidth, dataHeight);

        table.sizeDelta = new Vector2(
            Mathf.Min(table.sizeDelta.x, headerWidth + borderWidth),
            Mathf.Min(table.sizeDelta.y, dataContent.sizeDelta.y + headerRow.sizeDelta.y + borderWidth)
        );
        fullContent.sizeDelta = new Vector2(headerWidth, table.sizeDelta.y - borderWidth);
        dataSection.sizeDelta = new Vector2(headerWidth, fullContent.sizeDelta.y - headerRow.sizeDelta.y);
        // data content rect needs to adjust height based on amount of data
        if (debugMode)
        {
            Debug.Log("data window size = " + dataContent.sizeDelta);
        }
    }

    /// <summary>
    /// Update the size of windows
    /// </summary>
    public void UpdateWindow()
    {
        if (table == null)
        {
            table = gameObject.GetComponent<RectTransform>();
        }
        float headerWidth = 0;
        for (int i = headerRow.childCount - 1; i >= 0; i--)
        {
            headerWidth += headerRow.GetChild(i).GetComponent<RectTransform>().sizeDelta.x;
        }
        // table should be as wide as the fields require, as tall as data require
        headerRow.sizeDelta = new Vector2(headerWidth, headerRow.sizeDelta.y);
        dataContent.sizeDelta  = new Vector2(
            dataContent.GetComponent<VerticalLayoutGroup>().preferredWidth, 
            dataContent.GetComponent<VerticalLayoutGroup>().preferredHeight
        );
        
        table.sizeDelta = new Vector2(
            Mathf.Min(table.sizeDelta.x, headerWidth + borderWidth), 
            Mathf.Min(table.sizeDelta.y, dataContent.sizeDelta.y + headerRow.sizeDelta.y + borderWidth)
        );
        fullContent.sizeDelta = new Vector2(headerWidth, table.sizeDelta.y - borderWidth);
        dataSection.sizeDelta = new Vector2(headerWidth, fullContent.sizeDelta.y - headerRow.sizeDelta.y);
        // data content rect needs to adjust height based on amount of data
        if (debugMode)
        {
            Debug.Log("data window size = " + dataContent.sizeDelta);
        }
    }

    public void OnDrag(UnityEngine.EventSystems.BaseEventData eventData)
    {
        var pointerData = eventData as UnityEngine.EventSystems.PointerEventData;
        if (pointerData == null)
            return;
        
        Vector3 currentPosition = table.position;
        currentPosition.x += pointerData.delta.x;
        currentPosition.y += pointerData.delta.y;
        //Debug.Log("press position = " + pointerData.pressPosition);
        //Debug.Log("table position = " + table.position);
        //Debug.Log("pointer delta = " + pointerData.delta);
        //Debug.Log("pointer position = " + pointerData.position);
        table.position = currentPosition;
    }

    // Update is called once per frame
    void Update()
    {
        //UpdateWindow();
    }

    /// <summary>
    /// Clean up all data
    /// </summary>
    void OnDestroy() { }

}
