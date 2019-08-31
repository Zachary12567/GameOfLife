using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridEnumsNS;
using GridNS;

public class GridController : MonoBehaviour {

    public delegate void NotifyPassDone();
    public static event NotifyPassDone OnPassDone;

    public const int MAXDIM = 100;
    public const int MINDIM = 2;
    public int len, hei;
    public bool isPaused;
    public float speed;
    
    private void OnEnable() {
        UIController.OnGridDimInit += Init;
        UIController.OnBegin += PauseToggle;
        UIController.OnBegin += RunSimulation;
        UIController.OnPause += PauseToggle;
        UIController.OnNext += DoPass;
        UIController.OnClear += ClearGrid;
        UIController.OnSpeedChange += UpdateSpeed;
    }
    private void OnDisable() {
        UIController.OnGridDimInit -= Init;
        UIController.OnBegin -= PauseToggle;
        UIController.OnBegin -= RunSimulation;
        UIController.OnPause -= PauseToggle;
        UIController.OnNext -= DoPass;
        UIController.OnClear -= ClearGrid;
        UIController.OnSpeedChange -= UpdateSpeed;
    }

    private void Awake () {
        isPaused = true;
        speed = 0.5f;
    }
    private void Init(int len, int hei) {
        this.len = len;
        this.hei = hei;
    }
    private void PauseToggle() {
        if (isPaused) {
            isPaused = false;
        } else {
            isPaused = true;
        }
    }
    private void ClearGrid() {
        foreach (GridSq tile in GridSetup.gridObj.grid) {
            if (tile.state) {
                tile.FlipState();
            }
        }
    }
    private void RunSimulation() {
        StartCoroutine(CoRunSimulation());
    }
    private IEnumerator CoRunSimulation() {
        while (!isPaused) {
            DoPass();
            yield return new WaitForSeconds(speed);
        }
    }
    private void DoPass() {
        bool[,] gridBool = FillGridBool();

        ApplyChanges(gridBool);

        if (OnPassDone != null) {
            OnPassDone();
        }
    }
    private bool[,] FillGridBool() {
        bool[,] tmpGridBool = new bool[len, hei];

        for (int i = 0; i < len; i++) {
            for (int j = 0; j < hei; j++) {
                int amtAliveNei = AddValidNeighbors(GridSetup.gridObj.grid[i, j]);

                if (GridSetup.gridObj.grid[i,j].state) {
                    if (amtAliveNei == 2 || amtAliveNei == 3) {
                        tmpGridBool[i, j] = true;
                    } else {
                        tmpGridBool[i, j] = false;
                    }
                } else {
                    if (amtAliveNei == 3)
                        tmpGridBool[i, j] = true;
                }
            }
        }
        return tmpGridBool;
    }
    private int AddValidNeighbors(GridSq tile) {
        int amtAliveNei = 0;
        TileType tileType = tile.TileType;

        if (tileType == TileType.normal) {
            if (GrabNeighbor(tile, Location.above).state) {
                amtAliveNei++;
            }
            if (GrabNeighbor(tile, Location.left).state) {
                amtAliveNei++;
            }
            if (GrabNeighbor(tile, Location.right).state) {
                amtAliveNei++;
            }
            if (GrabNeighbor(tile, Location.below).state) {
                amtAliveNei++;
            }
            if (GrabNeighbor(tile, Location.tlCorner).state) {
                amtAliveNei++;
            }
            if (GrabNeighbor(tile, Location.trCorner).state) {
                amtAliveNei++;
            }
            if (GrabNeighbor(tile, Location.blCorner).state) {
                amtAliveNei++;
            }
            if (GrabNeighbor(tile, Location.brCorner).state) {
                amtAliveNei++;
            }
        } else if (tileType == TileType.leftSide) {
            if (GrabNeighbor(tile, Location.above).state) {
                amtAliveNei++;
            }
            if (GrabNeighbor(tile, Location.right).state) {
                amtAliveNei++;
            }
            if (GrabNeighbor(tile, Location.below).state) {
                amtAliveNei++;
            }
            if (GrabNeighbor(tile, Location.trCorner).state) {
                amtAliveNei++;
            }
            if (GrabNeighbor(tile, Location.brCorner).state) {
                amtAliveNei++;
            }
        } else if (tileType == TileType.blCorner) {
            if (GrabNeighbor(tile, Location.above).state) {
                amtAliveNei++;
            }
            if (GrabNeighbor(tile, Location.right).state) {
                amtAliveNei++;
            }
            if (GrabNeighbor(tile, Location.trCorner).state) {
                amtAliveNei++;
            }
        } else if (tileType == TileType.tlCorner) {
            if (GrabNeighbor(tile, Location.right).state) {
                amtAliveNei++;
            }
            if (GrabNeighbor(tile, Location.below).state) {
                amtAliveNei++;
            }
            if (GrabNeighbor(tile, Location.brCorner).state) {
                amtAliveNei++;
            }
        } else if (tileType == TileType.topSide) {
            if (GrabNeighbor(tile, Location.left).state) {
                amtAliveNei++;
            }
            if (GrabNeighbor(tile, Location.right).state) {
                amtAliveNei++;
            }
            if (GrabNeighbor(tile, Location.below).state) {
                amtAliveNei++;
            }
            if (GrabNeighbor(tile, Location.brCorner).state) {
                amtAliveNei++;
            }
            if (GrabNeighbor(tile, Location.brCorner).state) {
                amtAliveNei++;
            }
        } else if (tileType == TileType.bottomSide) {
            if (GrabNeighbor(tile, Location.above).state) {
                amtAliveNei++;
            }
            if (GrabNeighbor(tile, Location.left).state) {
                amtAliveNei++;
            }
            if (GrabNeighbor(tile, Location.right).state) {
                amtAliveNei++;
            }
            if (GrabNeighbor(tile, Location.tlCorner).state) {
                amtAliveNei++;
            }
            if (GrabNeighbor(tile, Location.trCorner).state) {
                amtAliveNei++;
            }
        } else if (tileType == TileType.rightSide) {
            if (GrabNeighbor(tile, Location.above).state) {
                amtAliveNei++;
            }
            if (GrabNeighbor(tile, Location.left).state) {
                amtAliveNei++;
            }
            if (GrabNeighbor(tile, Location.below).state) {
                amtAliveNei++;
            }
            if (GrabNeighbor(tile, Location.tlCorner).state) {
                amtAliveNei++;
            }
            if (GrabNeighbor(tile, Location.blCorner).state) {
                amtAliveNei++;
            }
        } else if (tileType == TileType.trCorner) {
            if (GrabNeighbor(tile, Location.left).state) {
                amtAliveNei++;
            }
            if (GrabNeighbor(tile, Location.below).state) {
                amtAliveNei++;
            }
            if (GrabNeighbor(tile, Location.blCorner).state) {
                amtAliveNei++;
            }
        } else if (tileType == TileType.brCorner) {
            if (GrabNeighbor(tile, Location.above).state) {
                amtAliveNei++;
            }
            if (GrabNeighbor(tile, Location.left).state) {
                amtAliveNei++;
            }
            if (GrabNeighbor(tile, Location.tlCorner).state) {
                amtAliveNei++;
            }
        }
        return amtAliveNei;
    }
    private GridSq GrabNeighbor(GridSq currentTile, Location location) {
        GridSq neighbor = null;

        int i = currentTile.x;
        int j = currentTile.y;

        if (location == Location.above) {
            if (GridSetup.gridObj.grid[i, j - 1] != null)
                neighbor = GridSetup.gridObj.grid[i, j - 1];
        } else if (location == Location.left) {
            if (GridSetup.gridObj.grid[i - 1, j] != null)
                neighbor = GridSetup.gridObj.grid[i - 1, j];
        } else if (location == Location.right) {
            if (GridSetup.gridObj.grid[i + 1, j] != null)
                neighbor = GridSetup.gridObj.grid[i + 1, j];
        } else if (location == Location.below) {
            if (GridSetup.gridObj.grid[i, j + 1] != null)
                neighbor = GridSetup.gridObj.grid[i, j + 1];
        } else if (location == Location.tlCorner) {
            if (GridSetup.gridObj.grid[i - 1, j - 1] != null)
                neighbor = GridSetup.gridObj.grid[i-1, j-1];
        } else if (location == Location.trCorner) {
            if (GridSetup.gridObj.grid[i + 1, j - 1] != null)
                neighbor = GridSetup.gridObj.grid[i+1, j-1];
        } else if (location == Location.blCorner) {
            if (GridSetup.gridObj.grid[i - 1, j + 1] != null)
                neighbor = GridSetup.gridObj.grid[i-1, j+1];
        } else if (location == Location.brCorner) {
            if (GridSetup.gridObj.grid[i + 1, j + 1] != null)
                neighbor = GridSetup.gridObj.grid[i+1, j+1];
        } else {
            throw new System.ArgumentException("The location specified does not exist.");
        }
        return neighbor;
    }
    private void ApplyChanges(bool[,] gridBool) {
        for (int i = 0; i < len; i++) {
            for (int j = 0; j < hei; j++) {
                if (gridBool[i, j] != GridSetup.gridObj.grid[i, j].state) {
                    GridSetup.gridObj.grid[i, j].FlipState();
                }
            }
        }
    }
    private void UpdateSpeed(float speed) {
        this.speed = speed;
    }
}
