using System.Threading.Tasks;

namespace k8s_disaster_recovery_net_console.API
{
    public interface IController
    {
        object Response { get; }

        string ContentType { get; }
    }
}