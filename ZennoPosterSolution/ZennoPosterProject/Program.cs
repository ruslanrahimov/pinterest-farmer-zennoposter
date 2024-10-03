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
            //"accountType":"influencer_public_figure_or_celebrity"

            //TODO добавить проверку под бизнес акк

            Log log = new Log(project);
            Puppeteer page = new Puppeteer(instance, project);

            string threadid = project.Variables["threadid"].Value;

            if (page.IsDomTextExists("\"name\":\"UserSettingsResource\"") || page.IsDomTextExists("\"accountType\":\"influencer_public_figure_or_celebrity\""))
            {
                log.Print($"{threadid}: Проверка на чек пройдена.", "yellow");
                return true; //TODO replace 1 with true;
            }

            log.Print($"{threadid}: Пробую авторизоваться...");

            string login = project.Variables["login"].Value;
            string password = project.Variables["password"].Value;

            HtmlElement loginButton = page.WaitXpath("//div[@data-test-id=\"simple-login-button\"]/button");
            page.Click(loginButton);

            HtmlElement emailInput = page.WaitXpath("//input[@id=\"email\"]");
            HtmlElement passwordInput = page.WaitXpath("//input[@id=\"password\"]");
            HtmlElement submitButton = page.WaitXpath("//button[@type=\"submit\"]");

            page.Type(emailInput, login);
            Thread.Sleep(1000);
            page.Type(passwordInput, password);
            Thread.Sleep(1000);
            page.Click(submitButton);

            try
            {
                page.WaitXpath("//div[@role=\"alertdialog\"]", 20000);
                log.Print($"{threadid}: Аккаунт забанен.", "red");
                project.Variables["ban"].Value = "True";
            }
            catch
            {
                log.Print($"{threadid}: Проверка на чек пройдена.", "yellow");
                return true;
            }




            return 0;
        }
    }
}