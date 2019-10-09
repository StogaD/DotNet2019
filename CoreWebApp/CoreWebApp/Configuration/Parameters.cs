using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreWebApp.Configuration
{
    public class Parameters
    {
        public int Speed { get; set; }
        public int Acceleration { get; set; }
        public  string Name { get; set; }
        
        public int Version { get; set; }
    }

    public class KeyVaultOptions
    {
        public string KeyVaultName { get; set; }
        public string SecretName { get; set; }
    }
}
