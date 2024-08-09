using Microsoft.Playwright;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace Cosmos_voucher
{
    public class Tests
    {
        private IPage Page;
        private IBrowserContext Context;
        private IBrowser Browser;

        [SetUp]
        public async Task Setup()
        {
            // Initialize Playwright
            var playwright = await Playwright.CreateAsync();
            Browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });

            // Create a new browser context
            Context = await Browser.NewContextAsync();
        }

        [Test]
        public async Task Login()
        {
            // Create a new page from the context
            Page = await Context.NewPageAsync();

            // Navigate to the login page
            await Page.GotoAsync("https://cosmos.utopiadeals.com/cosmos/login");
            // Perform login actions
            await Write("#cosmos_username", "mujahid.akber", "Enter Username");
            await Write("#cosmos_password", "Mujahid12345", "Enter Password");
            await Click("body > div > form > div:nth-child(8) > button", "Click Login");
            await Page.WaitForSelectorAsync("body > nav > div > ul > li:nth-child(1) > a > div > span");
            await Page.Locator("body > nav > div > ul > li:nth-child(1) > a > div > span").HoverAsync();
            await Page.Locator("body > nav > div > ul > li:nth-child(1) > ul > li:nth-child(18) > a").HoverAsync();
            await Click("body > nav > div > ul > li:nth-child(1) > ul > li:nth-child(18) > ul > li:nth-child(4) > a", "Press Vouchers");
            Thread.Sleep(6000);

            await Context.StorageStateAsync(new()
            {
                Path = "C:\\Users\\DELL\\source\\repos\\Cosmos_voucher\\Cosmos_voucher\\state.json"
            });
        }


        private static IEnumerable<TestCaseData> TestData()
        {
            string csvFilePath = @"C:\Users\DELL\source\repos\Cosmos_voucher\Cosmos_voucher\Data_voucher.csv";
            var lines = File.ReadAllLines(csvFilePath).Skip(1); // Skip header line
            
            foreach (var line in lines)
            {
                var values = line.Split(',');
                string Voucher_Date = values[0];
                string Voucher_Type = values[1];
                string Company_Title = values[2];
                string Instrument_Number = values[3];
                string Description = values[4];
                string Notes = values[5];
                string Marketplace = values[6];
                string Currency = values[7];
                string Exchange_Rate = values[8];
                string Type = values[9];
                string Type1 = values[10];
                string Amount = values[11];
                string Account_Debit = values[12];
                string Account_Credit = values[13];

                yield return new TestCaseData(Voucher_Date, Voucher_Type, Company_Title, Instrument_Number, Description, Notes,Marketplace, Currency, Exchange_Rate, Type, Type1, Amount, Account_Debit, Account_Credit);
            }
        }


        [Test]
        [TestCaseSource(nameof(TestData))]
        public async Task Voucher(string Voucher_Date, string Voucher_Type, string Company_Title, string Instrument_Number, string Description, string Notes, string Marketplace, string Currency, string Exchange_Rate, string Type, string Type1, string Amount, string Account_Debit, string Account_Credit)
        {
            // Load the previously saved context state from a file
            Context = await Browser.NewContextAsync(new BrowserNewContextOptions
            {
                StorageStatePath = "C:\\Users\\DELL\\source\\repos\\Cosmos_voucher\\Cosmos_voucher\\state.json"
            });

            // Perform actions after loading the saved stat
            await Click("body > div:nth-child(3) > div > div.col-md-2 > div > button", "Press Add Vouchers");
            await Write("#voucherDate", Voucher_Date, "Enter Voucher date");
            await Page.Locator("xpath=//html/body/div[4]/div[2]/div/form/fieldset[1]/div/div[2]/div[1]/div/select").SelectOptionAsync(Voucher_Type);
            await Page.Locator("xpath=//html/body/div[4]/div[2]/div/form/fieldset[1]/div/div[1]/div[2]/div/select").SelectOptionAsync(Company_Title);

            await Write("#instrumentNumber", Instrument_Number, "Enter Instrument number");
            await Write("#frm-add-broker > fieldset:nth-child(4) > div > div:nth-child(1) > div:nth-child(4) > div > textarea", Notes, "Enter Notes");
            await Page.Locator("xpath=//html/body/div[4]/div[2]/div/form/fieldset[1]/div/div[2]/div[4]/div/select").SelectOptionAsync(Marketplace);
            await Page.Locator("xpath=//html/body/div[4]/div[2]/div/form/fieldset[2]/div/div[1]/div/div/select").SelectOptionAsync(Currency);
            await Write("#currencyConversionRate", Exchange_Rate, "Enter Exchange rate");


            await Click("#frm-add-broker > fieldset:nth-child(8) > div:nth-child(4) > div > div > button", "Press Add Transaction");
            await Write("#transaction-legs-container > div > div:nth-child(2) > input", Amount, "Enter Amount");

            await Page.FillAsync($"input[placeholder='Account']", Account_Debit);
            Thread.Sleep(4000);
            await Press($"input[placeholder='Account']", "ArrowDown", "arrowdown");
            await Press($"input[placeholder='Account']", "Enter", "arrowdown");

            await Write("#transaction-legs-container > div > div:nth-child(5) > input", Description, "Enter Description");


            await Click("#frm-add-broker > fieldset:nth-child(8) > div:nth-child(4) > div > div > button", "Press Add Transaction");

            await Page.Locator("#transaction-legs-container > div:nth-child(2) > div:nth-child(1) > select").SelectOptionAsync(Type1);

            await Write("#transaction-legs-container > div:nth-child(2) > div:nth-child(2) > input", Amount, "Enter Amount");

            await Page.FillAsync("xpath=//html/body/div[4]/div[2]/div/form/fieldset[3]/div[2]/div[2]/div[4]/input[1]", Account_Credit);
            Thread.Sleep(4000);
            await Press("xpath=//html/body/div[4]/div[2]/div/form/fieldset[3]/div[2]/div[2]/div[4]/input[1]", "ArrowDown", "arrowdown");
            await Press("xpath=//html/body/div[4]/div[2]/div/form/fieldset[3]/div[2]/div[2]/div[4]/input[1]", "Enter", "arrowdown");


            await Write("#transaction-legs-container > div:nth-child(2) > div:nth-child(5) > input", Description, "Enter Description");

            await Click("#frm-add-broker > button", "Press Submit");

            Thread.Sleep(6000);
            

            // Optionally, add a delay before the test finishes
            await Task.Delay(6000); // Replaces Thread.Sleep with Task.Delay
        }

        public async Task Click(string locator, string detail)
        {
            try
            {
                await Page.ClickAsync(locator);
                //await ExtentReport.TakeScreenshot(Page, Status.Pass, detail);
                await Task.Delay(1000); // Replaces Thread.Sleep with Task.Delay
            }
            catch (Exception ex)
            {
                //await ExtentReport.TakeScreenshot(Page, Status.Fail, "Click Failed" + ex);
                throw; // Re-throw the exception to catch errors
            }
        }

        public async Task Write(string locator, string data, string detail)
        {
            try
            {
                await Page.FillAsync(locator, data);
                //await ExtentReport.TakeScreenshot(Page, Status.Pass, detail);
                await Task.Delay(1000); // Replaces Thread.Sleep with Task.Delay
            }
            catch (Exception ex)
            {
                //await ExtentReport.TakeScreenshot(Page, Status.Fail, "Entry Failed" + ex);
                throw; // Re-throw the exception to catch errors
            }
        }

        public async Task Press(string locator, string data, string detail)
        {
            try
            {
                await Page.PressAsync(locator, data);
                //await ExtentReport.TakeScreenshot(Page, Status.Pass, detail);
                await Task.Delay(1000); // Replaces Thread.Sleep with Task.Delay
            }
            catch (Exception ex)
            {
                //await ExtentReport.TakeScreenshot(Page, Status.Fail, "Entry Failed" + ex);
                throw; // Re-throw the exception to catch errors
            }
        }
    }
}
