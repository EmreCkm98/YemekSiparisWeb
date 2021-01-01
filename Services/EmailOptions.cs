using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YemekSiparisWeb.Services//email gonderme islemlerini yapıcaz burda.bunun icin sendgrid apisini kulanıcaz.sendgride kayıt oldum.
{//sol menuden emre cakmak-account detaile giriyoruz.sol menude asagıda api key kısmına giriyoruz.create api key tıklıyoruz.api key name kısmına spice
    //yazdık ful accces secili.create view tıklıyoruz.api key olusturuldu.kopyalıyoruz.stripeda oldugu gibi appseting.json dosyasına api keyini yazıcaz.
    //kullanmak icin.//appsetingjsondaki apikeyi asagıdaki propa aktarıcaz.bunun icin startupa.csye geciyoruz.
    public class EmailOptions
    {
        public string SendGridKey { get; set; }//dependency injection ile api keyi bu propa aktarmamız lazım.bunun icin email sender clasında yapıcaz.
    }
}
