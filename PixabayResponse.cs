using System;
using System.Collections.Generic;
using System.Text;

namespace SnakeWpf
{
    public class PixabayResponse
    {
        public string total { get; set; }
        public string totalHits { get; set; }
        public List<Hit> hits = new List<Hit>();
    }

    public class Hit
    {
        public string id { get; set; }
        public string previewURL { get; set; }
        public string webformatWidth { get; set; }
    }
}