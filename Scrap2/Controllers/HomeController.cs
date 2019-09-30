using HtmlAgilityPack;
using Scrap2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace Scrap2.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult Scrap()
        {
            //TestDB1Context
            TestDB1Entities context = new TestDB1Entities();
            for (int j = 217; j <= 217; j++)
            {

                context = new TestDB1Entities();

                HtmlWeb htmlWeb = new HtmlWeb();
                HtmlDocument doc = htmlWeb.Load("http://haryanacurrentgk.com/haryana-current-affairs-in-hindi-for-hssc/page/" + j.ToString());

                var result = doc.DocumentNode.SelectNodes("//div[contains(@class, 'col-xs-12')]");

                if (result == null)
                {
                }

                foreach (var item in result)
                {
                    int i = 0;
                    var innerData = item;
                    htmlWeb = new HtmlWeb();
                    var doc3 = new HtmlDocument();
                    doc3.LoadHtml(item.InnerHtml);

                    var innerNode = doc3.DocumentNode.SelectNodes("//div[contains(@style, 'no-repeat')]");
                    var innerNodeAns = doc3.DocumentNode.SelectNodes("//p[contains(@class, 'answer')]");

                    foreach (var questions in innerNode)
                    {
                        HaryanaGK haryanaGK = new HaryanaGK();
                        var doc2 = new HtmlDocument();
                        var docAns = new HtmlDocument();
                        doc2.LoadHtml(innerNode[i].InnerHtml);
                        doc2.LoadHtml(innerNodeAns[i].InnerHtml);

                        var ques = doc3.DocumentNode.SelectNodes("//strong")[i].InnerText;
                        var ans = doc2.DocumentNode.SelectNodes("//span")[1].InnerText;

                        if (ques != "")
                        {
                            haryanaGK.Question = ques;
                            haryanaGK.Answere = ans;
                            context.HaryanaGKs.Add(haryanaGK);
                        }
                        i = i + 1;

                    }
                }
                try
                {
                    context.SaveChanges();
                }
                catch (System.Exception e)
                {

                    j = j + 1;
                }




                //ques.



            }





            return View();
        }

        public ActionResult HaryanaGK()
        {
            TestDB1Entities context = new TestDB1Entities();
            var lstHaryanaGK = context.HaryanaGKs.ToList();
            return View(lstHaryanaGK);
        }


        public ActionResult GetIndiaBix()
        {
            TestDB1Entities context = new TestDB1Entities();

            HtmlWeb htmlWeb = new HtmlWeb();
            string baseUrl = "", subSectionUrl = "";
            baseUrl = "https://www.indiabix.com";
            subSectionUrl = "/aptitude/questions-and-answers/";
            HtmlDocument doc = htmlWeb.Load(baseUrl + subSectionUrl);
            var lstSubSections = doc.DocumentNode.SelectNodes("//div[contains(@class, 'div-topics-index')]");



            foreach (var item in lstSubSections)
            {
                IndiaBix indiaBix = new IndiaBix();

                indiaBix.SectionType = subSectionUrl.Split('/')[1];  //main section

                HtmlDocument subList = new HtmlDocument();
                subList.LoadHtml(item.InnerHtml);
                var liList = subList.DocumentNode.SelectNodes("//a").ToList();



                foreach (var liItem in liList)
                {
                    string subSectionURL = "https://www.indiabix.com" + liItem.Attributes["href"].Value;
                    indiaBix.SubSection = liItem.InnerText; //sub section

                    HtmlDocument subForPaging = htmlWeb.Load(subSectionURL);

                    var paging = subForPaging.DocumentNode.SelectNodes("//div[contains(@class, 'mx-pager-container')]").ToList()[0];

                    HtmlDocument Pages = new HtmlDocument();
                    Pages.LoadHtml(paging.InnerHtml);

                    var lstPages = Pages.DocumentNode.SelectNodes("//a").ToList();

                    //var firstPage = lstPages[1].Attributes[];
                    int lastCount = lstPages.Count() - 2;
                    var lastPage = lstPages[lastCount].Attributes["href"].Value.Split('/').LastOrDefault();

                    var firstPage = Convert.ToInt32(lastPage) - lastCount - 1;

                    int last = Convert.ToInt32(lastPage);
                    for (int i = firstPage; i <= last; i++)
                    {
                        HtmlDocument subSectionFetch = new HtmlDocument();
                        if (i == firstPage)
                        {
                            subSectionFetch = htmlWeb.Load(subSectionURL);
                        }
                        else
                        {
                            subSectionFetch = htmlWeb.Load(subSectionURL + "0" + i.ToString());
                        }

                        var lstSubSectionData = subSectionFetch.DocumentNode.SelectNodes("//table[contains(@class, 'bix-tbl-container')]").ToList();
                        HtmlDocument QueAns = new HtmlDocument();
                        HtmlDocument Q = new HtmlDocument();
                        HtmlDocument ans = new HtmlDocument();
                        HtmlDocument options = new HtmlDocument();

                        foreach (var itemQAns in lstSubSectionData)
                        {
                            QueAns.LoadHtml(itemQAns.InnerHtml);

                            //question
                            indiaBix.Question = QueAns.DocumentNode.SelectNodes("//td[contains(@class, 'bix-td-qtxt')]").ToList()[0].InnerText;

                            var lstOptionsHtml = QueAns.DocumentNode.SelectNodes("//td[contains(@width, '99%')]").ToList();

                            List<string> lstOptions = new List<string>();

                            foreach (var option in lstOptionsHtml)
                            {
                                lstOptions.Add(option.InnerText);
                            }

                            var jsonSerialiser = new JavaScriptSerializer();
                            var json = jsonSerialiser.Serialize(lstOptions);

                            indiaBix.Options = json; //options

                            indiaBix.Answer = QueAns.DocumentNode.SelectNodes("//span[contains(@class, 'jq-hdnakqb mx-bold')]").ToList()[0].InnerText;  //answer

                            indiaBix.AnswerDiv = QueAns.DocumentNode.SelectNodes("//div[contains(@class, 'bix-ans-description')]").ToList()[0].InnerText; //answer div

                            context.IndiaBixes.Add(indiaBix);
                        }
                        context.SaveChanges();
                    }
                }
            }
            return View();
        }


    }
}