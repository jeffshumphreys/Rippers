using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RipSSIS
{
    public interface IMainArgumentDirector
    {
        int DirectToProcessAccordingToArguments(string[] args);
    }
}
