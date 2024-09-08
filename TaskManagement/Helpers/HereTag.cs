using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace TaskManagement.Helpers
{
    [HtmlTargetElement("div", Attributes = "here-when")]
    [HtmlTargetElement("a", Attributes = "here-when")]
    public class HereTag: TagHelper
    {

        public string? HereWhen { get; set; }

        [ViewContext]
        [HtmlAttributeNotBound]
        public ViewContext? ViewContextData { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (HereWhen == null)
                return;

            var targetControllers = HereWhen.Split(",");

            var currentController = ViewContextData?.RouteData.Values["controller"]?.ToString();

            foreach (var controller in targetControllers)
            {
                if (controller.Equals(currentController))
                {
                    if (output.Attributes.ContainsName("class"))
                        output.Attributes.SetAttribute("class", $"{output.Attributes["class"].Value} here show active");

                    else
                        output.Attributes.SetAttribute("class", "here show active");

                    break;
                }
            }
        }
    }
}
