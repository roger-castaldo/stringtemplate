using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Reddragonit.Stringtemplate.Interfaces
{
    public interface IParentComponent : IComponent
    {
        List<IComponent> Children
        {
            get;
        }
    }
}
