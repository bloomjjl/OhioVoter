using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OhioVoter.ViewModels.RSS
{
    public class Feed
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
        public string Title { get; set; }
        public string Link { get; set; }
        public string Description { get; set; }

        public string Summary { get; set; }
        public string Language { get; set; }
        public DateTime PubDate { get; set; }
        public string Category { get; set; }
        public string Image { get; set; }
    }


}