using System.Linq;
using System;
using HtmlAgilityPack;
using ScrapySharp.Extensions;

static class Scraper
{

    public static void dooStuff()
    {
        var url = "https://tipidpc.com/catalog.php?cat=0&sec=s";
        var webGet = new HtmlWeb();
        if (webGet.Load(url) is HtmlDocument document)
        {
            var nodes = document.DocumentNode.CssSelect("#item-search-results li").ToList();
            foreach (var node in nodes)
            {
            Console.WriteLine("Selling: " + node.CssSelect("h2 a").Single().InnerText);
            }
        }
        Console.ReadLine();
    }
}