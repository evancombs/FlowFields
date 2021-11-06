using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateFlowField : MonoBehaviour
{
    public int resolution = 10;
    public float stepLength = 1;
    public int numSteps = 10;

    public int width;
    public int height;

    private int xMin, xMax, yMin, yMax;

    public int flowRes = 10;

    public int numFlows = 10;

    private int numColumns;
    private int numRows;
    // Start is called before the first frame update
    void Start()
    {
        // Creat the render target object
        GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
        quad.transform.localScale += new Vector3(9, 9, 0);

        // The boundaries of the flow field should extend farther than the rendered texture
        xMin = (int) (width * -0.5);
        xMax = (int) (width * 1.5);
        yMin = (int) (height * -0.5);
        yMin = (int) (height * 1.5);

        // flowRes = (int) (width * 0.01);
        int res = (int)(width * (1f / flowRes));

        numColumns = (2 * width) / res;
        numRows = (2 * height) / res;

        // grid is a 2D array of angles
        float[,] grid = GenerateGrid(numColumns, numRows);

        // float stepLength = 1;
        // int numSteps = 3;


        // End texture to write to
        Texture2D tex = generateTexture(grid);

        Renderer renderer = quad.GetComponent<Renderer>();
        renderer.material.mainTexture = tex;
    }

    Texture2D generateTexture(float[,] grid)
    {
        Texture2D tex = new Texture2D(width, height);

        // Set all pixels to white
        for (int i = 0; i <= width; i++)
            for (int j = 0; j <= width; j++)
                tex.SetPixel(i, j, Color.white);

        tex.Apply();

        // (float x, float y) point = (width / 2, height / 2);
        Vector2[] points = new Vector2[numFlows];

        for (int i = 0; i < numFlows; i++)
            points[i] = new Vector2(Random.Range(0, width), Random.Range(0, height));

        for (int i = 0; i < numFlows; i++)
        {
            Vector2 point = points[i];

            // Generate one color for this "flow"
            Color flowColor = RandomColor();
            int size = Random.Range(1, 5);
            //flowColor = new Color(255, 255, 0);

            for (int j = 0; j <= numSteps; j++)
            {
                // Check if in bounds
                if (point.x >= numColumns || point.x < 0
                    || point.y >= numRows || point.y < 0)
                    break;
                for (int p = size * -1; p < size; p++)
                    for (int q = size * -1; q < size; q++)
                        tex.SetPixel((int)point.x + p, (int)point.y + q, flowColor);

                // grid + angle = grangle lol
                float grangle = grid[(int)point.x, (int)point.y];

                // Lo siento, this is rough
                // This advances the point by taking a "step" (of stepLength) in the direction of the angle as determined by the grid.
                float xChange = stepLength * Mathf.Cos(grangle);
                float yChange = stepLength * Mathf.Sin(grangle);

                // Advance the point
                point = new Vector2((point.x + xChange), (point.y + yChange));
            }
        }

        tex.Apply();
        return tex;
    }

    float[,] GenerateGrid(int numColumns, int numRows)
    {
        float[,] grid = new float[numColumns, numRows];

        float default_angle = Mathf.PI * 0.25f;
        float angle;

        // Generates a slowly turning field
        for (int column = 0; column < numColumns; column++)
        {
            for (int row = 0; row < numRows; row++)
            {
                angle = (row / (float)numRows) * Mathf.PI;
                // grid[column,row] = angle;
                 grid[column, row] = Random.Range(0f, 2 * Mathf.PI);
                //grid[column, row] = Mathf.Cos((row / num_rows) * (Mathf.PI / 2));
                //grid[column, row] = angle;
            }
        }
        return grid;
    }


    Color RandomColor()
    {
        // return new Color(Random.Range(200, 255), Random.Range(200, 255), Random.Range(200, 255), 1);
        // return new Color(Random.Range(0, 255), Random.Range(0, 255), Random.Range(0, 255), 1);
        return Random.ColorHSV(0f,.1f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
