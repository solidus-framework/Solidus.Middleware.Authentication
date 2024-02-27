
namespace Solidus.Middleware.Authentication.Account.AccountService.UnitTests;

public class DefaultAccountServiceSpecificationTests : AccountServiceSpecificationTestsBase
{
    protected override IAccountService CreateSut(
        IAccountStorage accountStorage,
        IAccountPasswordHasher accountPasswordHasher)
    {
        return new DefaultAccountService(
            accountStorage: accountStorage,
            accountPasswordHasher: accountPasswordHasher
        );
    }
}
