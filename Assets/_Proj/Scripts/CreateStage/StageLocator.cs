using System.Threading.Tasks;
using UnityEngine;
// Resources 폴더에

public interface IStageSource
{
    Task<string> LoadAsync(string key); // Stages/Stage-01
}

public class ResourcesStageSource : IStageSource
{
    public async Task<string> LoadAsync(string key)
    {
        var req = Resources.LoadAsync<TextAsset>(key); // 확장자/Resources 폴더명 제외
        while (!req.isDone)
        {
            await Task.Yield();
        }
        return (req.asset as TextAsset)?.text;
    }
}
public static class StageLocator
{
    public static IStageSource Source { get; private set; } = new ResourcesStageSource();
    public static Task<string> LoadAsync(string key) => Source.LoadAsync(key);
}
