using System;
using System.Text;
using System.Web.Mvc;
using NaftanRailway.WebUI.ViewModels;

namespace NaftanRailway.WebUI.HtmlHelpers {
    /// <summary>
    /// Extenstion html helper basic class
    /// </summary>
    public static class PagingHelpers {
        /// <summary>
        /// The PageLinks extension method(HtmlHelper) generates the HTML for a set of page links 
        /// using the information provided in a PagingInfo object. 
        /// The Func parameter accepts a delegate that it uses to generate the links to view other pages.
        /// Bootsrap pagination
        /// </summary>
        /// <param name="html">Extention</param>
        /// <param name="pagingInfo">PagignInfo object</param>
        /// <param name="sizePatginatonBar">Size pagination bar</param>
        /// <param name="pageUrl">Route Url</param>
        /// <returns></returns>
        public static MvcHtmlString PageLinks(this HtmlHelper html, PagingInfo pagingInfo, int sizePatginatonBar, Func<int, string> pageUrl) {
            //if (html == null) throw new ArgumentNullException("html");

            StringBuilder result = new StringBuilder();

            //for best view paging (selection always in middle)
            int startIndex =(int)Math.Max(1,
                Math.Max(0, pagingInfo.CurrentPage - (int)Math.Round((double)sizePatginatonBar/2)) -
                Math.Max(0, Math.Round((double)(pagingInfo.CurrentPage - (pagingInfo.TotalPages - sizePatginatonBar)) / 2)));

            int endIndex = Math.Min(pagingInfo.TotalPages, startIndex + sizePatginatonBar);

            #region Previos Page
                TagBuilder ulTag = new TagBuilder("ul");
                ulTag.AddCssClass("pagination");
                TagBuilder stliTag = new TagBuilder("li");
                if(pagingInfo.CurrentPage == startIndex ) {
                       stliTag.AddCssClass("disabled");
                }
                TagBuilder atag = new TagBuilder("a");
                //retrive url
                atag.MergeAttribute("href", pagingInfo.CurrentPage > 1 ? pageUrl(pagingInfo.CurrentPage - 1) : pageUrl(1));
                atag.MergeAttribute("aria-label", "Previous");

                TagBuilder stspanTag = new TagBuilder("span");
                stspanTag.MergeAttribute("aria-hidden", "true");
                stspanTag.InnerHtml = "&laquo";

                atag.InnerHtml = stspanTag.ToString();
                stliTag.InnerHtml = atag.ToString();
                //ulTag.InnerHtml = stliTag.ToString();
                //result.Append(ulTag);
            #endregion

            #region PageLinks
            string linkPage = "";
                for(int i = startIndex; i <= endIndex; i++) {
                    TagBuilder lipageTag = new TagBuilder("li");
                    if(i == pagingInfo.CurrentPage) {
                        lipageTag.AddCssClass("active");
                    }

                    TagBuilder apagetag = new TagBuilder("a");
                    TagBuilder srSpan = new TagBuilder("span");
                        srSpan.AddCssClass("sr-only");
                    srSpan.InnerHtml = "(current)";
                    apagetag.MergeAttribute("href", pageUrl(i));
                        if(i == pagingInfo.CurrentPage) {
                            //lipageTag.AddCssClass("active");
                            apagetag.InnerHtml = i + srSpan.ToString(); 
                        }
                    apagetag.InnerHtml = i.ToString();

                    lipageTag.InnerHtml = apagetag.ToString();
                    linkPage =linkPage + lipageTag;
                    //result.Append(lipageTag);
                }
            #endregion

            #region Next Page
                TagBuilder endliTag = new TagBuilder("li");
                 if(pagingInfo.CurrentPage == endIndex ) {
                       endliTag.AddCssClass("disabled");
                    }
                TagBuilder nexttag = new TagBuilder("a");

                nexttag.MergeAttribute("href", pagingInfo.CurrentPage < pagingInfo.TotalPages ? pageUrl(pagingInfo.CurrentPage + 1): pageUrl(pagingInfo.TotalPages));
                nexttag.MergeAttribute("aria-label", "Next");

                TagBuilder endSpanTag = new TagBuilder("span");
                endSpanTag.MergeAttribute("aria-hidden", "true");
                endSpanTag.InnerHtml = "&raquo";

                nexttag.InnerHtml = endSpanTag.ToString();
                endliTag.InnerHtml = nexttag.ToString();
                //result.Append(endliTag);
            #endregion

            ulTag.InnerHtml = stliTag + linkPage + endliTag;
            result.Append(ulTag);

            return MvcHtmlString.Create(result.ToString());
        }
    }
}