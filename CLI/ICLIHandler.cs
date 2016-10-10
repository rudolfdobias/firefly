using Firefly.Properties;

namespace Firefly.CLI{
    public interface ICLIHandler{
        void HandleCommand(CLIActions action);
    }
}