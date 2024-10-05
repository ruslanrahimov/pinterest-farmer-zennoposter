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
using System.Threading;
using System.Security.Cryptography;

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
            string businessVertical = project.Variables["businessVertical"].Value;
            HtmlElement dropdownButton = page.WaitXpath("//input[@id=\"combobox-tags\"]");

            for (int i = 0; i < 3; i++)
            {
                page.Click(dropdownButton);
                page.WaitXpath("//div[@role=\"option\"]");
                HtmlElementCollection goalOptions = instance.ActiveTab.FindElementsByXPath("//div[@role=\"option\"]");
                int indexOfOption = Global.Variables.MainRandom.GetNext(1, goalOptions.Count + 1);
                HtmlElement option = page.WaitXpath($"(//div[@role=\"option\"])[{indexOfOption}]");
                page.Click(option);
                Thread.Sleep(2000);
            }

            HtmlElement selectVertical = page.WaitXpath("//select[@id=\"verticals\"]");
            selectVertical.SetValue(ZennoPoster.Parser.ParseByXpath(selectVertical.OuterHtml, ".//option", "OuterHtml").ToList().FindIndex(x => x.Contains($@"value=""{businessVertical}""")).ToString(), "None", false);
            selectVertical.RiseEvent("onchange", "Full");

            HtmlElement doneButton = page.WaitXpath("//div[@data-test-id=\"biz-nux-done-button\"]/button[not(@disabled)]");
            page.Click(doneButton);





            return 0;
        }
    }
}