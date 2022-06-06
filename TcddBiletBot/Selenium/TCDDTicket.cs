using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TcddBiletBot.Model;
using TcddBiletBot.TelegramBot;

namespace TcddBiletBot.Selenium
{
    public class TCDDTicket
    {
        public async Task Search(Bilet model)
        {

            string url = "https://ebilet.tcddtasimacilik.gov.tr/view/eybis/tnmGenel/tcddWebContent.jsf";
            ChromeOptions CHoptions = new ChromeOptions();
            //CHoptions.AddArgument("--headless");
            IWebDriver driver = new ChromeDriver(CHoptions);
            try
            {
                driver.Manage().Window.Maximize();
                driver.Navigate().GoToUrl(url);

                Thread.Sleep(3000);

                IWebElement btnGidisDonus = driver.FindElement(By.XPath("/html/body/div[3]/div[2]/div/div[2]/ul/li[1]/div/form/div[2]/table/tbody/tr/td[3]/div/div[2]"));
                btnGidisDonus.Click();

                Thread.Sleep(1000);

                IWebElement nereden = driver.FindElement(By.Name("nereden"));
                nereden.SendKeys(model.Sefer.Kalkis);

                Thread.Sleep(1000);

                IWebElement nereye = driver.FindElement(By.Name("nereye"));
                nereye.SendKeys(model.Sefer.Varis);

                Thread.Sleep(1000);

                IWebElement trCalGid_input = driver.FindElement(By.Name("trCalGid_input"));
                trCalGid_input.Clear();
                trCalGid_input.SendKeys(model.Sefer.GidisTarihi);

                Thread.Sleep(1000);

                IWebElement trCalDon_input = driver.FindElement(By.Name("trCalDon_input"));
                trCalDon_input.Clear();
                trCalDon_input.SendKeys(model.Sefer.DonusTarihi);

                Thread.Sleep(1000);
                IWebElement btn = driver.FindElement(By.Name("btnSeferSorgula"));
                btn.Click();
                Thread.Sleep(7000);

                #region GidisBiletiBul
                string gidisMesaj = "";

                IWebElement tableElement = driver.FindElement(By.XPath("/html/body/div[3]/div[2]/div/div/div/div/form/div[1]/div/div[1]/div/div/div/div[1]/div/div/div/table/tbody"));
                var satir = tableElement.FindElements(By.TagName("tr"));
                int satirSayisi = satir.Count();
                for (int i = 0; i < satirSayisi; i++)
                {
                    string seferSaati = driver.FindElement(By.XPath("/html/body/div[3]/div[2]/div/div/div/div/form/div[1]/div/div[1]/div/div/div/div[1]/div/div/div/table/tbody/tr[" + (i + 1) + "]/td[1]/span")).Text;

                    if ((DateTime.Parse(seferSaati) > DateTime.Parse(model.Sefer.MinGidisSaat)) && (DateTime.Parse(seferSaati) < DateTime.Parse(model.Sefer.MaxGidisSaat)))
                    {
                        string id = "mainTabView:gidisSeferTablosu:" + i.ToString() + ":j_idt109:0:somVagonTipiGidis1_input";
                        var options = driver.FindElements(By.Id(id));

                        var optionValues = options.Select(elem => elem.GetAttribute("innerText")).ToList();

                        string[] splitText = optionValues[0].Split(")");
                        int ekonomiKoltukSayisi = Convert.ToInt32(splitText[1].Remove(1, 1));
                        int businessKoltukSayisi = Convert.ToInt32(splitText[3].Remove(1, 1));

                        if (ekonomiKoltukSayisi > 2 || businessKoltukSayisi > 0)
                        {
                          

                            gidisMesaj = gidisMesaj + model.Sefer.GidisTarihi + " Tarihinde " + model.Sefer.Kalkis + " Yönünden " + model.Sefer.Varis + " Yönüne Saat: " + seferSaati + "'da boş koltuk var." + Environment.NewLine + Environment.NewLine;
                        }
                    }
                }

                if (gidisMesaj == "")
                {
                    gidisMesaj = "Gidiş Bileti Bulunamadı!";
                }
             
                #endregion

                #region DonusBiletiBul
                string donusMesaj = "";

                IWebElement tableElementDonus = driver.FindElement(By.XPath("/html/body/div[3]/div[2]/div/div/div/div/form/div[1]/div/div[1]/div/div/div/div[2]/div/div/div/table/tbody"));
                var satirDonus = tableElementDonus.FindElements(By.TagName("tr"));
                int satirSayisiDonus = satirDonus.Count();

                for (int i = 0; i < satirSayisiDonus; i++)
                {
                    string seferSaati = driver.FindElement(By.XPath("/html/body/div[3]/div[2]/div/div/div/div/form/div[1]/div/div[1]/div/div/div/div[2]/div/div/div/table/tbody/tr[" + (i + 1) + "]/td[1]/span")).Text;

                    if ((DateTime.Parse(seferSaati) > DateTime.Parse(model.Sefer.MinGidisSaat)) && (DateTime.Parse(seferSaati) < DateTime.Parse(model.Sefer.MaxDonusSaat)))
                    {
                        string id = "mainTabView:donusSeferTablosu:" + i.ToString() + ":j_idt175:0:somVagonTipiGidis1_input";
                        var options = driver.FindElements(By.Id(id));

                        var optionValues = options.Select(elem => elem.GetAttribute("innerText")).ToList();

                        string[] splitText = optionValues[0].Split(")");
                        int ekonomiKoltukSayisi = Convert.ToInt32(splitText[1].Remove(1, 1));
                        int businessKoltukSayisi = Convert.ToInt32(splitText[3].Remove(1, 1));

                        if (ekonomiKoltukSayisi > 2 || businessKoltukSayisi > 0)
                        {
                       
                            donusMesaj = donusMesaj + model.Sefer.DonusTarihi + " Tarihinde " + model.Sefer.Varis + " Yönünden " + model.Sefer.Kalkis + " Yönüne Saat: " + seferSaati + "'da boş koltuk var." + Environment.NewLine + Environment.NewLine;
                        }
                    }
                }

                if (donusMesaj == "")
                {
                    donusMesaj = "Dönüş Bileti Bulunamadı!";
                }
                #endregion

                Send send = new Send();
                await send.Message(model.Client, model.Update, gidisMesaj + Environment.NewLine + "***" + Environment.NewLine + donusMesaj);

                driver.Close();
                driver.Quit();

            }
            catch (Exception ex)
            {
                Console.WriteLine("Hata => " + ex.Message);
                driver.Close();
                driver.Quit();
                await Search(model);
            }
        }
    }
}
