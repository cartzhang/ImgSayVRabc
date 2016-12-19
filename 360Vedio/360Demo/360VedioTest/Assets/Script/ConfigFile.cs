using UnityEngine;
using System;
using System.IO;
using System.Xml.Serialization;

public class ConfigFile : MonoBehaviour {

    public static ConfigFile Instance { private set; get; }
    public static ClientConfigReference Cconfig;
    
    void Awake()
    {
        try
        {
            ReadConfigFile(Application.streamingAssetsPath + "/Configfile.xml");
        }
        catch (Exception ex)
        {
            Debug.LogError("Read XML Failed !  " + ex.Message);
        }
    }

    void ReadConfigFile(string filePath)
    {
        try
        {
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            XmlSerializer xs = new XmlSerializer(typeof(ClientConfigReference));
            Cconfig = xs.Deserialize(fs) as ClientConfigReference;
            fs.Dispose();
            Debug.Log("Configfile :" + filePath);
        }
        catch (Exception ex)
        {
            Debug.LogError("Read ConfigFile Failed !  " + ex.Message);
        }
    }
}

// configure class
public class ClientConfigReference
{
    public string VediofullPathName;
}


