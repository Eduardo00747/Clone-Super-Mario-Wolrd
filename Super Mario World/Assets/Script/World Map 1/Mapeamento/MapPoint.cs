using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPoint : MonoBehaviour
{
    [Header("Conexões")]
    public MapPoint left;
    public MapPoint right;
    public MapPoint up;
    public MapPoint down;

    [Header("Próximo ponto do caminho")]
    public MapPoint nextPoint;
    public MapPoint previousPoint;


    [Header("Tipo")]
    public bool isFinalPoint;
}
