﻿using Google.Cloud.Speech.V1;
using NAudio.Wave;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;
using Themes_in_WPF;

namespace WpfApplication2
{
    /// <summary>
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        
        public static string SharedData { get; set; }

        private bool _isRecording = false; // флаг, указывающий, идет ли запись звука
        private WaveInEvent _waveIn; // объект для записи звука
        private WaveFileWriter _waveFileWriter; // объект для записи звука в файл

        private SpeechClientBuilder _builder; // объект для настройки клиента распознавания речи
        private SpeechClient _channel; // объект для распознавания речи
        private RecognitionConfig _config; // объект для настройки параметров распознавания речи

        string _lang= "en-US";

       
        public MainPage()
        {
           
            InitializeComponent();


            switch (Properties.Settings.Default.Theme)
            {
                case "Светлая":
                    AppTheme.ChangeTheme(new Uri("Theme/Light.xaml", UriKind.Relative));
                    
                    break;
                case "Темная":
                    AppTheme.ChangeTheme(new Uri("Theme/Dark.xaml", UriKind.Relative));
                    
                    break;
                case "Розовая":
                    //AppTheme.ChangeTheme(new Uri("Theme/Dark.xaml", UriKind.Relative));
                    
                    break;
            }



            textbox1.Text = Properties.Settings.Default.Theme;

            // инициализируем объекты для настройки клиента распознавания речи, клиента и параметров распознавания
            _builder = new SpeechClientBuilder
            {

                CredentialsPath = @"..\..\googleApi.json",
            };
            _channel = _builder.Build();
            
            
        }

        private async void StartButton_OnClick(object sender, RoutedEventArgs e)
        {

            if (_isRecording)
            {

                


                Style style = this.FindResource("ButtonDef") as Style;
                startButton.Style = style;

                StopButton_OnClick(_isRecording);
            }
            else
            {

                Style style = this.FindResource("ButtonOn") as Style;
                startButton.Style= style;

                switch (Properties.Settings.Default.Lang)
                {
                    case "Английский":
                        _lang="en-US";
                        break;
                    case "Русский":
                        _lang = "ru-RU";
                        break;
                }

                _isRecording = true;


                _config = new RecognitionConfig
                {
                    Encoding = RecognitionConfig.Types.AudioEncoding.Linear16,
                    SampleRateHertz = 44100,
                    EnableSpokenEmojis = true,
                    EnableAutomaticPunctuation = true,
                    EnableWordConfidence = true,
                    EnableWordTimeOffsets = true,
                    ProfanityFilter = true,
                    LanguageCode = _lang,
                   
                   

                    AudioChannelCount = 1,

                };
                // создаем объект WaveFormat для задания параметров записи звука
                WaveFormat format = new WaveFormat(44100, 16, 1);

                // создаем объект WaveInEvent для захвата звука с микрофона
                _waveIn = new WaveInEvent();
                _waveIn.WaveFormat = format;
                _waveIn.DeviceNumber = 0;

                // создаем объект WaveFileWriter для записи данных в файл
                string outputPath = "aud.wav";
                _waveFileWriter = new WaveFileWriter(outputPath, format);

                // обработчик события DataAvailable вызывается каждый раз, когда появляются новые данные от микрофона
                _waveIn.DataAvailable += WaveIn_DataAvailable;

                // запускаем запись звука
                _waveIn.StartRecording();

                // в бесконечном цикле ж
                while (_isRecording)
                {
                    await Task.Delay(500);
                }
                // создаем объект RecognitionAudio для передачи аудиоданных в API распознавания речи
                RecognitionAudio audio = RecognitionAudio.FromFile("aud.wav");
                RecognizeRequest request = new RecognizeRequest
                {
                    Audio = audio,
                    Config = _config
                };
                 UpdateTextBoxAsync(outputPath);

            }
        }
        private async Task UpdateTextBoxAsync(string filePath)
        {
            // создаем объект RecognitionAudio для передачи аудиоданных в API распознавания речи
            RecognitionAudio audio = RecognitionAudio.FromFile(filePath);
            RecognizeRequest request = new RecognizeRequest
            {
                Audio = audio,
                Config = _config
            };
            // отправляем запрос на распознавание речи
            RecognizeResponse response = await _channel.RecognizeAsync(request);
            

            
            // выводим результат распознавания речи в TextBox
            foreach (var result in response.Results)
            {
                foreach (var alternative in result.Alternatives)
                {

                           textbox1.Text+=(alternative.Transcript+" ");
                        

                }
            }
        }

        private void WaveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            // записываем данные в файл
            _waveFileWriter.Write(e.Buffer, 0, e.BytesRecorded);
        }

        private void StopButton_OnClick(bool checkRecord)
        {
            if (checkRecord)
            {
                
                _isRecording = false;
                // останавливаем запись звука
                _waveIn.StopRecording();

                // закрываем объект для записи звука в файл
                _waveFileWriter.Close();
                _waveFileWriter = null;
            }
            else  return; 

        }




        private void Open_Setting_Btn(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Setting());
        }
    }
}
