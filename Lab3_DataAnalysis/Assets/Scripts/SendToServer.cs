using Gamekit3D;
using Gamekit3D.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Networking;

public class SendToServer : MonoBehaviour, IMessageReceiver
{
    public GameObject mainPlayer;

    public HeatMapParent heatMapParent;
    public PathParent pathParent;

    private float sendPositionTime = 0.2f;
    private float sendPositionTimer = 0.0f;

    int id;

    // Start is called before the first frame update
    void Start()
    {
        id = GenerateRandomID();
    }
    public void OnReceiveMessage(MessageType type, object sender, object msg) //Esto se triggerea cuando el personaje principal recibe daño
    {
        Debug.Log("Message Recieved");

        Damageable.DamageMessage dmgMsg = (Damageable.DamageMessage)msg;

        LogHit(dmgMsg.damageSource, dmgMsg.direction, dmgMsg.damager, dmgMsg.amount, dmgMsg.throwing, type == MessageType.DEAD);

    }


    public void LogHit(Vector3 pos, Vector3 dir, MonoBehaviour source, int dmgAmount, bool isThrowing, bool isDeath)
    {
        string text = "";

        WWWForm form = new WWWForm();
        form.AddField("Position", Vec3ToString(pos));
        form.AddField("Direction", Vec3ToString(dir));
        form.AddField("Source", source.ToString());
        form.AddField("Damage", dmgAmount.ToString());
        form.AddField("isThrowing", Convert.ToInt32(isThrowing).ToString());
        form.AddField("isDeath", Convert.ToInt32(isDeath).ToString());
        StartCoroutine(Upload(form, "https://citmalumnes.upc.es/~xavierac8/Assignment3/hit.php"));           //Claro, poner solo "Upload(form)", siendo que es una corrutina y no se ejecuta de manera normal, no funciona. He de invocarla o añadirla a la lista de procesos de alguna manera
        
        //READFORM es la funcion que lee de la database
        
        StartCoroutine(ReadForm(form, text, "https://citmalumnes.upc.es/~xavierac8/Assignment3/readhit.php")); //He hecho que cada vez que grabe lea
        StartCoroutine(ReadForm(form, text, "https://citmalumnes.upc.es/~xavierac8/Assignment3/readpath.php")); //He hecho que cada vez que grabe lea
        Debug.Log("Text data: ");
        Debug.Log(text);
    }

    //Funcion que logea el path en la database
    public void LogPath(Vector3 pos, int sessionID)
    {
        WWWForm form = new WWWForm();
        form.AddField("PathStep", Vec3ToString(pos));
        form.AddField("SessionID", sessionID.ToString());
        StartCoroutine(Upload(form, "https://citmalumnes.upc.es/~xavierac8/Assignment3/path.php"));
    }

    public void LogPosition(DateTime time, Vector3 pos)
    {
        WWWForm form = new WWWForm();
        form.AddField("DataType", "Position");
        form.AddField("Position", Vec3ToString(pos));
        form.AddField("PositionTime", time.ToString());
        Upload(form, "https://citmalumnes.upc.es/~xavierac8/Assignment3/hit.php");
    }

    public void LogPositionOnHit()
    {
        Vector3 pos = new Vector3(mainPlayer.transform.position.x, mainPlayer.transform.position.y + 1, mainPlayer.transform.position.z);
        DateTime time = DateTime.Now;

        WWWForm form = new WWWForm();
        form.AddField("DataType", "PositionOnHit");
        form.AddField("PositionOnHit", Vec3ToString(pos));
        form.AddField("PositionTime", time.ToString());
        Upload(form, "https://citmalumnes.upc.es/~xavierac8/Assignment3/hit.php");
    }

    public void LogPositionLocal(DateTime time, Vector3 pos)
    {
        ProcessReceivedData($"Position,{Vec3ToString(pos)},{time.ToString()}");
    }

    public void LogPositionOnHitLocal()
    {
        Vector3 pos = new Vector3(mainPlayer.transform.position.x, mainPlayer.transform.position.y + 1, mainPlayer.transform.position.z);
        DateTime time = DateTime.Now;
        ProcessReceivedData($"PositionOnHit,{Vec3ToString(pos)},{time.ToString()}");
    }

    private string Vec3ToString(Vector3 pos)
    {
        return $"x:{pos.x.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)} " +
               $"y:{pos.y.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)} " +
               $"z:{pos.z.ToString("F2", System.Globalization.CultureInfo.InvariantCulture)}";
    }


    // Update is called once per frame
    void Update()
    {
        sendPositionTimer += Time.deltaTime;

        if (sendPositionTimer >= sendPositionTime)
        {
            LogPositionLocal(DateTime.Now, new Vector3(mainPlayer.transform.position.x, mainPlayer.transform.position.y + 1, mainPlayer.transform.position.z));
            
            //Aqui logeamos el path en la database con cada step
            LogPath(new Vector3(mainPlayer.transform.position.x, mainPlayer.transform.position.y + 1, mainPlayer.transform.position.z), id);
            sendPositionTimer = 0.0f;
        }
    }

    IEnumerator Upload(WWWForm form, string url)
    {
        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Upload error:");
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Form Upload Completed");
                Debug.Log(www.downloadHandler.text);
            }
        }
    }

    IEnumerator ReadForm(WWWForm form, string formData, string url)
    {
        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("Upload error:");
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Form Read Completed");
                Debug.Log(www.downloadHandler.text);
                formData = www.downloadHandler.text;
            }
        }
    }

    IEnumerator ReceiveData()
    {
        string url = "https://citmalumnes.upc.es/~xavierac8/Assignment3/hit.php";

        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error: " + www.error);
            }
            else
            {
                string responseText = www.downloadHandler.text;
                Debug.Log("Datos recibidos: " + responseText);
                ProcessReceivedData(responseText);
            }
        }
    }

    void ProcessReceivedData(string data)
    {
        string[] entries = data.Split('\n', StringSplitOptions.RemoveEmptyEntries);

        foreach (string entry in entries)
        {
            string[] parts = entry.Split(',');

            if (parts.Length >= 3)
            {
                string dataType = parts[0];
                string positionPart = parts[1];
                string timePart = parts[2];

                if (DateTime.TryParse(timePart, out DateTime parsedTime))
                {
                    Vector3 parsedPosition = ParseVec3(positionPart);

                    if (dataType == "Position")
                    {
                        Debug.Log($"Position: Time: {parsedTime}, Position: {parsedPosition}");
                        pathParent.CreateSphereOnPathDot(parsedPosition);
                    }
                    else if (dataType == "PositionOnHit")
                    {
                        Debug.Log($"PositionOnHit: Time: {parsedTime}, Position: {parsedPosition}");
                        heatMapParent.CreateCubeOnHeatMap(parsedPosition);
                    }
                    else
                    {
                        Debug.LogWarning("Unknown data type: " + dataType);
                    }
                }
                else
                {
                    Debug.LogError("Invalid time format: " + timePart);
                }
            }
            else
            {
                Debug.LogError("Invalid entry format: " + entry);
            }
        }
    }

    Vector3 ParseVec3(string vecString)
    {
        float x = 0, y = 0, z = 0;

        string[] components = vecString.Split(' ');

        foreach (string component in components)
        {
            string[] keyValue = component.Split(':');

            if (keyValue.Length == 2)
            {
                if (keyValue[0] == "x" && float.TryParse(keyValue[1], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float parsedX))
                    x = parsedX;
                else if (keyValue[0] == "y" && float.TryParse(keyValue[1], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float parsedY))
                    y = parsedY;
                else if (keyValue[0] == "z" && float.TryParse(keyValue[1], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float parsedZ))
                    z = parsedZ;
            }
        }
        return new Vector3(x, y, z);
    }

    public int GenerateRandomID()
    {
        var r = new System.Random();
        return r.Next(1000000000, 2147483647);
    }

}