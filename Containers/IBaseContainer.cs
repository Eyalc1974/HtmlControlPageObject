using System;
namespace Automation.Core.Containers
{
    public interface IBaseContainer
    {
        void Focus();
        void WaitToLoad();
        void WaitToLoadAjax();
        Automation.Core.Infrastructure.WebRunner WebRunner { get; }
        IBaseContainer Parent { get; }
        
    }
}
