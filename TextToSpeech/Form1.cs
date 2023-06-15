using Gma.System.MouseKeyHook;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;
using System.Web;
using System.Diagnostics;


namespace TextToSpeech
{
    public partial class Form1 : Form
    {
        static public IKeyboardMouseEvents? m_Events;
        private IWebDriver driver;
        public static string mainLink = "https://thescrayx.github.io/index.html?text=Her%20Þey%20Yolunda%20Gözüküyor.";

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            driver.Navigate().GoToUrl("https://chat.openai.com/login");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Subscribe();

            EdgeOptions options = new EdgeOptions();
            driver = new EdgeDriver(options);
            driver.Navigate().GoToUrl("https://thescrayx.github.io/index.html?text=Her%20Þey%20Yolunda%20Gözüküyor.");
            timer1.Start();
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Unsubscribe();
            driver.Quit();
        }

        public void Subscribe()
        {
            m_Events = Hook.GlobalEvents();
            m_Events.KeyDown += OnKeyDown;
        }

        public void Unsubscribe()
        {
            m_Events.KeyDown -= OnKeyDown;
            m_Events.Dispose();
        }

        public void OnKeyDown(object? sender, KeyEventArgs e)
        {
            Console.WriteLine("Basýlan Tuþ: " + e.KeyCode);
            if (e.KeyCode == System.Windows.Forms.Keys.Subtract && button1.Enabled.ToString() == "False")
            {
                Console.WriteLine("Tuþ yakalandý");
                OpenAndListen();
            }
        }

        public void OpenAndListen()
        {
            if (driver.Url.ToString().Substring(0, 24) == "https://www.youtube.com/")
            {

            }
            else
            {
                driver.Navigate().GoToUrl("https://www.youtube.com/");
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                IWebElement button = wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("button.yt-spec-button-shape-next")));
                button.Click();
            }
        }

        public void CopyAndRequestApi(string url)
        {
            string[] speechText = url.Split("=");
            string utfText = (HttpUtility.UrlDecode(speechText[1])).Replace("+", " ");
            Console.WriteLine(utfText);

            string pythonScriptPath = @"C:\Users\thesc\OneDrive\Masaüstü\speech\TextToSpeech\bin\Debug\net6.0-windows\test.py";

            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = "python";
            psi.Arguments = $"{pythonScriptPath} {utfText}";
            psi.RedirectStandardOutput = true;
            psi.UseShellExecute = false;

            Process process = new Process();
            process.StartInfo = psi;
            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            Console.WriteLine(output);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if(driver.Url.ToString() == mainLink)
            {
                //do nothing
            } 
            else
            {
                if(driver.Url.ToString().Length > 24)
                {
                    if (driver.Url.ToString().Substring(0, 25) == "https://www.youtube.com/r")
                    {
                        mainLink = driver.Url.ToString();
                        Console.WriteLine(mainLink);
                        CopyAndRequestApi(driver.Url.ToString());
                    }
                }
            }
        }
    }
}