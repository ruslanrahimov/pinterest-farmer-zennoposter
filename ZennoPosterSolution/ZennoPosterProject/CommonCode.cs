using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ZennoLab.CommandCenter;
using ZennoLab.InterfacesLibrary.Enums.Log;
using ZennoLab.InterfacesLibrary.ProjectModel;

namespace ZennoPosterProject
{
    public class Log
    {
        private readonly IZennoPosterProjectModel project;

        public Log(IZennoPosterProjectModel project)
        {
            this.project = project;
        }

        public void Print(string message, string color = "Default")
        {
            LogColor logColor = (LogColor)Enum.Parse(typeof(LogColor), color, true);
            project.SendToLog(message, LogType.Info, true, logColor);

        }
    }

    public class Puppeteer
    {
        Instance instance;
        IZennoPosterProjectModel project;

        public Puppeteer(Instance instance, IZennoPosterProjectModel project)
        {
            this.instance = instance;
            this.project = project;
        }

        public HtmlElement WaitXpath(string xpath, int pause = 30000)
        {
            HtmlElement he = null;
            Tab tab = instance.ActiveTab;

            for (int i = 0; i < 8; i++)
            {
                if (i == 7)
                {
                    throw new Exception($"Не найден элемент: {xpath}");
                }

                he = tab.FindElementByXPath(xpath, 0);

                if (he.IsVoid)
                {
                    Thread.Sleep(pause / 6); //Делем на шесть что бы разделить время на 6 итераций
                    continue;
                }
                else
                {
                    break;
                }
            }

            return he;
        }

        public void Click(HtmlElement element)
        {
            // Задержка эмуляции
            instance.WaitFieldEmulationDelay();
            // Вызвать событие "click"
            element.RiseEvent("click", instance.EmulationLevel);
        }

        public void Type(HtmlElement element, string text)
        {
            // Задержка эмуляции
            instance.WaitFieldEmulationDelay();
            // Установить элементу значение
            element.SetValue(text, instance.EmulationLevel, false);
        }


        public void GoTo(string url)
        {
            Tab tab = instance.ActiveTab;
            if ((tab.IsNull) || (tab.IsVoid))
            {
                throw new Exception($"Неактивна вкладка браузера!");
            }
            if (tab.IsBusy) tab.WaitDownloading();
            tab.Navigate(url);
            if (tab.IsBusy) tab.WaitDownloading();
        }
    }

    public class CustomHttpRequset
    {
        public string MakeGetRequest(string url, string proxy = "")
        {
            return ZennoPoster.HTTP.Request(
                ZennoLab.InterfacesLibrary.Enums.Http.HttpMethod.GET,
                url: url,
                Encoding: "utf-8",
                proxy: proxy,
                respType: ZennoLab.InterfacesLibrary.Enums.Http.ResponceType.BodyOnly,
                Timeout: 30000
            );
        }

        public string MakePostRequest(string url, string data)
        {
            return ZennoPoster.HTTP.Request(
                    method: ZennoLab.InterfacesLibrary.Enums.Http.HttpMethod.POST,
                    url: url,
                    content: data,
                    contentPostingType: "application/json",
                    Encoding: "UTF-8",
                    respType: ZennoLab.InterfacesLibrary.Enums.Http.ResponceType.BodyOnly,
                    Timeout: 30000,
                    Cookies: null,
                    UseRedirect: true,
                    MaxRedirectCount: 5,
                    AdditionalHeaders: null,
                    UseOriginalUrl: false,
                    throwExceptionOnError: false,
                    removeDefaultHeaders: true
                );
        }
    }

    public class Ipify
    {
        private CustomHttpRequset httpRequest;

        public Ipify()
        {
            httpRequest = new CustomHttpRequset();
        }

        public string GetMyIP(string proxy)
        {
            string res = httpRequest.MakeGetRequest("https://www.myip.com/", proxy);
            string ip = Regex.Match(res, @"(?<=<span\ id=""ip"">).*?(?=</span>)").Value;
            return ip;
        }
    }
}
