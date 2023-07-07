using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        public Vector3Int _pos;
        public bool firstMove = true;
        public Vector3Int pos
        {
            get { return _pos; }
            set
            {
                _pos = value;
                obj.transform.position = GetCeilWorldPos(_pos);
            }
        }
        public Figure(FigureType _type, FigureColor _color, GameObject _obj, Vector3Int _pos)
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
    public enum CeilStatus
    {
        empty,
        figure,
        figureUnderAttack
    }
    public class Vector3IntCeilStatus
    {
        public Vector3Int pos;
        public CeilStatus ceilStatus = CeilStatus.empty;
        public Vector3IntCeilStatus(int x, int y, int z)
        {
            pos = new Vector3Int(x, y, z);
            if (GetFigureCeil(TDChessController.instance.selectedFigure.pos + pos) != null) ceilStatus = CeilStatus.figure;
        }
        public Vector3IntCeilStatus(Vector3Int _pos)
        {
            pos = _pos;
            if (GetFigureCeil(TDChessController.instance.selectedFigure.pos + pos) != null) ceilStatus = CeilStatus.figure;
        }
        public Vector3IntCeilStatus(int x, int y, int z, bool checkFigureCanBit)
        {
            pos = new Vector3Int(x, y, z);
            Figure figureInCeil = GetFigureCeil(TDChessController.instance.selectedFigure.pos + pos);
            if (GetFigureCeil(TDChessController.instance.selectedFigure.pos + pos) != null) ceilStatus =
                    checkFigureCanBit &&
                    figureInCeil.color != TDChessController.instance.selectedFigure.color
                    ? CeilStatus.figureUnderAttack : CeilStatus.figure;
        }
        public Vector3IntCeilStatus(Vector3Int _pos, bool checkFigureCanBit)
        {
            pos = _pos;
            Figure figureInCeil = GetFigureCeil(TDChessController.instance.selectedFigure.pos + pos);
            if (GetFigureCeil(TDChessController.instance.selectedFigure.pos + pos) != null) ceilStatus =
                    checkFigureCanBit &&
                    figureInCeil.color != TDChessController.instance.selectedFigure.color
                    ? CeilStatus.figureUnderAttack : CeilStatus.figure;
        }
        public Vector3IntCeilStatus(Vector3IntCeilStatus vector3IntCeilStatus)
        {
            pos = vector3IntCeilStatus.pos;
            ceilStatus = vector3IntCeilStatus.ceilStatus;
        }
        public Vector3IntCeilStatus IsCeilHasFigure()
        {
            Figure figureInCeil = GetFigureCeil(TDChessController.instance.selectedFigure.pos + pos);
            if (GetFigureCeil(TDChessController.instance.selectedFigure.pos + pos) != null)
            {
                if (figureInCeil.color == TDChessController.instance.selectedFigure.color) return null;
                return this;
            }
            return this;
        }
    }
    static class FigureMovement
    {
        public static readonly Vector3IntCeilStatus Pawn = new Vector3IntCeilStatus(0, 1, 0);
        public static List<Vector3IntCeilStatus> Knight
        {
            get
            {
                List<Vector3IntCeilStatus> positions = new List<Vector3IntCeilStatus>();
                List<Vector3IntCeilStatus> positionsFiltered = new List<Vector3IntCeilStatus>();

                positions.Add(new Vector3IntCeilStatus(1, 2, 0, true).IsCeilHasFigure());
                positions.Add(new Vector3IntCeilStatus(-1, 2, 0, true).IsCeilHasFigure());
                positions.Add(new Vector3IntCeilStatus(0, 2, 1, true).IsCeilHasFigure());
                positions.Add(new Vector3IntCeilStatus(0, 2, -1, true).IsCeilHasFigure());

                positions.Add(new Vector3IntCeilStatus(1, -2, 0, true).IsCeilHasFigure());
                positions.Add(new Vector3IntCeilStatus(-1, -2, 0, true).IsCeilHasFigure());
                positions.Add(new Vector3IntCeilStatus(0, -2, 1, true).IsCeilHasFigure());
                positions.Add(new Vector3IntCeilStatus(0, -2, -1, true).IsCeilHasFigure());

                positions.Add(new Vector3IntCeilStatus(1, 0, 2, true).IsCeilHasFigure());
                positions.Add(new Vector3IntCeilStatus(-1, 0, 2, true).IsCeilHasFigure());
                positions.Add(new Vector3IntCeilStatus(0, 1, 2, true).IsCeilHasFigure());
                positions.Add(new Vector3IntCeilStatus(0, -1, 2, true).IsCeilHasFigure());

                positions.Add(new Vector3IntCeilStatus(1, 0, -2, true).IsCeilHasFigure());
                positions.Add(new Vector3IntCeilStatus(-1, 0, -2, true).IsCeilHasFigure());
                positions.Add(new Vector3IntCeilStatus(0, 1, -2, true).IsCeilHasFigure());
                positions.Add(new Vector3IntCeilStatus(0, -1, -2, true).IsCeilHasFigure());

                positions.Add(new Vector3IntCeilStatus(2, 0, 1, true).IsCeilHasFigure());
                positions.Add(new Vector3IntCeilStatus(2, 0, -1, true).IsCeilHasFigure());
                positions.Add(new Vector3IntCeilStatus(2, 1, 0, true).IsCeilHasFigure());
                positions.Add(new Vector3IntCeilStatus(2, -1, 0, true).IsCeilHasFigure());

                positions.Add(new Vector3IntCeilStatus(-2, 0, 1, true).IsCeilHasFigure());
                positions.Add(new Vector3IntCeilStatus(-2, 0, -1, true).IsCeilHasFigure());
                positions.Add(new Vector3IntCeilStatus(-2, 1, 0, true).IsCeilHasFigure());
                positions.Add(new Vector3IntCeilStatus(-2, -1, 0, true).IsCeilHasFigure());

                for (int p = 0; p < positions.Count; p++) if (positions[p] != null) positionsFiltered.Add(positions[p]);
                return (positionsFiltered);
            }
        }
        public static List<Vector3IntCeilStatus> Bishop
        {
            get
            {
                return Diagonal(instance.ceilsCount);
            }
        }
        public static List<Vector3IntCeilStatus> Rook
        {
            get
            {
                return Linear(instance.ceilsCount);
            }
        }
        public static List<Vector3IntCeilStatus> Diagonal(int iterations)
        {
            List<Vector3IntCeilStatus> positions = new List<Vector3IntCeilStatus>();
            bool xyMove = true;
            bool xyMoveNegative = true;
            for (int xy = 1; xy < iterations + 1; xy++)
            {
                Vector3IntCeilStatus pos = new Vector3IntCeilStatus(xy, xy, 0, true);
                Vector3IntCeilStatus posNegative = new Vector3IntCeilStatus(-xy, -xy, 0, true);
                if (pos.ceilStatus == CeilStatus.figure) xyMove = false;
                if (posNegative.ceilStatus == CeilStatus.figure) xyMoveNegative = false;
                if (xyMove) positions.Add(pos);
                if (xyMoveNegative) positions.Add(posNegative);
                if (pos.ceilStatus != CeilStatus.empty) xyMove = false;
                if (posNegative.ceilStatus != CeilStatus.empty) xyMoveNegative = false;
            }
            bool xyxmyMove = true;
            bool xyxmyMoveNegative = true;
            for (int xy = 1; xy < iterations + 1; xy++)
            {
                Vector3IntCeilStatus pos = new Vector3IntCeilStatus(xy, -xy, 0, true);
                Vector3IntCeilStatus posNegative = new Vector3IntCeilStatus(-xy, xy, 0, true);
                if (pos.ceilStatus == CeilStatus.figure) xyxmyMove = false;
                if (posNegative.ceilStatus == CeilStatus.figure) xyxmyMoveNegative = false;
                if (xyxmyMove) positions.Add(pos);
                if (xyxmyMoveNegative) positions.Add(posNegative);
                if (pos.ceilStatus != CeilStatus.empty) xyxmyMove = false;
                if (posNegative.ceilStatus != CeilStatus.empty) xyxmyMoveNegative = false;
            }
            bool mxyxyMove = true;
            bool mxyxyMoveNegative = true;
            for (int xy = 1; xy < iterations + 1; xy++)
            {
                Vector3IntCeilStatus pos = new Vector3IntCeilStatus(-xy, xy, 0, true);
                Vector3IntCeilStatus posNegative = new Vector3IntCeilStatus(xy, -xy, 0, true);
                if (pos.ceilStatus == CeilStatus.figure) mxyxyMove = false;
                if (posNegative.ceilStatus == CeilStatus.figure) mxyxyMoveNegative = false;
                if (mxyxyMove) positions.Add(pos);
                if (mxyxyMoveNegative) positions.Add(posNegative);
                if (pos.ceilStatus != CeilStatus.empty) mxyxyMove = false;
                if (posNegative.ceilStatus != CeilStatus.empty) mxyxyMoveNegative = false;
            }

            bool xyzMove = true;
            bool xyzMoveNegative = true;
            for (int xyz = 1; xyz < iterations + 1; xyz++)
            {
                Vector3IntCeilStatus pos = new Vector3IntCeilStatus(xyz, xyz, xyz, true);
                Vector3IntCeilStatus posNegative = new Vector3IntCeilStatus(-xyz, -xyz, -xyz, true);
                if (pos.ceilStatus == CeilStatus.figure) xyzMove = false;
                if (posNegative.ceilStatus == CeilStatus.figure) xyzMoveNegative = false;
                if (xyzMove) positions.Add(pos);
                if (xyzMoveNegative) positions.Add(posNegative);
                if (pos.ceilStatus != CeilStatus.empty) xyzMove = false;
                if (posNegative.ceilStatus != CeilStatus.empty) xyzMoveNegative = false;
            }
            bool mxyzxyzxyzMove = true;
            bool mxyzxyzxyzMoveNegative = true;
            for (int xyz = 1; xyz < iterations + 1; xyz++)
            {
                Vector3IntCeilStatus pos = new Vector3IntCeilStatus(-xyz, xyz, xyz, true);
                Vector3IntCeilStatus posNegative = new Vector3IntCeilStatus(xyz, -xyz, -xyz, true);
                if (pos.ceilStatus == CeilStatus.figure) mxyzxyzxyzMove = false;
                if (posNegative.ceilStatus == CeilStatus.figure) mxyzxyzxyzMoveNegative = false;
                if (mxyzxyzxyzMove) positions.Add(pos);
                if (mxyzxyzxyzMoveNegative) positions.Add(posNegative);
                if (pos.ceilStatus != CeilStatus.empty) mxyzxyzxyzMove = false;
                if (posNegative.ceilStatus != CeilStatus.empty) mxyzxyzxyzMoveNegative = false;
            }
            bool xyzmxyzxyzMove = true;
            bool xyzmxyzxyzMoveNegative = true;
            for (int xyz = 1; xyz < iterations + 1; xyz++)
            {
                Vector3IntCeilStatus pos = new Vector3IntCeilStatus(xyz, -xyz, xyz, true);
                Vector3IntCeilStatus posNegative = new Vector3IntCeilStatus(-xyz, xyz, -xyz, true);
                if (pos.ceilStatus == CeilStatus.figure) xyzmxyzxyzMove = false;
                if (posNegative.ceilStatus == CeilStatus.figure) xyzmxyzxyzMoveNegative = false;
                if (xyzmxyzxyzMove) positions.Add(pos);
                if (xyzmxyzxyzMoveNegative) positions.Add(posNegative);
                if (pos.ceilStatus != CeilStatus.empty) xyzmxyzxyzMove = false;
                if (posNegative.ceilStatus != CeilStatus.empty) xyzmxyzxyzMoveNegative = false;
            }
            bool xyzxyzmxyzMove = true;
            bool xyzxyzmxyzMoveNegative = true;
            for (int xyz = 1; xyz < iterations + 1; xyz++)
            {
                Vector3IntCeilStatus pos = new Vector3IntCeilStatus(xyz, xyz, -xyz, true);
                Vector3IntCeilStatus posNegative = new Vector3IntCeilStatus(-xyz, -xyz, xyz, true);
                if (pos.ceilStatus == CeilStatus.figure) xyzxyzmxyzMove = false;
                if (posNegative.ceilStatus == CeilStatus.figure) xyzxyzmxyzMoveNegative = false;
                if (xyzxyzmxyzMove) positions.Add(pos);
                if (xyzxyzmxyzMoveNegative) positions.Add(posNegative);
                if (pos.ceilStatus != CeilStatus.empty) xyzxyzmxyzMove = false;
                if (posNegative.ceilStatus != CeilStatus.empty) xyzxyzmxyzMoveNegative = false;
            }
            bool xzMove = true;
            bool xzMoveNegative = true;
            for (int xz = 1; xz < iterations + 1; xz++)
            {
                Vector3IntCeilStatus pos = new Vector3IntCeilStatus(xz, 0, xz, true);
                Vector3IntCeilStatus posNegative = new Vector3IntCeilStatus(-xz, 0, -xz, true);
                if (pos.ceilStatus == CeilStatus.figure) xzMove = false;
                if (posNegative.ceilStatus == CeilStatus.figure) xzMoveNegative = false;
                if (xzMove) positions.Add(pos);
                if (xzMoveNegative) positions.Add(posNegative);
                if (pos.ceilStatus != CeilStatus.empty) xzMove = false;
                if (posNegative.ceilStatus != CeilStatus.empty) xzMoveNegative = false;
            }
            bool mxzxzMove = true;
            bool mxzxzMoveNegative = true;
            for (int xz = 1; xz < iterations + 1; xz++)
            {
                Vector3IntCeilStatus pos = new Vector3IntCeilStatus(-xz, 0, xz, true);
                Vector3IntCeilStatus posNegative = new Vector3IntCeilStatus(xz, 0, -xz, true);
                if (pos.ceilStatus == CeilStatus.figure) mxzxzMove = false;
                if (posNegative.ceilStatus == CeilStatus.figure) mxzxzMoveNegative = false;
                if (mxzxzMove) positions.Add(pos);
                if (mxzxzMoveNegative) positions.Add(posNegative);
                if (pos.ceilStatus != CeilStatus.empty) mxzxzMove = false;
                if (posNegative.ceilStatus != CeilStatus.empty) mxzxzMoveNegative = false;
            }
            bool xzmxzMove = true;
            bool xzmxzMoveNegative = true;
            for (int xz = 1; xz < iterations + 1; xz++)
            {
                Vector3IntCeilStatus pos = new Vector3IntCeilStatus(xz, 0, -xz, true);
                Vector3IntCeilStatus posNegative = new Vector3IntCeilStatus(-xz, 0, xz, true);
                if (pos.ceilStatus == CeilStatus.figure) xzmxzMove = false;
                if (posNegative.ceilStatus == CeilStatus.figure) xzmxzMoveNegative = false;
                if (xzmxzMove) positions.Add(pos);
                if (xzmxzMoveNegative) positions.Add(posNegative);
                if (pos.ceilStatus != CeilStatus.empty) xzmxzMove = false;
                if (posNegative.ceilStatus != CeilStatus.empty) xzmxzMoveNegative = false;
            }
            bool yzMove = true;
            bool yzMoveNegative = true;
            for (int yz = 1; yz < iterations + 1; yz++)
            {
                Vector3IntCeilStatus pos = new Vector3IntCeilStatus(0, yz, yz, true);
                Vector3IntCeilStatus posNegative = new Vector3IntCeilStatus(0, -yz, -yz, true);
                if (pos.ceilStatus == CeilStatus.figure) yzMove = false;
                if (posNegative.ceilStatus == CeilStatus.figure) yzMoveNegative = false;
                if (yzMove) positions.Add(pos);
                if (yzMoveNegative) positions.Add(posNegative);
                if (pos.ceilStatus != CeilStatus.empty) yzMove = false;
                if (posNegative.ceilStatus != CeilStatus.empty) yzMoveNegative = false;
            }
            bool myzyzMove = true;
            bool myzyzMoveNegative = true;
            for (int yz = 1; yz < iterations + 1; yz++)
            {
                Vector3IntCeilStatus pos = new Vector3IntCeilStatus(0, -yz, yz, true);
                Vector3IntCeilStatus posNegative = new Vector3IntCeilStatus(0, yz, -yz, true);
                if (pos.ceilStatus == CeilStatus.figure) myzyzMove = false;
                if (posNegative.ceilStatus == CeilStatus.figure) myzyzMoveNegative = false;
                if (myzyzMove) positions.Add(pos);
                if (myzyzMoveNegative) positions.Add(posNegative);
                if (pos.ceilStatus != CeilStatus.empty) myzyzMove = false;
                if (posNegative.ceilStatus != CeilStatus.empty) myzyzMoveNegative = false;
            }
            bool yzmyzMove = true;
            bool yzmyzMoveNegative = true;
            for (int yz = 1; yz < iterations + 1; yz++)
            {
                Vector3IntCeilStatus pos = new Vector3IntCeilStatus(0, yz, -yz, true);
                Vector3IntCeilStatus posNegative = new Vector3IntCeilStatus(0, -yz, yz, true);
                if (pos.ceilStatus == CeilStatus.figure) yzmyzMove = false;
                if (posNegative.ceilStatus == CeilStatus.figure) yzmyzMoveNegative = false;
                if (yzmyzMove) positions.Add(pos);
                if (yzmyzMoveNegative) positions.Add(posNegative);
                if (pos.ceilStatus != CeilStatus.empty) yzmyzMove = false;
                if (posNegative.ceilStatus != CeilStatus.empty) yzmyzMoveNegative = false;
            }
            return positions;
        }

        public static List<Vector3IntCeilStatus> Linear(int iterations)
        {
            List<Vector3IntCeilStatus> positions = new List<Vector3IntCeilStatus>();
            bool xMove = true;
            bool xMoveNegative = true;
            for (int x = 1; x < iterations + 1; x++)
            {
                Vector3IntCeilStatus pos = new Vector3IntCeilStatus(x, 0, 0, true);
                Vector3IntCeilStatus posNegative = new Vector3IntCeilStatus(-x, 0, 0, true);
                if (pos.ceilStatus == CeilStatus.figure) xMove = false;
                if (posNegative.ceilStatus == CeilStatus.figure) xMoveNegative = false;
                if (xMove) positions.Add(pos);
                if (xMoveNegative) positions.Add(posNegative);
                if (pos.ceilStatus != CeilStatus.empty) xMove = false;
                if (posNegative.ceilStatus != CeilStatus.empty) xMoveNegative = false;
            }
            bool yMove = true;
            bool yMoveNegative = true;
            for (int y = 1; y < iterations + 1; y++)
            {
                Vector3IntCeilStatus pos = new Vector3IntCeilStatus(0, y, 0, true);
                Vector3IntCeilStatus posNegative = new Vector3IntCeilStatus(0, -y, 0, true);
                if (pos.ceilStatus == CeilStatus.figure) yMove = false;
                if (posNegative.ceilStatus == CeilStatus.figure) yMoveNegative = false;
                if (yMove) positions.Add(pos);
                if (yMoveNegative) positions.Add(posNegative);
                if (pos.ceilStatus != CeilStatus.empty) yMove = false;
                if (posNegative.ceilStatus != CeilStatus.empty) yMoveNegative = false;
            }
            bool zMove = true;
            bool zMoveNegative = true;
            for (int z = 1; z < iterations + 1; z++)
            {
                Vector3IntCeilStatus pos = new Vector3IntCeilStatus(0, 0, z, true);
                Vector3IntCeilStatus posNegative = new Vector3IntCeilStatus(0, 0, -z, true);
                if (pos.ceilStatus == CeilStatus.figure) zMove = false;
                if (posNegative.ceilStatus == CeilStatus.figure) zMoveNegative = false;
                if (zMove) positions.Add(pos);
                if (zMoveNegative) positions.Add(posNegative);
                if (pos.ceilStatus != CeilStatus.empty) zMove = false;
                if (posNegative.ceilStatus != CeilStatus.empty) zMoveNegative = false;
            }
            return positions;
        }

        public static List<Vector3IntCeilStatus> Queen
        {
            get
            {
                List<Vector3IntCeilStatus> positions = new List<Vector3IntCeilStatus>();
                positions.AddRange(Diagonal(instance.ceilsCount));
                positions.AddRange(Linear(instance.ceilsCount));
                return positions;
            }
        }

        public static List<Vector3IntCeilStatus> King
        {
            get
            {
                List<Vector3IntCeilStatus> positions = new List<Vector3IntCeilStatus>();
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
    public Figure CreateFigure(FigureType type, FigureColor color, Vector3Int pos)
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
        List<Vector3IntCeilStatus> positions = new List<Vector3IntCeilStatus>();
        switch (_figure.type)
        {
            case FigureType.p:
                positions.Add(FigureMovement.Pawn);
                int side = _figure.color == FigureColor.white ? 1 : -1;
                if (_figure.color == FigureColor.black) positions[0].pos *= side;
                if (_figure.firstMove) positions.Add(new Vector3IntCeilStatus(positions[0].pos + Vector3Int.up * side));

                Vector3IntCeilStatus pos = new Vector3IntCeilStatus(1, side, 1);
                Vector3IntCeilStatus pos1 = new Vector3IntCeilStatus(-1, side, 1);
                Vector3IntCeilStatus pos2 = new Vector3IntCeilStatus(1, side, -1);
                Vector3IntCeilStatus pos3 = new Vector3IntCeilStatus(-1, side, -1);

                Vector3IntCeilStatus pos4 = new Vector3IntCeilStatus(0, side, 1);
                Vector3IntCeilStatus pos5 = new Vector3IntCeilStatus(0, side, -1);
                Vector3IntCeilStatus pos6 = new Vector3IntCeilStatus(1, side, 0);
                Vector3IntCeilStatus pos7 = new Vector3IntCeilStatus(-1, side, 0);

                Figure figure = GetFigureCeil(_figure.pos + pos.pos);
                Figure figure1 = GetFigureCeil(_figure.pos + pos1.pos);
                Figure figure2 = GetFigureCeil(_figure.pos + pos2.pos);
                Figure figure3 = GetFigureCeil(_figure.pos + pos3.pos);

                Figure figure4 = GetFigureCeil(_figure.pos + pos4.pos);
                Figure figure5 = GetFigureCeil(_figure.pos + pos5.pos);
                Figure figure6 = GetFigureCeil(_figure.pos + pos6.pos);
                Figure figure7 = GetFigureCeil(_figure.pos + pos7.pos);

                if (figure != null && figure.color != _figure.color) positions.Add(pos);
                if (figure1 != null && figure1.color != _figure.color) positions.Add(pos1);
                if (figure2 != null && figure2.color != _figure.color) positions.Add(pos2);
                if (figure3 != null && figure3.color != _figure.color) positions.Add(pos3);


                if (figure4 != null && figure4.color != _figure.color) positions.Add(pos4);
                if (figure5 != null && figure5.color != _figure.color) positions.Add(pos5);
                if (figure6 != null && figure6.color != _figure.color) positions.Add(pos6);
                if (figure7 != null && figure7.color != _figure.color) positions.Add(pos7);

                break;
            case FigureType.kn: positions.AddRange(FigureMovement.Knight); break;
            case FigureType.b: positions.AddRange(FigureMovement.Bishop); break;
            case FigureType.r: positions.AddRange(FigureMovement.Rook); break;
            case FigureType.q: positions.AddRange(FigureMovement.Queen); break;
            case FigureType.kg: positions.AddRange(FigureMovement.King); break;
        }
        for (int p = 0; p < positions.Count; p++)
        {
            Vector3Int pos = _figure.pos + positions[p].pos;
            if (!IsPosInBoard(pos)) continue;
            GameObject moveCeil = Instantiate(moveCeilPrefab, GetCeilWorldPos(pos), Quaternion.identity, boardOrigin);
            moveCeil.GetComponent<MeshRenderer>().material.color = ceilColors[positions[p].ceilStatus == CeilStatus.figureUnderAttack ? 2 : 0];
            if (positions[p].ceilStatus == CeilStatus.figureUnderAttack) moveCeil.tag = "figureUnderAttackCeil";
            moveCeil.name = "move ceil " + _figure.type.ToString();
            moveCeils.Add(moveCeil);
        }
    }
    [Tooltip("0 - default ceil\n1 - selected ceil\n2 - figure under attack ceil\n3 - selected figure under attack ceil")]
    public Color[] ceilColors;
    public Vector3Int SelectCeil(Vector3 pos, GameObject ceilObject)
    {
        Vector3Int ceilPos = GetCeilPos(pos);
        for (int c = 0; c < moveCeils.Count; c++) moveCeils[c].GetComponent<Renderer>().material.color = ceilColors[moveCeils[c].tag == "figureUnderAttackCeil" ? 2 : 0];
        ceilObject.GetComponent<Renderer>().material.color = ceilColors[ceilObject.tag == "figureUnderAttackCeil" ? 3 : 1];
        return ceilPos;
    }
    public void MoveFigure(Vector3Int originPos, Vector3Int newPos)
    {
        if (!IsPosInBoard(newPos)) return;
        if (boards[newPos.y].ceil[newPos.x, newPos.z] != null)
        {
            DeleteFigure(newPos);
        }
        boards[newPos.y].ceil[newPos.x, newPos.z] = boards[originPos.y].ceil[originPos.x, originPos.z];
        boards[newPos.y].ceil[newPos.x, newPos.z].pos = newPos;
        if (boards[newPos.y].ceil[newPos.x, newPos.z].firstMove) boards[newPos.y].ceil[newPos.x, newPos.z].firstMove = false;
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
                SetFigureCeil(CreateFigure(FigureType.q, FigureColor.black, newPos));
            }
        }
        boards[originPos.y].ceil[originPos.x, originPos.z] = null;

    }
    public static Vector3 GetCeilWorldPos(Vector3Int pos)
    {
        return new Vector3(pos.x, pos.y, pos.z) * instance.ceilSize + instance.boardOrigin.position;
    }
    public static Vector3Int GetCeilPos(Vector3 pos)
    {
        pos = pos / instance.ceilSize;
        pos -= instance.boardOrigin.position;
        return new Vector3Int((byte)Mathf.RoundToInt(pos.x), (byte)Mathf.RoundToInt(pos.y), (byte)Mathf.RoundToInt(pos.z));
    }
    public void SetFigureCeil(Figure _figure)
    {
        if (!IsPosInBoard(_figure.pos)) return;
        boards[_figure.pos.y].ceil[_figure.pos.x, _figure.pos.z] = _figure;
    }
    public void DeleteFigure(Vector3Int pos)
    {
        if (boards[pos.y].ceil[pos.x, pos.z] != null)
        {
            Destroy(boards[pos.y].ceil[pos.x, pos.z].obj);
            boards[pos.y].ceil[pos.x, pos.z] = null;
        }
    }
    public static Figure GetFigureCeil(Vector3Int _pos)
    {
        if (!IsPosInBoard(_pos)) return null;
        return instance.boards[_pos.y].ceil[_pos.x, _pos.z];
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
            SetFigureCeil(CreateFigure(FigureType.p, FigureColor.white, new Vector3Int(1, 1, (byte)z)));
            SetFigureCeil(CreateFigure(FigureType.p, FigureColor.black, new Vector3Int(6, 6, (byte)z)));
        }
        //Knight
        SetFigureCeil(CreateFigure(FigureType.kn, FigureColor.white, new Vector3Int(0, 0, 1)));
        SetFigureCeil(CreateFigure(FigureType.kn, FigureColor.black, new Vector3Int(7, 7, 1)));

        SetFigureCeil(CreateFigure(FigureType.kn, FigureColor.white, new Vector3Int(0, 0, 6)));
        SetFigureCeil(CreateFigure(FigureType.kn, FigureColor.black, new Vector3Int(7, 7, 6)));
        //Bishop
        SetFigureCeil(CreateFigure(FigureType.b, FigureColor.white, new Vector3Int(0, 0, 2)));
        SetFigureCeil(CreateFigure(FigureType.b, FigureColor.black, new Vector3Int(7, 7, 2)));

        SetFigureCeil(CreateFigure(FigureType.b, FigureColor.white, new Vector3Int(0, 0, 5)));
        SetFigureCeil(CreateFigure(FigureType.b, FigureColor.black, new Vector3Int(7, 7, 5)));
        //Rook
        SetFigureCeil(CreateFigure(FigureType.r, FigureColor.white, new Vector3Int(0, 0, 0)));
        SetFigureCeil(CreateFigure(FigureType.r, FigureColor.black, new Vector3Int(7, 7, 0)));

        SetFigureCeil(CreateFigure(FigureType.r, FigureColor.white, new Vector3Int(0, 0, 7)));
        SetFigureCeil(CreateFigure(FigureType.r, FigureColor.black, new Vector3Int(7, 7, 7)));
        //Queen
        SetFigureCeil(CreateFigure(FigureType.q, FigureColor.white, new Vector3Int(0, 0, 3)));
        SetFigureCeil(CreateFigure(FigureType.q, FigureColor.black, new Vector3Int(7, 7, 4)));
        //King
        SetFigureCeil(CreateFigure(FigureType.kg, FigureColor.white, new Vector3Int(0, 0, 4)));
        SetFigureCeil(CreateFigure(FigureType.kg, FigureColor.black, new Vector3Int(7, 7, 3)));
    }
    static Vector3Int SelectedFigurePos()
    {
        return TDChessController.instance.selectedFigure.pos;
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene("Game");
    }
}