using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TDChessController : MonoBehaviour
{
    public static TDChessController instance;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }
    public bool chessMove = false;
    public LayerMask layerChess;
    public LayerMask layerCeil;
    public Camera cameraPos;
    public Vector3Byte ceilPos;
    public TDChessManager.Figure selectedFigure;
    public Transform selectingBox;
    public void ThrowRay(Vector2 mousePos)
    {
        RaycastHit hit;
        Ray ray = cameraPos.ScreenPointToRay(mousePos);
        Debug.DrawRay(ray.origin, ray.direction * 20, Color.red, 0.5f);
        if (Physics.Raycast(ray, out hit, 20, chessMove ? layerCeil : layerChess))
        {
            if (chessMove)
            {
                if ((layerCeil.value & (1 << hit.collider.gameObject.layer)) > 0)
                {
                    ceilPos = TDChessManager.instance.SelectCeil(hit.collider.transform.position, hit.collider.gameObject);
                }
            }
            else
            {
                if ((layerChess.value & (1 << hit.collider.gameObject.layer)) > 0)
                {
                    Vector3Byte figurePos = TDChessManager.GetCeilPos(hit.collider.transform.position);
                    selectedFigure = TDChessManager.GetFigureCeil(figurePos);
                    selectingBox.transform.position = hit.collider.transform.position;
                    selectingBox.transform.localScale = hit.collider.GetComponent<BoxCollider>().size;
                }
            }
        }
    }
    public GameObject selectFigureButton;
    public GameObject selectMoveFigureButton;
    public void OnFigureSelected()
    {
        chessMove = true;
        selectFigureButton.SetActive(false);
        selectMoveFigureButton.SetActive(true);
        TDChessManager.instance.CreateMoveCeils(selectedFigure);
    }
    public void OnFigureMoveSelected()
    {
        TDChessManager.instance.MoveFigure(selectedFigure.pos, ceilPos);
        EndMove();
    }
    public void EndMove()
    {
        chessMove = false;
        TDChessManager.instance.CreateMoveCeils(null);
        selectFigureButton.SetActive(true);
        selectMoveFigureButton.SetActive(false);
        selectingBox.transform.localScale = Vector3.zero;
        ceilPos = Vector3Byte.zero;
        selectedFigure = null;
    }
}
