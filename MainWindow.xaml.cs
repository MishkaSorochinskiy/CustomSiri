using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.Diagnostics;

namespace Recognizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SpeechRecognitionEngine recognize = new SpeechRecognitionEngine(new System.Globalization.CultureInfo("en-GB"));

        SpeechSynthesizer say = new SpeechSynthesizer();

        public MainWindow()
        {
            InitializeComponent();

            init();
        }

        private void init()
        {
            this.say.SetOutputToDefaultAudioDevice();

            this.say.Rate = 2;

            this.recognize.SpeechRecognized += this.speechCompleted;

            this.recognize.SetInputToDefaultAudioDevice();

            Choices commands = new Choices("what time is it", "size up", "size down", "change color",
                "move","open youtube","what is your name");

            GrammarBuilder build = new GrammarBuilder(commands);

            build.Culture = this.recognize.RecognizerInfo.Culture;

            Grammar gramatic = new Grammar(build);

            this.recognize.LoadGrammar(gramatic);
        }

        private void startRecognize(object sender, RoutedEventArgs e)
        {
            this.recognize.RecognizeAsync(RecognizeMode.Multiple);

            Task.Factory.StartNew(() => { this.say.Speak("Hello!! What do you want me to do??"); });
           
        }

        private void stopRecognize(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() => { this.say.Speak("Bye!!"); });

            this.recognize.RecognizeAsyncStop();
        }

        private void speechCompleted (object sender,SpeechRecognizedEventArgs args)
        {
            Random rand = new Random();

            switch (args.Result.Text)
            {
                case "change color":
                    this.kolobok.Fill = new SolidColorBrush
                        (Color.FromArgb(100, (byte)rand.Next(0, 255), (byte)rand.Next(0, 255), (byte)rand.Next(0, 255)));
                    break;

                case "what time is it":
                    Task.Factory.StartNew(
                        () => {
                            this.say.Speak(string.Format
                        ("Current Time: {0} {1}", DateTime.Now.Hour, DateTime.Now.Minute)); });
                    break;

                case "size up":
                    double width = rand.Next(0, 50);
                    this.kolobok.Width += width+ this.kolobok.Width >= 450 ? 0 : width;
                    break;

                case "size down":
                    width = rand.Next(0, 50);
                    this.kolobok.Width -= this.kolobok.Width-width <= 0 ? 0 : width;
                    break;

                case "move":
                    DoubleAnimation anim = new DoubleAnimation(0, 360, new Duration(new TimeSpan(0, 0, 3)));
                    anim.AutoReverse = true;
                    RotateTransform rt = new RotateTransform();
                    this.kolobok.RenderTransform = rt;
                    rt.BeginAnimation(RotateTransform.AngleProperty, anim);
                    break;

                case "open youtube":
                    Process.Start(@"https://www.youtube.com/");
                    break;

                case "what is your name":
                    Task.Factory.StartNew(() => {
                        this.say.Speak("My name is Elena!! What's your name??");
                    });
                    break;

                default:
                    Task.Factory.StartNew(() => { this.say.Speak
                        ("Unrecognized: " + args.Result.Text); });
                    break;
            }

            this.textbox.Text += "\n" + args.Result.Text;
        }
    }
}
