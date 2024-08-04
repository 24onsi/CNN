using ART.View;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ART.Model
{
    public class Check
    {
        public bool ID_Check { get; set; }
        public bool Login_Check { get; set; }
    }

    public class Data
    {
        public int Key { get; set; }
        public string? ID { get; set; }
        public string? Value { get; set; }

        public List<ArtistInfo>? ArtistList { get; set; }
        public List<User>? user { get; set; }

    }

    public class User
    {
        public string? PW { get; set; }
        public string? Name { get; set; }
        public string? PhoneNum { get; set; }
    }

    public class ArtistInfo
    {
        public int No { get; set; }
        public string? Artist { get; set; }
        public string? KorArtist { get; set; }
        public string? Years { get; set; }
        public string? Genre { get; set; }
        public string? Nationality { get; set; }
        public string? Url { get; set; }
    }

    public class Send_Email
    {
        public string? From { get; set; }
        public string? To { get; set; }
        public string? Content { get; set; }
        public string? Titile { get; set; }
    }

    enum KEY
    {
        FAIL = 0,

        ID_CHECK = 10,
        // 11
        JOIN,
        // 12
        LOGIN,
        // 13
        ARTIST,
        // 14
        RESULT,

        OK = 20,
        // 21
        NO,
        // 22
        ERR,
    }

}