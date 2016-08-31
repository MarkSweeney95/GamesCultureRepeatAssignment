using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WebAPIAuthenticationClient
{
    [JsonObject]
   public class Registration
    {
        [JsonProperty]
        public string Email { get; set; }
        [JsonProperty]
        public string Password { get; set; }
        [JsonProperty]
        public string ConfirmPassword { get; set; }
    }
}
