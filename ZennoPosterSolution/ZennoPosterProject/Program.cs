﻿using Global.ZennoExtensions;
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
            

            string accountType = project.Variables["accountType"].Value;

            if(accountType == "business")
            {
                Puppeteer page = new Puppeteer(instance, project);
                Log log = new Log(project);

                // Получаем и задаем параметры загружаемого файла

                int randomFileIndex;
                string threadid = project.Variables["threadid"].Value;

                string coverImagesDirPath = project.Variables["coverImagesDirPath"].Value;

                string[] coverImages = Directory.GetFiles(coverImagesDirPath, "*.*", SearchOption.AllDirectories);

                if (coverImages.Length == 0)
                {
                    log.Print($"{threadid}: Папка с фотографиями пуста! Пропускаю данное действие.", "red"); // Реализовать скачивание фотки со стороннего сервиса
                    return true;
                }

                randomFileIndex = Global.Variables.MainRandom.GetNext(0, coverImages.Length);

                string imageFilePath = coverImages[randomFileIndex];

                instance.SetFileUploadPolicy("ok", "");
                instance.SetFilesForUpload(imageFilePath);



                HtmlElement profileLink = page.WaitXpath("//div[@data-test-id=\"header-profile\"]//a");
                page.Click(profileLink);

                HtmlElement editCoverButton = page.WaitXpath("//div[@data-test-id=\"profile-cover-edit-button\"]/button");
                page.Click(editCoverButton);

                HtmlElement browse = page.WaitXpath("//input[@id=\"asset-picker-upload\"]");
                page.Click(browse);

                HtmlElement doneButton = page.WaitXpath("//div[text()=\"Done\"]/../..");
                page.Click(doneButton);
            }

            return 0;
        }
    }
}