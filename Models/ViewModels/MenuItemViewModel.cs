using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YemekSiparisWeb.Models.ViewModels
{
    public class MenuItemViewModel
    {
        public MenuItem MenuItem { get; set; }//name,description,price gibi alanlara ulasmak icin nesne tanımladık.
        public IEnumerable<Category> CategoryList { get; set; }//dropdowndan kategori secmek icin 
        public IEnumerable<SubCategory> SubCategoryList { get; set; }//dropdowndan sub kategori secmek icin 



    }
}
