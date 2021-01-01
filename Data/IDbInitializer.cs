using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YemekSiparisWeb.Data
{//programı deploy ederken register sayfasında kayıtr olurken user default olarak customer oluyor.register.csdeki rol atama satırını yorum satırı yaptık.
    //
    public interface IDbInitializer
    {
        void Initialize();
    }
}
