using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Xunit;
using Xunit.Abstractions;

namespace JsonExcelExpressions
{
    public class Web : BaseTest
    {
        public Web(ITestOutputHelper output) : base(output) { }

        [Fact]
        public void ENCODEURL()
        {
            AssertExpression("=ENCODEURL(\"http://contoso.sharepoint.com/teams/Finance/Documents/April Reports/Profit and Loss Statement.xlsx\")",
                "http%3A%2F%2Fcontoso.sharepoint.com%2Fteams%2FFinance%2FDocuments%2FApril%20Reports%2FProfit%20and%20Loss%20Statement.xlsx");
        }

        [Fact]
        public void WEBSERVICE_JSON()
        {
            var expression = "=WEBSERVICE(\"https://en.wikipedia.org/w/api.php?action=query&list=recentchanges&format=json\")";
            var result = processor.Evaluate(expression);

            Assert.Null(result.Error);
            var json = JObject.Parse(result.Text);
            Assert.NotNull(json["query"]["recentchanges"][0]["title"]);
        }

        [Fact]
        public void WEBSERVICE_XML()
        {
            var url = "https://en.wikipedia.org/w/api.php?action=query&list=recentchanges&rcnamespace=0&format=xml";
            var expression = $"=WEBSERVICE(\"{url}\")";
            var result = processor.Evaluate(expression);

            Assert.Null(result.Error);
            var xml = new XmlDocument();
            xml.LoadXml(result.Text);
            Assert.NotEmpty(xml.SelectNodes("//rc/@title"));
        }

        [Fact]
        public void FILTERXML()
        {
            var xml = "<?xml version='1.0'?><api batchcomplete=''><continue rccontinue='20200516134912|1261857306' continue='-||' /><query><recentchanges><rc type='edit' ns='0' title='2015 Swindon Borough Council election' pageid='46323119' revid='956999953' old_revid='956999165' rcid='1261857312' timestamp='2020-05-16T13:49:22Z' /><rc type='edit' ns='0' title='St Michael&#039;s Church, Rudbaxton' pageid='63973890' revid='956999951' old_revid='956999445' rcid='1261857315' timestamp='2020-05-16T13:49:21Z' /><rc type='edit' ns='0' title='Newstead Abbey' pageid='145954' revid='956999950' old_revid='954722343' rcid='1261857311' timestamp='2020-05-16T13:49:20Z' /><rc type='edit' ns='0' title='137th Street–City College station' pageid='1952711' revid='956999949' old_revid='956783543' rcid='1261857310' timestamp='2020-05-16T13:49:16Z' /><rc type='edit' ns='0' title='Domain name' pageid='39878' revid='956999948' old_revid='956999856' rcid='1261857304' timestamp='2020-05-16T13:49:16Z' /><rc type='edit' ns='0' title='Mónica Ojeda' pageid='60393174' revid='956999947' old_revid='934861408' rcid='1261857303' timestamp='2020-05-16T13:49:15Z' /><rc type='edit' ns='0' title='Otto von Botenlauben' pageid='7615316' revid='956999945' old_revid='956999766' rcid='1261857300' timestamp='2020-05-16T13:49:15Z' /><rc type='edit' ns='0' title='Maude Petre' pageid='10532383' revid='956999944' old_revid='956999907' rcid='1261857296' timestamp='2020-05-16T13:49:15Z' /><rc type='edit' ns='0' title='Topmodell (Hungarian TV series)' pageid='8568695' revid='956999941' old_revid='956999815' rcid='1261857299' timestamp='2020-05-16T13:49:14Z' /><rc type='edit' ns='0' title='SM City Baguio' pageid='13016753' revid='956999940' old_revid='948610670' rcid='1261857297' timestamp='2020-05-16T13:49:13Z' /></recentchanges></query></api>";
            var xpath = "//rc/@title";
            var expression1 = $"=FILTERXML(\"{xml}\", \"{xpath}\")";
            var result1 = processor.Evaluate(expression1);
            Assert.Null(result1.Error);
            Assert.Equal("2015 Swindon Borough Council election", result1.Text);
        
            var expression2 = $"=FILTERXML(\"<?xml version='1.0'?>\", \"{xpath}\")";
            var result2 = processor.Evaluate(expression2);
            Assert.Null(result2.Error);
            Assert.Equal("#VALUE!", result2.Text);

            var expression3 = $"=FILTERXML(\"{xml}\", \"{xpath}xxx\")";
            var result3 = processor.Evaluate(expression3);
            Assert.Null(result3.Error);
            Assert.Equal("#VALUE!", result3.Text);
        }
    }
}
