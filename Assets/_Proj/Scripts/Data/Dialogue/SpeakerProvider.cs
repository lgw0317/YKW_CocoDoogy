using UnityEngine;

public class SpeakerProvider : IDataProvider<string, SpeakerData>
{
    private SpeakerDatabase database;
    private IResourceLoader loader;

    public SpeakerProvider(SpeakerDatabase db, IResourceLoader resLoader)
    {
        database = db; ;
        loader = resLoader;
    }

    public SpeakerData GetData(string id)
    {
        return database.speakerList.Find(s => s.speaker_id == id);
    }

    public Sprite GetPortrait(string id, EmotionType emotion)
    {
        var data = GetData(id);
        return data?.GetPortrait(emotion, loader);
    }
}
