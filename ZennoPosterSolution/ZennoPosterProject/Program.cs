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



            string gender = project.Variables["gender"].Value;

            Puppeteer page = new Puppeteer(instance, project);

            HtmlElement nextButton = page.WaitXpath("//button[@aria-label=\"Next\"]");
            page.Click(nextButton);

            // Pick gender 

            HtmlElement genderInput;

            genderInput = gender == "f"
                ? page.WaitXpath("//input[@name=\"genderOptions\"][@id=\"female\"]")
                : page.WaitXpath("//input[@name=\"genderOptions\"][@id=\"male\"]");

            page.Click(genderInput);

            // Pick Geo and Language

            HtmlElement submitButton = page.WaitXpath("//button[@type=\"submit\"]");
            page.Click(submitButton);

            //What are you interested in?

            int pinsPickersCount;
            int prevPinsPickerIndex = 0;
            string pinsPickerXPath = "//div[@data-test-id=\"nux-picker-topic\"]/div[@role=\"button\"]";

            page.WaitXpath(pinsPickerXPath);

            HtmlElement pinsPicker;
            HtmlElementCollection listOfPinsPickers = instance.ActiveTab.FindElementsByXPath(pinsPickerXPath);
            pinsPickersCount = listOfPinsPickers.Count();

            for (int i = 0; i < 5; i++)
            {
                int currentPinsPickerIndex = Global.Variables.MainRandom.GetNext(2, pinsPickersCount); 

                if (prevPinsPickerIndex == currentPinsPickerIndex)
                {
                    currentPinsPickerIndex--;
                }

                listOfPinsPickers.Elements[currentPinsPickerIndex].RiseEvent("click", instance.EmulationLevel);
                prevPinsPickerIndex = currentPinsPickerIndex;
            }

            submitButton = page.WaitXpath("//button[@type=\"submit\"]");
            page.Click(submitButton);





            return 0;
        }
    }
}