using System.Threading.Tasks;

namespace MyTenantWorld
{
    public interface IAzureRest
    {
        HomeProfile GetHomeProfile(string authorisation);
    }
}