using System;
namespace NG.Automation.Core.Containers
{
    public interface IBaseContainer
    {
        void Focus();
        void WaitToLoad();
        void WaitToLoadAjax();
        NG.Automation.Core.Infrastructure.WebRunner WebRunner { get; }
        IBaseContainer Parent { get; }
        
    }
}
