using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionTest : MonoBehaviour
{
    private LineRenderer _lineRenderer;
    private NeuralConnection connection;


    public void setConnection(NeuralConnection connection){
        _lineRenderer = GetComponent<LineRenderer>();
        this.connection = connection;

        _lineRenderer.positionCount = 2;
        _lineRenderer.SetPosition(0,new Vector3(connection.N1.GetLayerID()*2-2, connection.N1.GetQueueID()*1.2f-1.2f,0));
        _lineRenderer.SetPosition(1,new Vector3(connection.N2.GetLayerID()*2-2, connection.N2.GetQueueID()*1.2f-1.2f,0));
    }

    private void Update() {
        _lineRenderer.enabled = connection.N1.GetIsFired();
    }


}
