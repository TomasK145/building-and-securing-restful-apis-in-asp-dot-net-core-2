namespace LandonApi.Infrastructure
{
    public interface IEtaggable
    {
        string GetEtag();
    }
}