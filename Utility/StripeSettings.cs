using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YemekSiparisWeb.Utility
{
    public class StripeSettings//nugetten stripe.net dosyasını indircez.startup.csde bu clası eklicez.
    {
        public string SecretKey { get; set; }
        public string PublishableKey { get; set; }
    }
}
