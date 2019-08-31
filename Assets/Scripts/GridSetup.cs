using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridEnumsNS;
using GridNS;

namespace GridEnumsNS {
    public enum TileType {
        normal,
        leftSide,
        topSide,
        bottomSide,
        rightSide,
        blCorner,
        tlCorner,
        brCorner,
        trCorner
    };
    public enum Location {
        tlCorner,
        trCorner,
        blCorner,
        brCorner,
        above,
        left,
        right,
        below
    };
    public enum MaterialType {
        vacant,
        filled
    };
}

namespace GridNS {

    public class MyGrid {
        public Material vacant, filled;

        public GridSq[,] grid;

        public int gridLen;
        public int gridHei;

        public MyGrid(int len, int hei) {
            gridLen = len;
            gridHei = hei;
            grid = new GridSq[len, hei];

            vacant = Resources.Load("Material/Vacancy", typeof(Material)) as Material;
            filled = Resources.Load("Material/Filled", typeof(Material)) as Material;
        }
    }
    public class GridSq {

        public MyGrid       grid;
        public GameObject   GridSquareObj { get; set; }
        public bool         state;
        public TileType     TileType { get; set; }
        public int          x;
        public int          y;

        private MeshRenderer sqMR;

        public GridSq(MyGrid grid, GameObject gridSquareObj, bool state, TileType tileType, int x, int y) {
            this.grid = grid;
            GridSquareObj = gridSquareObj;
            this.state = state;
            TileType = tileType;
            this.x = x;
            this.y = y;

            sqMR = gridSquareObj.GetComponent<MeshRenderer>();
            if (state)
                sqMR.material = grid.filled;
            else
                sqMR.material = grid.vacant;
        }
        public void FlipState() {
            if (state) {
                state = false;
                SetMat(MaterialType.vacant);
            } else {
                state = true;
                SetMat(MaterialType.filled);
            }
        }
        private void SetMat(MaterialType mat) {
            if (mat == MaterialType.filled) {
                this.sqMR.material = grid.filled;
            } else {
                this.sqMR.material = grid.vacant;
            }
        }
    }
}

public class GridSetup : MonoBehaviour {

    public Camera cam;
    public GameObject gridParent;
    public GameObject gridSquare;

    public static MyGrid gridObj;

    private bool gridExists;

    private void OnEnable() {
        UIController.OnGridDimInit += Init;
    }
    private void OnDisable() {
        UIController.OnGridDimInit -= Init;
    }
    private void DrawGrid(int len, int hei) {
        gridObj = new MyGrid(len, hei);
        
        for (int y=0; y < gridObj.gridHei; y++) {
            for (int x=0; x < gridObj.gridLen; x++) {
                GameObject tempSq = Instantiate(gridSquare, new Vector2(x, -y), Quaternion.Euler(-90, 0, 0), gridParent.transform);
                tempSq.name = x + "," + y;

                TileType tileType = CategorizeTile(x, y, gridObj.gridLen, gridObj.gridHei);
                
                gridObj.grid[x,y] = new GridSq(gridObj, tempSq, false, tileType, x, y);
            }
        }
    }
    private void PositionCam() {
        // Center camera
        cam.transform.position = new Vector3(gridObj.gridLen >> 1, -gridObj.gridHei >> 1, -1);

        // Set cam width according to grid size
        if (gridObj.gridLen > gridObj.gridHei) {
            cam.orthographicSize = (gridObj.gridLen >> 1) + 2;
        } else {
            cam.orthographicSize = (gridObj.gridHei >> 1) + 2;
        }
    }
    private void Init(int len, int hei) {
        if (gridExists) {
            foreach (Transform child in gridParent.transform) {
                Destroy(child.gameObject);
            }
        } else {
            gridExists = true;
        }
        DrawGrid(len,hei);
        PositionCam();
    }
    private TileType CategorizeTile(int i, int j, int len, int hei) {
        TileType tt;
        if (i == 0) {
            if (j == 0) {

                tt = TileType.tlCorner;
            } else if (j + 1 == hei) {
                tt = TileType.blCorner;
            } else {
                tt = TileType.leftSide;
            }
        } else if (j == 0) {
            if (i + 1 == len) {
                tt = TileType.trCorner;
            } else {
                tt = TileType.topSide;
            }
        } else if (j + 1 == hei) {
            if (i + 1 == len) {
                tt = TileType.brCorner;
            } else {
                tt = TileType.bottomSide;
            }
        } else if (i + 1 == len) {
            tt = TileType.rightSide;
        } else {
            tt = TileType.normal;
        }
        return tt;
    }
}
