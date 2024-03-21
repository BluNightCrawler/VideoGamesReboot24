using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using VideoGamesReboot24.Models.ViewModels;

namespace VideoGamesReboot24.Infrastructure
{
    [HtmlTargetElement("div", Attributes = "page-model")]
    public class PageLinkTagHelper : TagHelper
    {
        private IUrlHelperFactory urlHelperFactory;
        public PageLinkTagHelper(IUrlHelperFactory helperFactory)
        {
            urlHelperFactory = helperFactory;
        }
        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext? ViewContext { get; set; }

        public PagingInfo? PageModel { get; set; }

        public string? PageAction { get; set; }

        public string? SystemFilter { get; set; }

        [HtmlAttributeName(DictionaryAttributePrefix = "page-url-")]
        public Dictionary<string, object> PageUrlValues { get; set; } = new Dictionary<string, object>();

        public override void Process(TagHelperContext context,
        TagHelperOutput output)
        {
            if (ViewContext != null && PageModel != null)
            {
                IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
                TagBuilder result = new TagBuilder("div");
                TagBuilder innerUl = new TagBuilder("ul");
                innerUl.AddCssClass("pagination");
                for (int i = 1; i <= PageModel.TotalPages; i++)
                {
                    TagBuilder innerLi = new TagBuilder("li");
                    innerLi.AddCssClass("page-item");
                    TagBuilder tag = new TagBuilder("a");
                    PageUrlValues["productPage"] = i;
                    tag.Attributes["href"] = SystemFilter!=null ? urlHelper.Action(PageAction, PageUrlValues) + "?system=" + SystemFilter : urlHelper.Action(PageAction, PageUrlValues);
                    tag.AddCssClass("page-link");
                    tag.InnerHtml.Append(i.ToString());
                    innerLi.InnerHtml.AppendHtml(tag);
                    innerUl.InnerHtml.AppendHtml(innerLi);
                }
                result.InnerHtml.AppendHtml(innerUl);
                output.Content.AppendHtml(result.InnerHtml);
            }
        }
    }
}
