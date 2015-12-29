using System;
using System.Text;
using System.Web.Mvc;
using NaftanRailway.WebUI.Models;

namespace NaftanRailway.WebUI.HtmlHelpers {
    /// <summary>
    /// extenstion html helper basic class
    /// </summary>
    public static class PagingHelpers {
        /// <summary>
        /// The PageLinks extension method generates the HTML for a set of page links 
        /// using the information provided in a PagingInfo object. 
        /// The Func parameter accepts a delegate that it uses to generate the links to view other pages.
        /// </summary>
        /// <param name="html">Extention</param>
        /// <param name="pagingInfo">PagignInfo object</param>
        /// <param name="sizePatginatonBar">Size pagination bar</param>
        /// <param name="pageUrl">Route Url</param>
        /// <returns></returns>
        public static MvcHtmlString PageLinks(this HtmlHelper html,PagingInfo pagingInfo,int sizePatginatonBar,Func<int,string> pageUrl) {
            //if (html == null) throw new ArgumentNullException("html");

            StringBuilder result = new StringBuilder();

            int startIndex =(int) Math.Max(1,
                Math.Max(0, pagingInfo.CurrentPage - (int)Math.Round((double)sizePatginatonBar/2)) -
                Math.Max(0,Math.Round((double)(pagingInfo.CurrentPage - (pagingInfo.TotalPages - sizePatginatonBar)) / 2)));

            int endIndex = Math.Min(pagingInfo.TotalPages, startIndex + sizePatginatonBar);

            #region Previos Page
                TagBuilder tag = new TagBuilder("a");

                tag.MergeAttribute("href", pagingInfo.CurrentPage > 1 ? 
                    pageUrl(pagingInfo.CurrentPage - 1) 
                    : pageUrl(1));

                tag.MergeAttribute("aria-label","Previous");
                tag.AddCssClass("btn btn-default");

                TagBuilder childtag = new TagBuilder("span");
                childtag.MergeAttribute("aria-hidden","true");
                childtag.InnerHtml = "&laquo";

                tag.InnerHtml = childtag.ToString();
                result.Append(tag);
            #endregion

            #region PageLinks
                for (int i = startIndex; i <= endIndex;i++) {

                TagBuilder pagetag = new TagBuilder("a");

                pagetag.MergeAttribute("href",pageUrl(i));
                pagetag.InnerHtml = i.ToString();

                if (i == pagingInfo.CurrentPage) {
                    pagetag.AddCssClass("selected");
                    pagetag.AddCssClass("btn-primary");
                }

                pagetag.AddCssClass("btn btn-default");

                result.Append(pagetag);
                }

            #endregion

            #region Next Page
                TagBuilder nexttag = new TagBuilder("a");

                nexttag.MergeAttribute("href",pagingInfo.CurrentPage < pagingInfo.TotalPages ?
                    pageUrl(pagingInfo.CurrentPage + 1)
                    : pageUrl(pagingInfo.TotalPages));

                nexttag.MergeAttribute("aria-label","Previous");
                nexttag.AddCssClass("btn btn-default");

                TagBuilder chilNexttagdtag = new TagBuilder("span");
                chilNexttagdtag.MergeAttribute("aria-hidden","true");
                chilNexttagdtag.InnerHtml = "&raquo";

                nexttag.InnerHtml = chilNexttagdtag.ToString();
                result.Append(nexttag);
            #endregion

            return MvcHtmlString.Create(result.ToString());
        }
    }
}