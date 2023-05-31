using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;


public class TDChessManager : MonoBehaviour
{
    public static TDChessManager instance;

    [Tooltip("\nPawn,\nKnight,\nBishop,\nRook,\nQueen,\nKing")]
    public GameObject[] figuresPrefabWhite;

    [Tooltip("\nPawn,\nKnight,\nBishop,\nRook,\nQueen,\nKing")]
    public GameObject[] figuresPrefabBlack;

    public Transform boardOrigin;
    public Transform figuresParent;
    public float ceilSize = 1;
    public List<Figure> figuresWhite;
    public List<Figure> figuresBlack;

    [System.Serializable]
    public class Figure
    {
        [Tooltip("\nPawn,\nKnight,\nBishop,\nRook,\nQueen,\nKing")]
        public FigureType type;
        public FigureColor color;
        public GameObject obj;
        public Vector3Byte _pos;
        public Vector3Byte pos
        {
            get { return _pos; }
            set
            {
                _pos = value;
                obj.transform.position = GetCeilWorldPos(_pos);
            }
        }
        public Figure(FigureType _type, FigureColor _color, GameObject _obj, Vector3Byte _pos)
        {
            type = _type;
            color = _color;
            obj = _obj;
            pos = _pos;
        }
    }
    /// <summary>
    ///  Pawn, Knight, Bishop, Rook, Queen, King
    /// </summary> 
    public enum FigureType
    {
        /// <summary>
        /// Pawn
        /// </summary>
        p,
        /// <summary>
        /// Knight
        /// </summary>
        kn,
        /// <summary>
        /// Bishop
        /// </summary>
        b,
        /// <summary>
        /// Rook
        /// </summary>
        r,
        /// <summary>
        /// Queen
        /// </summary>
        q,
        /// <summary>
        /// King
        /// </summary>
        kg,
    }
    public enum FigureColor
    {
        white,
        black
    }
    static class FigureMovement
    {
        public static readonly Vector3Int[] Pawn = new Vector3Int[1]
        {
            new Vector3Int(0,1,0)
        };
        public static readonly Vector3Int[] Knight = new Vector3Int[24]
        {
            new Vector3Int(1,2,0),
            new Vector3Int(-1,2,0),
            new Vector3Int(0,2,1),
            new Vector3Int(0,2,-1),

            new Vector3Int(1,-2,0),
            new Vector3Int(-1,-2,0),
            new Vector3Int(0,-2,1),
            new Vector3Int(0,-2,-1),

            new Vector3Int(1,0,2),
            new Vector3Int(-1,0,2),
            new Vector3Int(0,1,2),
            new Vector3Int(0,-1,2),

            new Vector3Int(1,0,-2),
            new Vector3Int(-1,0,-2),
            new Vector3Int(0,1,-2),
            new Vector3Int(0,-1,-2),

            new Vector3Int(2,0,1),
            new Vector3Int(2,0,-1),
            new Vector3Int(2,1,0),
            new Vector3Int(2,-1,0),

            new Vector3Int(-2,0,1),
            new Vector3Int(-2,0,-1),
            new Vector3Int(-2,1,0),
            new Vector3Int(-2,-1,0),
        };
        public static List<Vector3Int> Bishop
        {
            get
            {
                return Diagonal(instance.ceilsCount);
            }
        }
        public static List<Vector3Int> Rook
        {
            get
            {
                return Linear(instance.ceilsCount);
            }
        }
        public static List<Vector3Int> Diagonal(int iterations)
        {
            List<Vector3Int> positions = new List<Vector3Int>();
            for (int xy = -iterations - 1; xy < iterations + 1; xy++)
            {
                Vector3Int pos = new Vector3Int(xy, xy, 0);
                // if (GetFigureCeil(new v pos))
                if (pos != Vector3Int.zero) positions.Add(pos);
            }
            for (int xy = -iterations - 1; xy < iterations + 1; xy++)
            {
                Vector3Int pos = new Vector3Int(xy, -xy, 0);
                if (pos != Vector3Int.zero) positions.Add(pos);
            }
            for (int xy = -iterations - 1; xy < iterations + 1; xy++)
            {
                Vector3Int pos = new Vector3Int(-xy, xy, 0);
                if (pos != Vector3Int.zero) positions.Add(pos);
            }

            for (int xyz = -iterations - 1; xyz < iterations + 1; xyz++)
            {
                Vector3Int pos = new Vector3Int(xyz, xyz, xyz);
                if (pos != Vector3Int.zero) positions.Add(pos);
            }
            for (int xyz = -iterations - 1; xyz < iterations + 1; xyz++)
            {
                Vector3Int pos = new Vector3Int(-xyz, xyz, xyz);
                if (pos != Vector3Int.zero) positions.Add(pos);
            }
            for (int xyz = -iterations - 1; xyz < iterations + 1; xyz++)
            {
                Vector3Int pos = new Vector3Int(xyz, -xyz, xyz);
                if (pos != Vector3Int.zero) positions.Add(pos);
            }
            for (int xyz = -iterations - 1; xyz < iterations + 1; xyz++)
            {
                Vector3Int pos = new Vector3Int(xyz, xyz, -xyz);
                if (pos != Vector3Int.zero) positions.Add(pos);
            }

            for (int xz = -iterations - 1; xz < iterations + 1; xz++)
            {
                Vector3Int pos = new Vector3Int(xz, 0, xz);
                if (pos != Vector3Int.zero) positions.Add(pos);
            }
            for (int xz = -iterations - 1; xz < iterations + 1; xz++)
            {
                Vector3Int pos = new Vector3Int(-xz, 0, xz);
                if (pos != Vector3Int.zero) positions.Add(pos);
            }
            for (int xz = -iterations - 1; xz < iterations + 1; xz++)
            {
                Vector3Int pos = new Vector3Int(xz, 0, -xz);
                if (pos != Vector3Int.zero) positions.Add(pos);
            }

            for (int yz = -iterations - 1; yz < iterations + 1; yz++)
            {
                Vector3Int pos = new Vector3Int(0, yz, yz);
                if (pos != Vector3Int.zero) positions.Add(pos);
            }
            for (int yz = -iterations - 1; yz < iterations + 1; yz++)
            {
                Vector3Int pos = new Vector3Int(0, -yz, yz);
                if (pos != Vector3Int.zero) positions.Add(pos);
            }
            for (int yz = -iterations - 1; yz < iterations + 1; yz++)
            {
                Vector3Int pos = new Vector3Int(0, yz, -yz);
                if (pos != Vector3Int.zero) positions.Add(pos);
            }
            return positions;
        }
        public static List<Vector3Int> Linear(int iterations)
        {
            List<Vector3Int> positions = new List<Vector3Int>();
            for (int x = -instance.ceilsCount; x < instance.ceilsCount; x++)
            {
                Vector3Int pos = new Vector3Int(x, 0, 0);
                if (pos != Vector3Int.zero)
                    positions.Add(pos);
            }
            for (int y = -instance.ceilsCount; y < instance.ceilsCount; y++)
            {
                Vector3Int pos = new Vector3Int(0, y, 0);
                if (pos != Vector3Int.zero)
                    positions.Add(pos);
            }
            for (int z = -instance.ceilsCount; z < instance.ceilsCount; z++)
            {
                Vector3Int pos = new Vector3Int(0, 0, z);
                if (pos != Vector3Int.zero)
                    positions.Add(pos);
            }
            return positions;
        }
        public static List<Vector3Int> Queen
        {
            get
            {
                List<Vector3Int> positions = new List<Vector3Int>();
                positions.AddRange(Diagonal(instance.ceilsCount));
                positions.AddRange(Linear(instance.ceilsCount));
                return positions;
            }
        }
        public static List<Vector3Int> King
        {
            get
            {
                List<Vector3Int> positions = new List<Vector3Int>();
                positions.AddRange(Diagonal(1));
                positions.AddRange(Linear(1));
                return positions;
            }
        }
    }

    [Range(4, 8)] public int ceilsCount = 8;

    List<Board> boards = new List<Board>();
    public class Board
    {
        public string name;
        public Figure[,] ceil;
        public Board(string _name, int _cellsCount)
        {
            name = _name;
            ceil = new Figure[_cellsCount, _cellsCount];
        }
    }
    public Figure CreateFigure(FigureType type, FigureColor color, Vector3Byte pos)
    {
        GameObject obj = Instantiate(color == FigureColor.white ? figuresPrefabWhite[(int)type] : figuresPrefabBlack[(int)type], GetCeilWorldPos(pos), Quaternion.identity, figuresParent);
        if (color == FigureColor.black)
            obj.transform.localRotation = Quaternion.Euler(0, 0, 180);
        return new Figure(type, color, obj, pos);
    }
    public GameObject moveCeilPrefab;
    public List<GameObject> moveCeils = new List<GameObject>();
    public void CreateMoveCeils(Figure _figure)
    {
        for (int c = 0; c < moveCeils.Count; c++) Destroy(moveCeils[c]);
        moveCeils.Clear();
        if (_figure == null) return;
        List<Vector3Int> positions = new List<Vector3Int>();
        switch (_figure.type)
        {
            case FigureType.p:
                positions.AddRange(FigureMovement.Pawn.ToList());
                if (_figure.color == FigureColor.black) positions[0] = -positions[0];
                Vector3Int pos = new Vector3Int( _figure.pos.x + 1,  _figure.pos.y + 1, _figure.pos.z + 1);
                Vector3Int pos1 = new Vector3Int( _figure.pos.x + -1,  _figure.pos.y + 1, _figure.pos.z + 1);
                Vector3Int pos2 = new Vector3Int( _figure.pos.x + 1,  _figure.pos.y + 1, _figure.pos.z + -1);
                Vector3Int pos3 = new Vector3Int( _figure.pos.x + -1,  _figure.pos.y + 1, _figure.pos.z + -1);
                Figure figure = GetFigureCeil(new Vector3Byte((byte)pos.x, (byte)pos.y, (byte)pos.z));
                Figure figure1 = GetFigureCeil(new Vector3Byte((byte)pos1.x, (byte)pos1.y, (byte)pos1.z));
                Figure figure2 = GetFigureCeil(new Vector3Byte((byte)pos2.x, (byte)pos2.y, (byte)pos2.z));
                Figure figure3 = GetFigureCeil(new Vector3Byte((byte)pos3.x, (byte)pos3.y, (byte)pos3.z));
                if (figure != null || figure.color != _figure.color) positions.Add(pos);
                if (figure1 != null || figure1.color != _figure.color) positions.Add(pos1);
                if (figure2 != null || figure2.color != _figure.color) positions.Add(pos2);
                if (figure3 != null || figure3.color != _figure.color) positions.Add(pos3);
                break;
            case FigureType.kn: positions.AddRange(FigureMovement.Knight.ToList()); break;
            case FigureType.b: positions.AddRange(FigureMovement.Bishop); break;
            case FigureType.r: positions.AddRange(FigureMovement.Rook); break;
            case FigureType.q: positions.AddRange(FigureMovement.Queen); break;
            case FigureType.kg: positions.AddRange(FigureMovement.King); break;
        }
        for (int p = 0; p < positions.Count; p++)
        {
            Vector3Byte pos = new Vector3Byte((byte)(_figure.pos.x + positions[p].x), (byte)(_figure.pos.y + positions[p].y), (byte)(_figure.pos.z + positions[p].z));
            if (!IsPosInBoard(pos)) continue;
            GameObject moveCeil = Instantiate(moveCeilPrefab, GetCeilWorldPos(pos), Quaternion.identity, boardOrigin);
            moveCeil.name = "move ceil " + _figure.type.ToString();
            moveCeils.Add(moveCeil);
        }
    }
    public Color[] ceilColors;
    public Vector3Byte SelectCeil(Vector3 pos, GameObject ceilObject)
    {
        Vector3Byte ceilPos = GetCeilPos(pos);
        for (int c = 0; c < moveCeils.Count; c++) moveCeils[c].GetComponent<Renderer>().material.color = ceilColors[0];
        ceilObject.GetComponent<Renderer>().material.color = ceilColors[1];
        return ceilPos;
    }
    public void MoveFigure(Vector3Byte originPos, Vector3Byte newPos)
    {
        if (!IsPosInBoard(newPos)) return;
        if (boards[newPos.y].ceil[newPos.x, newPos.z] != null)
        {
            DeleteFigure(newPos);
        }
        boards[newPos.y].ceil[newPos.x, newPos.z] = boards[originPos.y].ceil[originPos.x, originPos.z];
        boards[newPos.y].ceil[newPos.x, newPos.z].pos = newPos;
        if (boards[newPos.y].ceil[newPos.x, newPos.z].type == FigureType.p)
        {
            if (boards[newPos.y].ceil[newPos.x, newPos.z].color == FigureColor.white && newPos.y == 7)
            {
                DeleteFigure(newPos);
                SetFigureCeil(CreateFigure(FigureType.q, FigureColor.white, newPos));
            }
            if (boards[newPos.y].ceil[newPos.x, newPos.z].color == FigureColor.black && newPos.y == 0)
            {
                DeleteFigure(newPos);
                SetFigureCeil(CreateFigure(FigureType.q, FigureColor.white, newPos));
            }
        }
        boards[originPos.y].ceil[originPos.x, originPos.z] = null;
    }
    public static Vector3 GetCeilWorldPos(Vector3Byte pos)
    {
        return new Vector3(pos.x, pos.y, pos.z) * instance.ceilSize + instance.boardOrigin.position;
    }
    public static Vector3Byte GetCeilPos(Vector3 pos)
    {
        pos = pos / instance.ceilSize;
        pos -= instance.boardOrigin.position;
        return new Vector3Byte((byte)Mathf.RoundToInt(pos.x), (byte)Mathf.RoundToInt(pos.y), (byte)Mathf.RoundToInt(pos.z));
    }
    public void SetFigureCeil(Figure _figure)
    {
        if (!IsPosInBoard(_figure.pos)) return;
        boards[_figure.pos.y].ceil[_figure.pos.x, _figure.pos.z] = _figure;
    }
    public void DeleteFigure(Vector3Byte pos)
    {
        if (boards[pos.y].ceil[pos.x, pos.z] != null)
        {
            Destroy(boards[pos.y].ceil[pos.x, pos.z].obj);
            boards[pos.y].ceil[pos.x, pos.z] = null;
        }
    }
    public static Figure GetFigureCeil(Vector3Byte _pos)
    {
        return instance.boards[_pos.y].ceil[_pos.x, _pos.z];
    }
    static bool IsPosInBoard(Vector3Byte _pos)
    {
        if (_pos.x >= instance.ceilsCount || _pos.y >= instance.ceilsCount || _pos.z >= instance.ceilsCount ||
            _pos.x < 0 || _pos.y < 0 || _pos.z < 0) return false;
        return true;
    }
    static bool IsPosInBoard(Vector3Int _pos)
    {
        if (_pos.x >= instance.ceilsCount || _pos.y >= instance.ceilsCount || _pos.z >= instance.ceilsCount ||
            _pos.x < 0 || _pos.y < 0 || _pos.z < 0) return false;
        return true;
    }
    public ChessGameType chessGameType = ChessGameType.diagonal;
    public enum ChessGameType
    {
        classic,
        fullClassic,
        diagonal,
    }
    private void Start()
    {
        instance = this;
        for (int b = 0; b < ceilsCount; b++)
        {
            string _name = "α";
            switch (b)
            {
                case 1: _name = "β"; break;
                case 2: _name = "γ"; break;
                case 3: _name = "δ"; break;
                case 4: _name = "ε"; break;
                case 5: _name = "ζ"; break;
                case 6: _name = "η"; break;
                case 7: _name = "θ"; break;
            }
            boards.Add(new Board(_name, instance.ceilsCount));
        }
        switch ((int)chessGameType)
        {
            case (int)ChessGameType.diagonal:
                CreateDiagonalBoard();
                break;
            default:
                CreateDiagonalBoard();
                break;
        }
    }
    void CreateDiagonalBoard()
    {
        //Pawn
        for (int z = 0; z < 8; z++)
        {
            SetFigureCeil(CreateFigure(FigureType.p, FigureColor.white, new Vector3Byte(1, 1, (byte)z)));
            SetFigureCeil(CreateFigure(FigureType.p, FigureColor.black, new Vector3Byte(6, 6, (byte)z)));
        }
        //Knight
        SetFigureCeil(CreateFigure(FigureType.kn, FigureColor.white, new Vector3Byte(0, 0, 1)));
        SetFigureCeil(CreateFigure(FigureType.kn, FigureColor.black, new Vector3Byte(7, 7, 1)));

        SetFigureCeil(CreateFigure(FigureType.kn, FigureColor.white, new Vector3Byte(0, 0, 6)));
        SetFigureCeil(CreateFigure(FigureType.kn, FigureColor.black, new Vector3Byte(7, 7, 6)));
        //Bishop
        SetFigureCeil(CreateFigure(FigureType.b, FigureColor.white, new Vector3Byte(0, 0, 2)));
        SetFigureCeil(CreateFigure(FigureType.b, FigureColor.black, new Vector3Byte(7, 7, 2)));

        SetFigureCeil(CreateFigure(FigureType.b, FigureColor.white, new Vector3Byte(0, 0, 5)));
        SetFigureCeil(CreateFigure(FigureType.b, FigureColor.black, new Vector3Byte(7, 7, 5)));
        //Rook
        SetFigureCeil(CreateFigure(FigureType.r, FigureColor.white, new Vector3Byte(0, 0, 0)));
        SetFigureCeil(CreateFigure(FigureType.r, FigureColor.black, new Vector3Byte(7, 7, 0)));

        SetFigureCeil(CreateFigure(FigureType.r, FigureColor.white, new Vector3Byte(0, 0, 7)));
        SetFigureCeil(CreateFigure(FigureType.r, FigureColor.black, new Vector3Byte(7, 7, 7)));
        //Queen
        SetFigureCeil(CreateFigure(FigureType.q, FigureColor.white, new Vector3Byte(0, 0, 3)));
        SetFigureCeil(CreateFigure(FigureType.q, FigureColor.black, new Vector3Byte(7, 7, 4)));
        //King
        SetFigureCeil(CreateFigure(FigureType.kg, FigureColor.white, new Vector3Byte(0, 0, 4)));
        SetFigureCeil(CreateFigure(FigureType.kg, FigureColor.black, new Vector3Byte(7, 7, 3)));
    }
}