﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace lab2SERVER.Models
{
    public class SRLRequest
    {
        public List<SearchResultLine> Lines { get; set; }
    }
    public class SearchResultLine
    {
        public string ClimberName { get; set; }
        public string ClimberSurname { get; set; }
        public string Mountain { get; set; }
        public int Id { get; set; }
    }
}