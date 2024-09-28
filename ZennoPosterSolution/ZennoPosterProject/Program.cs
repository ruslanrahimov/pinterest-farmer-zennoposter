using Global.ZennoExtensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.IO.Compression;
using System.Xml;
using System.Linq;
using System.Net.Sockets;
using System.Resources;
using System.Text;
using ZennoLab.CommandCenter;
using ZennoLab.Emulation;
using ZennoLab.InterfacesLibrary.ProjectModel;
using ZennoLab.InterfacesLibrary.ProjectModel.Enums;


namespace ZennoPosterProject
{
    /// <summary>
    /// Класс для запуска выполнения скрипта
    /// </summary>
    public class Program : IZennoExternalCode
    {
        /// <summary>
        /// Метод для запуска выполнения скрипта
        /// </summary>
        /// <param name="instance">Объект инстанса выделеный для данного скрипта</param>
        /// <param name="project">Объект проекта выделеный для данного скрипта</param>
        /// <returns>Код выполнения скрипта</returns>		
        public int Execute(Instance instance, IZennoPosterProjectModel project)
        {
            Puppeteer page = new Puppeteer(instance, project);

            IZennoList aboutsList = project.Lists["abouts"];

            int randomAboutIndex = Global.Variables.MainRandom.GetNext(0, aboutsList.Count);
            string about = aboutsList[randomAboutIndex];

            HtmlElement textArea = page.WaitXpath("//textarea[@id=\"about\"]");

            page.Type(textArea, about);




            return 0;
        }
    }
}