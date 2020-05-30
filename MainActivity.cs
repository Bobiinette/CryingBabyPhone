using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Crying_baby_phone.Models;
using System;
using Android.Media;
using System.Runtime.CompilerServices;
using Android.Widget;
using Android.Graphics.Drawables;

namespace Crying_baby_phone
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private AccelerometerReader accelerometer;
        private SoundManager soundManager;
        private static Button button01;
        private static Button button02;
        private static Button button03;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);
            this.soundManager = new SoundManager();
            this.accelerometer = new AccelerometerReader(this.soundManager);
            this.accelerometer.StartAccelerometer();

            //Initialisation des boutons
            this.InitButtons();
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnResume()
        {
            base.OnResume();
            this.accelerometer.StartAccelerometer();
        }

        protected override void OnPause()
        {
            base.OnPause();
            accelerometer.StopAccelerometer();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            accelerometer.StopAccelerometer();
        }

        /// <summary>
        /// Initialisation des boutons
        /// </summary>
        private void InitButtons()
        {
            //On récupère les boutons avec leurs ids
            button01 = FindViewById<Button>(Resource.Id.button_voice_01);
            button02 = FindViewById<Button>(Resource.Id.button_voice_02);
            button03 = FindViewById<Button>(Resource.Id.button_voice_03);
            //On met les couleurs de bases
            button01.SetBackgroundColor(Android.Graphics.Color.Green);
            button02.SetBackgroundColor(Android.Graphics.Color.Gray);
            button03.SetBackgroundColor(Android.Graphics.Color.Gray);
            //On défnie les actions au click
            button01.Click += (sender, e) => {
                this.soundManager.SetSounds(1);
                button01.SetBackgroundColor(Android.Graphics.Color.Green);
                button02.SetBackgroundColor(Android.Graphics.Color.Gray);
                button03.SetBackgroundColor(Android.Graphics.Color.Gray);
            };
            button02.Click += (sender, e) => {
                this.soundManager.SetSounds(2);
                button01.SetBackgroundColor(Android.Graphics.Color.Gray);
                button02.SetBackgroundColor(Android.Graphics.Color.Green);
                button03.SetBackgroundColor(Android.Graphics.Color.Gray);
            };
            button03.Click += (sender, e) => {
                this.soundManager.SetSounds(3);
                button01.SetBackgroundColor(Android.Graphics.Color.Gray);
                button02.SetBackgroundColor(Android.Graphics.Color.Gray);
                button03.SetBackgroundColor(Android.Graphics.Color.Green);
            };
        }
    }
}