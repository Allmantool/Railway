using System;
using System.Globalization;
using System.Web.Mvc;

namespace NaftanRailway.WebUI.Infrastructure.ValueProvider {
    /*This value provider only responds to requests for values for the Country property and it always returns the value 
    USA. For all other requests, I return null, indicating that I cannot provide data*/
    public class CountryValueProvider : IValueProvider {

        public bool ContainsPrefix(string prefix) {
            return prefix.ToLower().IndexOf("country", StringComparison.Ordinal) > -1;
        }
        /*This class has three constructor parameters. The 
        first is the data item that I want to associate with the requested key. The second parameter is a version of the data 
        value that is safe to display as part of an HTML page. The final parameter is the culture information that relates to the 
        value; I have specified the InvariantCulture.*/
        public ValueProviderResult GetValue(string key) {
            if(this.ContainsPrefix(key)) {
                return new ValueProviderResult("USA", "USA",
                    CultureInfo.InvariantCulture);
            } else {
                return null;
            }
        }
    }
}