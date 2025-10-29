using System.Collections.Generic;

[System.Serializable]
public class TableMetaList
{
    public List<TableMetaEntry> entries;
}

[System.Serializable]
public class TableMetaEntry
{
    public string name;
    public string type;
}