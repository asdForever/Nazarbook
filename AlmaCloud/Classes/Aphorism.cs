using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AlmaCloud.Classes
{
    public static class Book
    {
        public static int currentAphorism;
        public static int currentChapter;

        public static int langID;
        public static string bookTitle;
        public static List<Aphorism> aphorismList = new List<Aphorism>();
        public static List<Chapter> chapterList = new List<Chapter>();
    }

    public class Aphorism
    {
        public int aphID;
        public int chapterID;
        public List<string> textList = new List<string>();
        public bool isLiked;
        public bool isSharedT;
        public bool isSharedF;
    }

    public class Chapter
    {
        public int chapterID;
        public List<string> titleList = new List<string>();

        public Chapter(int ID, string kaztext, string rustext, string engtext)
        {
            this.chapterID = ID;
            titleList = new List<string>
            {
                kaztext, rustext, engtext
            };
        }
    }
}
