using System.Collections.Generic;

public interface IProfileSource
{
    IReadOnlyList<ProfileEntry> GetAll();
    IReadOnlyList<ProfileEntry> GetByCategory(string category);
}