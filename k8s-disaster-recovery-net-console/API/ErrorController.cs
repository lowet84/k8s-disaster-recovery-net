using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace k8s_disaster_recovery_net_console.API
{
    public class ErrorController : IController
    {
        private readonly string _text;

        public ErrorController(string text)
        {
            _text = text;
        }

        public object Response => _text;
        public string ContentType => "text/plain";
    }
}
