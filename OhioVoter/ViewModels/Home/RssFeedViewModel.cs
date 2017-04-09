using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OhioVoter.ViewModels.Home
{
    public class RssFeedViewModel
    {

        public Channel Channel { get; set; }
        public IEnumerable<Item> Items { get; set; }
    }



    public class Channel
    {
        public Element Element { get; set; }
    }



    public class Item
    {
        public Element Element { get; set; }
    }



    public class Element
    {
        public string Description { get; set; }
        public string Link_0 { get; set; }
        public string Link_1 { get; set; }
        public string Link_2 { get; set; }
        public string Title { get; set; }

        public string Category { get; set; }
        public string Id { get; set; }
        public string Image { get; set; }
        public string Language { get; set; }
        public string PubDate { get; set; }
        public string Summary { get; set; }
    }


}