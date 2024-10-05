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
            string boardsCreated = project.Variables["boardsCreated"].Value;

            if (boardsCreated != "True")
            {
                Log log = new Log(project);
                Puppeteer page = new Puppeteer(instance, project);
                List<string> boardsList = project.Lists["boards"].ToList<string>();


                int boardsCount = int.Parse(project.Variables["boardsCount"].Value);
                int randomIndexOfBoard;

                string boardName;
                string threadid = project.Variables["threadid"].Value;

                page.GoTo("https://www.pinterest.com");

                HtmlElement profileLink = page.WaitXpath("//div[@data-test-id=\"header-profile\"]//a");
                page.Click(profileLink);

                HtmlElement actionsButton;
                HtmlElement createNewBoardButton;
                HtmlElement boardNameInput;
                HtmlElement submitButton;
                HtmlElement doneButton;

                for (int i = 0; i < boardsCount; i++)
                {
                    // Создаем доски
                    randomIndexOfBoard = Global.Variables.MainRandom.GetNext(0, boardsList.Count);
                    boardName = boardsList[randomIndexOfBoard];

                    try
                    {
                        actionsButton = page.WaitXpath("//div[@data-test-id=\"boardActionsButton\"]//button");
                        page.Click(actionsButton);

                        createNewBoardButton = page.WaitXpath("//div[@data-test-id=\"Create board\"]");
                        page.Click(createNewBoardButton);

                        boardNameInput = page.WaitXpath("//form//input[@type=\"text\"]");
                        page.Type(boardNameInput, boardName);

                        submitButton = page.WaitXpath("//button[@type=\"submit\"][not(@disabled)]");
                        page.Click(submitButton);

                        doneButton = page.WaitXpath("//div[@data-test-id=\"done-button\"]/button", 10000);
                        page.Click(doneButton);

                        profileLink = page.WaitXpath("//div[@data-test-id=\"header-profile\"]//a");
                        page.Click(profileLink);

                    }
                    catch
                    {
                        profileLink = page.WaitXpath("//div[@data-test-id=\"header-profile\"]//a");
                        page.Click(profileLink);
                    }
                    finally
                    {
                        boardsList = boardsList.Where<string>(x => boardName != x).ToList<string>();
                    }
                   
                }

                project.Variables["boardsCreated"].Value = "True";

            }






            return 0;
        }
    }
}