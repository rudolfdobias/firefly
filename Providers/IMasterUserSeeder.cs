using System.Threading.Tasks;

namespace Firefly.Providers{
    public interface IMasterUserSeeder{
        void Seed();

        void Seed(string username);

        void Seed(string username, string role);
    }
}