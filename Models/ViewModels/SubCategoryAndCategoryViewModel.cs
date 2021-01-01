using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YemekSiparisWeb.Models.ViewModels
{
    public class SubCategoryAndCategoryViewModel//subcategory create viewinde categori listesi,subcategorisi listesi ve subcategori lazım!!!!
    {
        public IEnumerable<Category> CategoryList { get; set; }//dropdown icin liste
        public SubCategory SubCategory { get; set; }//name gibi alanları icin subcategoriden nesne
        public List<string> SubCategoryList { get; set; }//sağ tarafta tüm subcategorileri gostermek icin liste
        public string StatusMessage { get; set; }
    }
}
