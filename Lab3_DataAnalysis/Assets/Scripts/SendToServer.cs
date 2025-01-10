using Gamekit3D;
using Gamekit3D.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Networking;
using UnityEngine.Windows;

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
        Damageable.DamageMessage dmgMsg = (Damageable.DamageMessage)msg;

        LogHit(dmgMsg.damageSource, dmgMsg.direction, dmgMsg.damager, dmgMsg.amount, dmgMsg.throwing, type == MessageType.DEAD);

    }

    public void ReceiveDataBaseHit()
    {
        heatMapParent.ClearAll();
        WWWForm form = new WWWForm();
        StartCoroutine(ReadForm(form, "https://citmalumnes.upc.es/~xavierac8/Assignment3/readhit.php", true));
    }

    public void ReceiveDataBasePath()
    {
        pathParent.ClearAll();
        WWWForm form = new WWWForm();
        StartCoroutine(ReadForm(form, "https://citmalumnes.upc.es/~xavierac8/Assignment3/readpath.php", false));
    }

    public void LogHit(Vector3 pos, Vector3 dir, MonoBehaviour source, int dmgAmount, bool isThrowing, bool isDeath)
    {
        WWWForm form = new WWWForm();
        form.AddField("Position", Vec3ToString(pos));
        form.AddField("Direction", Vec3ToString(dir));
        form.AddField("Source", source.ToString());
        form.AddField("Damage", dmgAmount.ToString());
        form.AddField("isThrowing", Convert.ToInt32(isThrowing).ToString());
        form.AddField("isDeath", Convert.ToInt32(isDeath).ToString());
        StartCoroutine(Upload(form, "https://citmalumnes.upc.es/~xavierac8/Assignment3/hit.php"));

        heatMapParent.CreateCubeOnHeatMap(pos);
    }

    public void LogPath(Vector3 pos, int sessionID)
    {
        WWWForm form = new WWWForm();
        form.AddField("PathStep", Vec3ToString(pos));
        form.AddField("SessionID", sessionID.ToString());
        StartCoroutine(Upload(form, "https://citmalumnes.upc.es/~xavierac8/Assignment3/path.php"));

        pathParent.CreateSphereOnPathDot(pos);
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
            LogPath(new Vector3(mainPlayer.transform.position.x, mainPlayer.transform.position.y + 1, mainPlayer.transform.position.z), id);
            heatMapParent.CheckCubesCollisions();
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
                Debug.Log(www.error);
            }
        }
    }

    IEnumerator ReadForm(WWWForm form, string url, bool isHit)
    {
        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                if (isHit)
                    ProcessReceivedDataHit(www.downloadHandler.text);
                else
                    ProcessReceivedDataPath(www.downloadHandler.text);
            }
        }
    }

    void ProcessReceivedDataHit(string input)
    {
        var lines = input.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines)
        {
            if (line.StartsWith("Connected to database"))
                continue;

            Vector3 position = new Vector3(0,0,0);
            Vector3 direction = new Vector3(0, 0, 0);
            string source = "";
            int damage = 0;
            bool isThrowing = false;
            bool isDeath = false;

            var parts = line.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var part in parts)
            {
                var trimmedPart = part.Trim();

                if (trimmedPart.StartsWith("Position:"))
                {
                    position = ParseVec3(trimmedPart.Replace("Position:", string.Empty).Trim());
                }
                else if (trimmedPart.StartsWith("Direction:"))
                {
                    direction = ParseVec3(trimmedPart.Replace("Direction:", string.Empty).Trim());
                }
                else if (trimmedPart.StartsWith("Source:"))
                {
                    source = trimmedPart.Replace("Source:", string.Empty).Trim();
                }
                else if (trimmedPart.StartsWith("Damage:"))
                {
                    if (int.TryParse(trimmedPart.Replace("Damage:", string.Empty).Trim(), out int damage1))
                    {
                        damage = damage1;
                    }
                }
                else if (trimmedPart.StartsWith("isThrowing:"))
                {
                    if (bool.TryParse(trimmedPart.Replace("isThrowing:", string.Empty).Trim(), out bool isThrowing1))
                    {
                        isThrowing = isThrowing1;
                    }
                }
                else if (trimmedPart.StartsWith("isDeath:"))
                {
                    if (bool.TryParse(trimmedPart.Replace("isDeath:", string.Empty).Trim(), out bool isDeath1))
                    {
                        isDeath = isDeath1;
                    }
                }
            }
            //function to process 1 data
            for (int i = 0; i < damage; i++)
            {
                heatMapParent.CreateCubeOnHeatMap(position);
            }
        }
    }

    void ProcessReceivedDataPath(string input)
    {
        var lines = input.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var line in lines)
        {
            if (line.StartsWith("Connected to database"))
                continue;

            Vector3 position = new Vector3(0, 0, 0);
            int sessionID = 0;

            var parts = line.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var part in parts)
            {
                var trimmedPart = part.Trim();

                if (trimmedPart.StartsWith("PathStep:"))
                {
                    position = ParseVec3(trimmedPart.Replace("PathStep:", string.Empty).Trim());
                }
                else if (trimmedPart.StartsWith("SessionID:"))
                {
                    if (int.TryParse(trimmedPart.Replace("SessionID:", string.Empty).Trim(), out int sessionID1))
                    {
                        sessionID = sessionID1;
                    }
                }

            }
            //function to process 1 data
            pathParent.CreateSphereOnPathDot(position);
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