using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPASideLoader.ViewModels.Interfaces
{
    public interface IWindowEventAwareViewModel
    {
        void WindowClosed();
        void WindowShowed();
    }
}
