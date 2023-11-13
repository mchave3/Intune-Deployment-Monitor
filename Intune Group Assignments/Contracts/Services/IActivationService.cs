namespace Intune_Group_Assignments.Contracts.Services;

public interface IActivationService
{
    Task ActivateAsync(object activationArgs);
}
