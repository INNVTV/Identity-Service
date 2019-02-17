using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Common.Emails
{
    public static class EmailButtonGenerator
    {
        public static string Generate(string link, string label, int width)
        {
            // Based on bulletproof HTML email buttons from Campaign Monitor:
            // https://buttons.cm/

            var height = 40;
            var backgroundColor = "#000000";
            var fontColor = "#ffffff";

            var emailButtonString = new StringBuilder();

            emailButtonString.Append($"<div><!--[if mso]>");
            emailButtonString.Append($"<v:roundrect xmlns:v='urn:schemas-microsoft-com:vml' xmlns:w='urn:schemas-microsoft-com:office:word' href='{link}' style='height:{height}px;v-text-anchor:middle;width:{width}px;' arcsize='13%' stroke='f' fillcolor='{backgroundColor}'>");
            emailButtonString.Append($"<w:anchorlock/>");
            emailButtonString.Append($"<center>");
            emailButtonString.Append($"<![endif]-->");
            emailButtonString.Append($"<a href='{link}'");
            emailButtonString.Append($"style='background-color:{backgroundColor};border-radius:5px;color:{fontColor};display:inline-block;font-family:sans-serif;font-size:13px;font-weight:bold;line-height:{height}px;text-align:center;text-decoration:none;width:{width}px;-webkit-text-size-adjust:none;'>{label}</a>");
            emailButtonString.Append($"<!--[if mso]>");
            emailButtonString.Append($"</center>");
            emailButtonString.Append($"</v:roundrect>");
            emailButtonString.Append($"<![endif]--></div>");

            return emailButtonString.ToString();
        }
    }
}
