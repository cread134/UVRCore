using Unity.Netcode;

namespace Core.Networking
{
    public interface IGameNetwork
    {
        void StartServer();
        void StartClient();
        void StartHost();
    }
}
