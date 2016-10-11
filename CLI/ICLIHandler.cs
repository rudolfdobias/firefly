using System.Threading.Tasks;

namespace Firefly.CLI{
    public interface ICLIHandler{
        Task HandleCommand(CLIActions action);
    }
}