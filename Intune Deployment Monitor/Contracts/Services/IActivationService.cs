namespace Intune_Deployment_Monitor.Contracts.Services;

public interface IActivationService
{
    Task ActivateAsync(object activationArgs);
}
