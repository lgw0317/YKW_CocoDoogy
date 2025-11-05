using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ProfileSource", menuName = "GameData/Profile Source")]
public class ProfileSource : MonoBehaviour, IProfileSource
{
    [Header("Database")]
    public AnimalDatabase animalDatabase;
    public DecoDatabase decoDatabase;
    public CostumeDatabase costumeDatabase;
    public ArtifactDatabase artifactDatabase;
    public Profile_iconDatabase profileIconDatabase;

    private readonly IResourceLoader loader = new ResourcesLoader();

    public IReadOnlyList<ProfileEntry> GetAll()
    {
        var list = new List<ProfileEntry>();
        AppendEntries(list, "동물친구", animalDatabase?.animalList);
        AppendEntries(list, "조경품", decoDatabase?.decoList);
        AppendEntries(list, "치장품", costumeDatabase?.costumeList);
        AppendEntries(list, "유물", artifactDatabase?.artifactList);
        AppendEntries(list, "프로필 선택", profileIconDatabase?.profileList);
        return list;
    }

    public IReadOnlyList<ProfileEntry> GetByCategory(string category)
    {
        var list = new List<ProfileEntry>();

        switch (category)
        {
            case "동물친구":
                AppendEntries(list, "동물친구", animalDatabase?.animalList);
                break;
            case "조경품":
                AppendEntries(list, "조경품", decoDatabase?.decoList);
                break;
            case "치장품":
                AppendEntries(list, "치장품", costumeDatabase?.costumeList);
                break;
            case "유물":
                AppendEntries(list, "유물", artifactDatabase?.artifactList);
                break;
            case "프로필 선택":
                AppendEntries(list, "프로필 선택", profileIconDatabase?.profileList);
                break;
        }

        return list;
    }

    private void AppendEntries<T>(List<ProfileEntry> list, string category, List<T> dataList)
    {
        if (dataList == null) return;

        foreach (var item in dataList)
        {
            switch (item)
            {
                case AnimalData animal:
                    list.Add(new ProfileEntry(animal.animal_id, animal.GetIcon(loader), category));
                    break;
                case DecoData deco:
                    list.Add(new ProfileEntry(deco.deco_id, deco.GetIcon(loader), category));
                    break;
                case CostumeData costume:
                    list.Add(new ProfileEntry(costume.costume_id, costume.GetIcon(loader), category));
                    break;
                case ArtifactData artifact:
                    list.Add(new ProfileEntry(artifact.artifact_id, artifact.GetIcon(loader), category));
                    break;
                case Profile_iconData profile:
                    list.Add(new ProfileEntry(profile.icon_id, profile.GetIcon(loader), category));
                    break;
            }
        }
    }
}