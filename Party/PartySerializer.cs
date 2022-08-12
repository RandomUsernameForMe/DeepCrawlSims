using DeepCrawlSims.PartyNamespace;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml.Serialization;

class PartySerializer
{
    public static Party Deserialize(string filename)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        Party party = new Party();
        FileStream filestream;
        if (File.Exists(filename))
        {
            filestream = File.OpenRead(filename);
            party = (Party)formatter.Deserialize(filestream);
            filestream.Close();
        }
        else throw new FileLoadException();
        return party;
    }

    public static void Serialize(Party party, string filename)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        if (File.Exists(filename)) File.Delete(filename);

        FileStream filestream = File.Create(filename);
        formatter.Serialize(filestream, party);
        filestream.Close();

    }

}