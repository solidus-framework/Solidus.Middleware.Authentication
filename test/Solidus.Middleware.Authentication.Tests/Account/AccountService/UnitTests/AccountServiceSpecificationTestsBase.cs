namespace Solidus.Middleware.Authentication.Account.AccountService.UnitTests;

public abstract partial class AccountServiceSpecificationTestsBase : TestBase
{
    protected abstract IAccountService CreateSut(
        IAccountStorage accountStorage,
        IAccountPasswordHasher accountPasswordHasher);

    private IAccountService CreateSutInternal(
        IAccountStorage? accountStorage = null,
        IAccountPasswordHasher? accountPasswordHasher = null)
    {
        accountStorage ??= Mock.Of<IAccountStorage>();
        accountPasswordHasher ??= Mock.Of<IAccountPasswordHasher>();

        return CreateSut(
            accountStorage: accountStorage,
            accountPasswordHasher: accountPasswordHasher
        );
    }
}
