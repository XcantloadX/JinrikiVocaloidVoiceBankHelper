using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using JinrikiVocaloidVBHelper.Core;
using JinrikiVocaloidVBHelper.Util;
using System.Text.RegularExpressions;
using AuditionHelper.Core;
using Newtonsoft.Json;
using JinrikiVocaloidVBHelper.Audition;
using System.IO;

namespace AuditionHelper.Audition
{
    /// <summary>
    /// 基于 ExtendScript 来操作 Audition
    /// </summary>
    public class AuditionExtendScriptController : AuditionController
    {
        private HttpListener httpListener;
        private Thread listeningThread;
        //要执行的 es 代码
        private Queue<string> ESQueue = new Queue<string>(10);
        private bool running = true;

        public AuditionExtendScriptController()
        {
            httpListener = new HttpListener();
            httpListener.Prefixes.Add("http://localhost:2233/");
            httpListener.Start();
            listeningThread = new Thread(Listening);
            listeningThread.Start();
        }

        private void Listening()
        {
            while(running)
            {
                //等待请求
                try
                {
                    HttpListenerContext ctx = httpListener.GetContext();
                    HttpListenerRequest req = ctx.Request;
                    HttpListenerResponse res = ctx.Response;
                    string nextCommand = "";
                    ResponseData data = new ResponseData();

                    if (req.Url.AbsolutePath == "/getNextCommand")
                    {
                        if (ESQueue.Any())
                        {
                            while (ESQueue.Any())
                            {
                                nextCommand += ESQueue.Dequeue().EscapeSplash() + ";";
                            }
                            nextCommand = string.Format("csInterface.evalScript('{0}');", nextCommand.EscapeSplash());
                        }


                        data.message = "ok";
                        data.command = nextCommand;
                    }
                    else
                    {
                        data.message = "invaild action";
                        data.command = null;
                        res.StatusCode = 400;
                    }

                    byte[] buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));
                    res.ContentType = "application/json";
                    res.ContentEncoding = Encoding.UTF8;
                    res.ContentLength64 = buffer.LongLength;
                    res.OutputStream.Write(buffer, 0, buffer.Length);
                    res.OutputStream.Close();
                }
                catch (Exception e)
                {
                    if (!running)
                        return;
                    else
                        throw e;
                }
                

            }
        }

        public override void Dispose()
        {
            httpListener.Stop();
            running = false;
            
        }

        /// <summary>
        /// 执行 ExtendScript 代码
        /// </summary>
        /// <param name="expression"></param>
        public void EvalES(string expression)
        {
            ESQueue.Enqueue(expression);
        }

        public override void OpenFile(string path)
        {
            EvalES(string.Format("openFile(\"{0}\")", path.EscapeSplash()));
        }

        public override void Select(string startTime, string endTime)
        {
            Select2(TimeConvert.SrtTime2Sec(startTime), TimeConvert.SrtTime2Sec(endTime));
        }

        public override void Select2(double startTime, double endTime)
        {
            EvalES(string.Format("select({0}, {1})", startTime, endTime));
        }

        public override void Seek(string time)
        {
            Seek2(TimeConvert.SrtTime2Sec(time));
        }

        public override void Seek2(double time)
        {
            EvalES(string.Format("seek({0})", time));
        }

        public override void SaveSelection(string fileName, string filePath)
        {
            string tempPath = Path.GetFullPath("temp\\saveSelectionTemp.wav");
            if (!Directory.Exists("temp"))
                Directory.CreateDirectory("temp");
            EvalES(string.Format("saveSelection(\"{0}\")", tempPath));
            while (!System.IO.File.Exists(tempPath)) { } //等待命令执行
            System.Diagnostics.Process p =System.Diagnostics.Process.Start("tools\\ffmpeg.exe", string.Format("-i {0} -ac 48000 -acodec pcm_s16le {1}", tempPath, System.IO.Path.Combine(filePath.EscapeSplash(), fileName + ".wav")));
            p.WaitForExit();
            System.IO.File.Delete(tempPath);
        }

        private class ResponseData
        {
            public string message;
            public string command;
        }

    }

    public static class StringExtesion
    {
        /// <summary>
        /// 给斜杠（\）转义
        /// </summary>
        /// <param name="str"></param>
        /// <returns>返回转义后的字符串，如 \ ---> \\</returns>
        public static string EscapeSplash(this string str)
        {
            return str.Replace(@"\", @"\\");
        }
    }
}
