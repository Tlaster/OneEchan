using HeyRed.MarkdownSharp;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OneEchan.Server.TagHelpers
{

    [HtmlTargetElement("markdown", TagStructure = TagStructure.NormalOrSelfClosing)]
    [HtmlTargetElement(Attributes = "markdown")]
    public class MarkdownTagHelper : TagHelper
    {
        public ModelExpression Content { get; set; }
        public async override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (output.TagName == "markdown")
            {
                output.TagName = null;
            }
            output.Attributes.RemoveAll("markdown");

            var content = await GetContent(output);
            var markdown = content;
            var html = new Markdown().Transform(markdown);
            output.Content.SetHtmlContent(html ?? "");
        }


        private async Task<string> GetContent(TagHelperOutput output)
        {
            if (Content == null)
                return (await output.GetChildContentAsync()).GetContent();

            return Content.Model?.ToString();
        }
    }
}
