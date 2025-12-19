namespace TourneyMod.UI;

internal interface ICustomScreen<T>
{
    public void Init(T screenVanilla);
}