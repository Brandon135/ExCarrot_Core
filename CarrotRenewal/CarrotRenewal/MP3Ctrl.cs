using MediaToolkit;
using MediaToolkit.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using VideoLibrary;

namespace SitubeYoubal
{
    class VisualStat
    {
        public  string OutputRoot = @"C:\Users\fun67\Desktop\Youtubedata\";
        public  string FolderRoot = null;
        private string Statitle;
        public string TotslDown(string url)
        {
            try
            {
                string a = url;
                var source = @"C:\Users\fun67\Desktop\Youtubedata\";
                var youtube = YouTube.Default;
                var vid = youtube.GetVideo(a);
                File.WriteAllBytes(source + vid.FullName, vid.GetBytes());

                var inputFile = new MediaFile { Filename = source + vid.FullName };
                string strtitle = source + vid.FullName;
                strtitle = strtitle.Replace(" - YouTube.mp4", "");
                var outputFile = new MediaFile { Filename = $"{source + strtitle}.mp3" };
           
            using (var engine = new Engine())
            {
                engine.GetMetadata(inputFile);

                engine.Convert(inputFile, outputFile);
            }
            return OutputRoot + strtitle + ".mp3";
            }
            catch
            {
                return "에러 발생";
            }
        }   
   
    }
}