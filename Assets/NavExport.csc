using UnityEngine;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine.AI;

public class NavExport : MonoBehaviour
{
    #region Public Attributes
    public Vector3 leftUpStart = Vector3.zero;
    public float accuracy = 0.2f;
    public int height = 250;
    public int wide = 100;
    #endregion

    #region Unity Messages

    void OnGUI()
    {
        if (GUILayout.Button("Export"))
        {
            exportPoint(leftUpStart, height, wide, accuracy);
        }
    }

    #endregion

    #region Public Methods

    public void Exp()
    {
        exportPoint(leftUpStart, wide, height, accuracy);
    }

    public void exportPoint(Vector3 startPos, int x, int y, float accuracy)
    {
        StringBuilder str = new StringBuilder();
        int[,] list = new int[x, y];
        str.Append("startpos=").Append(startPos).Append("\r\n");
        str.Append("height=").Append(y).Append("\r\nwide=").Append(x).Append("\r\naccuracy=").Append(accuracy).Append("\r\n");
        for (int i = 0; i < y; ++i)
        {
            str.Append("{");
            for (int j = 0; j < x; ++j)
            {
                int res = list[j, i];
                NavMeshHit hit;
                for (int k = 0; k < 1; ++k)
                {
                    if (NavMesh.SamplePosition(startPos + new Vector3(j * accuracy, k, -i * accuracy), out hit, 0.2f, NavMesh.AllAreas))
                    {
                        res = 1;
                        break;
                    }
                }
                Debug.DrawRay(startPos + new Vector3(j * accuracy, 0, -i * accuracy), Vector3.up, res == 1 ? Color.green : Color.red, 5);
                str.Append(res).Append(",");
            }
            str.Append("},\n");
        }
        Debug.Log(str.ToString());
        FileStream mapfile = new FileStream(".\\mapinfo.txt", FileMode.Create);
        byte[] data = System.Text.Encoding.ASCII.GetBytes(str.ToString());
        mapfile.Write(data,0,data.Length);
        mapfile.Flush();
        mapfile.Close();
    }
    #endregion

}

[CustomEditor(typeof(NavExport))]
public class NavExportHelper : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Export"))
        {
            var exp = target as NavExport;
            exp.Exp();
        }
    }
}
