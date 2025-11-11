using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Profile_iconData", menuName = "GameData/Profile_iconData")]
public class Profile_iconDatabase : ScriptableObject, IEnumerable<Profile_iconData>
{
    public List<Profile_iconData> profileList = new List<Profile_iconData>();
    public IEnumerator<Profile_iconData> GetEnumerator()
    {
        return profileList.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
