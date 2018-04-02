using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Group : MonoBehaviour
{
    float LastFall = 0;

    bool IsValidGridPos()
    {
        foreach (Transform child in transform)
        {
            Vector2 v = Grid.RoundVec2(child.position);

            //Within Borders?
            if (!Grid.InsideBorder(v))
                return false;

            //Block in grid cell (and not part of the same group)?
            if (Grid.grid[(int)v.x, (int)v.y] != null &&
                Grid.grid[(int)v.x, (int)v.y].parent != transform)
                return false;
        }
        return true;
    }

    void UpdateGrid()
    {
        //Remove old children from grid
        for (int y = 0; y < Grid.h; y++)
            for (int x = 0; x < Grid.w; x++)
                if (Grid.grid[x, y] != null)
                    if (Grid.grid[x, y].parent == transform)
                        Grid.grid[x, y] = null;

        //Add new children to grid
        foreach (Transform child in transform)
        {
            Vector2 v = Grid.RoundVec2(child.position);
            Grid.grid[(int)v.x, (int)v.y] = child;
        }

    }

    // Use this for initialization
    void Start()
    {
        if (!IsValidGridPos())
        {
            Debug.Log("GAME OVER");
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Move Left
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            //Modify position
            transform.position += new Vector3(-1, 0, 0);

            //Check if valid
            if (IsValidGridPos())
                //Check clear, update
                UpdateGrid();

            else
                //It's invalid, revert
                transform.position += new Vector3(1, 0, 0);
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            //Modify position
            transform.position += new Vector3(1, 0, 0);

            //Check if valid
            if (IsValidGridPos())
                //Check clear, update
                UpdateGrid();

            else
                //It's invalid, revert
                transform.position += new Vector3(-1, 0, 0);
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            //Modify position
            transform.Rotate(0, 0, -90);

            //Check if valid
            if (IsValidGridPos())
                //Check clear, update
                UpdateGrid();

            else
                //It's invalid, revert
                transform.Rotate(0, 0, 90);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow) || (Time.time - LastFall) >= 1)
        {
            //Modify position
            transform.position += new Vector3(0, -1, 0);

            //Check if valid
            if (IsValidGridPos())
            {
                //Check clear, update
                UpdateGrid();
            }
            else
            {
                //It's invalid, revert
                transform.position += new Vector3(0, 1, 0);

                //Delete rows
                Grid.DeleteFullRows();

                //Spawn Next
                FindObjectOfType<Spawner>().SpawnNext();

                //Disable script
                enabled = false;
            }

            LastFall = Time.time;
        }
    }
}
