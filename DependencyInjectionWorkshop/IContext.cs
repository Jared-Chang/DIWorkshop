namespace DependencyInjectionWorkshop
{
    public interface IContext
    {
        User GetUser();
        void SetUser(string name);
    }
}