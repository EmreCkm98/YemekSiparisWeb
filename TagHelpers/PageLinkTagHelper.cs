using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using YemekSiparisWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YemekSiparisWeb.TagHelpers//customer areasındaki viewimport dosyasında bu custom helperı belirticez.cunku o aredaki bir viewde kullanıcaz.
{
    [HtmlTargetElement("div",Attributes ="page-model")]//hedef tagımızı belirledik.ozelligini verdik.orderhistory viewinde kullanıcaz.
    public class PageLinkTagHelper:TagHelper
    {//tag helperlar bir html elementine özgü olur.bizde ilk once hangi html tagına helper yazıcaksak onu seciyoruz.biz div icin helepr yazıcaz.
        private IUrlHelperFactory _urlHelperFactory;
        public PageLinkTagHelper(IUrlHelperFactory urlHelperFactory)
        {
            _urlHelperFactory = urlHelperFactory;
        }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext ViewContext { get; set; }

        public PagingInfo PageModel { get; set; }
        public string PageAction { get; set; }
        public bool PageClassesEnabled { get; set; }//
        public string PageClass { get; set; }
        public string PageClassNormal { get; set; }
        public string PageClassSelected { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)//taghelper clasınnın overrride metodu
        {
            IUrlHelper urlHelper = _urlHelperFactory.GetUrlHelper(ViewContext);
            TagBuilder result = new TagBuilder("div");

            for (int i = 1; i < PageModel.TotalPage; i++)//burada sayfayı olusturuyoruz.mesela sayfanın altında 1,2,3 gibi.sayfaya tıklanınca biyere gidicek
            {//bunun icin a tagı ve href tagı olusturduk.gidecegi urli verdik.
                TagBuilder tag = new TagBuilder("a");
                string url = PageModel.UrlParam.Replace(":", i.ToString());
                tag.Attributes["href"] = url;
                if (PageClassesEnabled)
                {
                    tag.AddCssClass(PageClass);
                    tag.AddCssClass(i == PageModel.CurrentPage ? PageClassSelected : PageClassNormal);
                }
                tag.InnerHtml.Append(i.ToString());
                result.InnerHtml.AppendHtml(tag);
            }
            output.Content.AppendHtml(result.InnerHtml);

        }
    }
}
