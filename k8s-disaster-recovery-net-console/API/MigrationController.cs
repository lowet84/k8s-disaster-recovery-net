using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Threading.Tasks;
using k8s_disaster_recovery_net_console.Model;
using Microsoft.Owin;

namespace k8s_disaster_recovery_net_console.API
{
    public class MigrationController : IController
    {
        private readonly IOwinContext _context;
        private static Reset CurrentReset;

        public MigrationController(IOwinContext context)
        {
            _context = context;
        }

        public object Response
        {
            get
            {
                if (_context.Request.Path.Value.StartsWith("/migration/initreset"))
                {
                    var reset = new Reset
                    {
                        Expire = DateTime.Now.AddMinutes(5),
                        Key = Guid.NewGuid().ToString()
                    };
                    CurrentReset = reset;
                    return reset;
                }
                if (_context.Request.Path.Value.StartsWith($@"/migration/performreset/{CurrentReset?.Key}") && (DateTime.Now < CurrentReset?.Expire))
                {
                    if (Utils.RunningMode == RunningMode.Reserve || Utils.RunningMode == RunningMode.Master)
                    {
                        CurrentReset = null;
                        Utils.Reset();
                        Utils.Promote();
                        return "Reserve promoted to master";
                    }
                    else
                    {
                        return "Not reserve";
                    }



                }
                return Utils.Migrate;
            }
        }

        public string ContentType => "application/json";

        private class Reset
        {
            public string Key { get; set; }
            public DateTime Expire { get; set; }
        }
    }
}
