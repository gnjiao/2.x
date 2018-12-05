using Microsoft.Practices.Unity;

namespace Hdc.Boot
{
    public interface IBootstrapperExtension
    {
        void Initialize(IUnityContainer container);
    }
}