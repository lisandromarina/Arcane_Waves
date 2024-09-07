using UnityEngine;
using UnityEngine.UI;

public class ResponsiveGridLayout : MonoBehaviour
{
    public GridLayoutGroup gridLayout; // Reference to the GridLayoutGroup component
    public int maxColumns = 3; // Maximum number of columns
    public float spacingRatio = 0.01f; // Spacing as a ratio of the screen width

    private RectTransform rectTransform;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        AdjustGridLayout();
    }

    private void Update()
    {
        AdjustGridLayout(); // Adjust layout on every frame (or optimize by checking screen size change)
    }

    private void AdjustGridLayout()
    {
        float screenWidth = rectTransform.rect.width;
        float screenHeight = rectTransform.rect.height;
        float cellWidth = screenWidth / maxColumns - (gridLayout.spacing.x + spacingRatio * screenWidth);
        float cellHeight = cellWidth; // Keep cells square; modify as needed

        gridLayout.cellSize = new Vector2(cellWidth, cellHeight);
        gridLayout.spacing = new Vector2(spacingRatio * screenWidth, spacingRatio * screenHeight);
    }
}
