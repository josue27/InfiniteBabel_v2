using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Brinco;
public static class ConvertidorData 
{
    /// <summary>
    /// Deserealiza y convierte los datos a tipo Score_Save
    /// </summary>
    /// <param name="data">byte array que hay que pasar</param>
    /// <returns></returns>
    public static Saved_Data ByteArra_Deserealizar(byte[] data)
    {
        if (data != null)
        {
            string jsonStr = System.Text.Encoding.UTF8.GetString(data);
            Debug.Log("Informaicon deserealizada de nube" + jsonStr);
            Saved_Data saveRecuperado = JsonUtility.FromJson<Saved_Data>(jsonStr);

            return saveRecuperado;
        }
        return null;
    }

    /// <summary>
    /// Convierte a Json y luego  seraliza a bytes un objeto de clase Score_Save
    /// </summary>
    /// <param name="dataObj"> variable de tipo Score_Save</param>
    /// <returns></returns>
    public static byte[] SavedGameDataToByteArray(Saved_Data dataObj)
    {
        if (dataObj != null)
        {
            // Convert to json string
            string jsonStr = JsonUtility.ToJson(dataObj);
            Debug.Log(jsonStr);
            // Json string to byte[]
            return System.Text.Encoding.UTF8.GetBytes(jsonStr);
        }

        return null;
    }


}
