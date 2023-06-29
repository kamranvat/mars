using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridLayout : LayoutGroup
{
    public enum FitType
    {
        Uniform, 
        Width,
        Height,
        FixedRows,
        FixedColumns
    }
    public FitType fitType;
    public int rows;
    public int columns;
    public Vector2 cellSize;
    public Vector2 spacing;
    public bool fitX;
    public bool fitY;

    public override void CalculateLayoutInputHorizontal()
    {
        base.CalculateLayoutInputHorizontal();

        if (fitType == FitType.Width ||  fitType == FitType.Height || fitType == FitType.Uniform) 
        {
            fitX = true;
            fitY = true;

            // Number of rows and columns
            float sqrRt = Mathf.Sqrt(transform.childCount);
            rows = Mathf.CeilToInt(sqrRt);
            columns = Mathf.CeilToInt(sqrRt);

        }

        if (fitType == FitType.Width || fitType == FitType.FixedColumns)
        {
            rows = Mathf.CeilToInt(transform.childCount / (float)columns);

        }
        if (fitType == FitType.Height || fitType== FitType.FixedRows)
        {
            columns = Mathf.CeilToInt(transform.childCount / (float)rows);
        }
        

        if(fitType == FitType.Width)
        {
            rows = Mathf.CeilToInt(transform.childCount / (float)columns);
        }
        if(fitType == FitType.Height)
        {
            columns = Mathf.CeilToInt(transform.childCount / (float)rows);
        }
        // Available space
        float parentWidth = rectTransform.rect.width;
        float parentHeight = rectTransform.rect.height;

        // Size of the children, reduced by spacing and padding to avoid overflow
        //float cellWidth = parentWidth / (float) columns - (spacing.x / (float) columns * 2) - (padding.left / (float) columns - (padding.right) / (float) columns);
        //float cellHeight = parentHeight / (float) rows - (spacing.y / (float) columns * 2) - (padding.top / (float) rows - (padding.bottom / (float) rows));

        float cellWidth = (parentWidth - padding.left - padding.right - spacing.x * (columns - 1)) / (float)columns;
        float cellHeight = (parentHeight - padding.top - padding.bottom - spacing.y * (rows - 1)) / (float)rows;

        cellSize.x = fitX ? cellWidth : cellSize.x;
        cellSize.y = fitY ? cellHeight : cellSize.y;

        int columnCount = 0;
        int rowCount = 0;

        for (int i = 0; i < rectChildren.Count; i++)
        {
            // Find index
            rowCount = i / columns;
            columnCount = i % columns;

            var item = rectChildren[i];

            var xPos = (cellSize.x * columnCount) + (spacing.x * columnCount) + padding.left;
            var yPos = (cellSize.y * rowCount) + (spacing.y * rowCount) + padding.top;

            SetChildAlongAxis(item, 0, xPos, cellSize.x);
            SetChildAlongAxis(item, 1, yPos, cellSize.y);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void CalculateLayoutInputVertical()
    {

    }

    public override void SetLayoutHorizontal()
    {

    }

    public override void SetLayoutVertical()
    {

    }
}
