using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoggleClient
{
    public interface IBoggleView
    {
        /// <summary>
        /// Fired when user must be registered.
        /// Parameters are name and email.
        /// </summary>
        event Action<string, string> RegisterPressed;
    }
}
