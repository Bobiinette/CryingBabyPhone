using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Widget;
using Xamarin.Essentials;

namespace Crying_baby_phone.Models
{
    class AccelerometerReader
    {
        // Set speed delayfor monitoring changes.
        SensorSpeed speed = SensorSpeed.UI;
        //Gestionnaire d'événements
        private AccelerometerEventHandler eventHandler;
        //Gestionnaire des sons
        private SoundManager sound;


        /// <summary>
        /// Constructeur
        /// </summary>
        public AccelerometerReader()
        {
            // On utilise un délégué à appeler lors d'un changement sur les axes de l'accéléromètre
            Accelerometer.ReadingChanged+= Accelerometer_ReadingChanged;
            this.eventHandler = new AccelerometerEventHandler();
            this.sound = new SoundManager();
        }

        public AccelerometerReader(SoundManager soundManager) : this()
        {
            this.sound = soundManager;
        }
        /// <summary>
        /// Traite un évènement quand il y a eu un changement sur un des axes de l'accéléromètre
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Accelerometer_ReadingChanged(object sender, AccelerometerChangedEventArgs e)
        {
            var data = e.Reading;
            Task.Run(() => {
                this.eventHandler.AddAcceleration(data.Acceleration.X, data.Acceleration.Y, data.Acceleration.Z);
                MainThread.BeginInvokeOnMainThread(() => this.sound.PlaySound(eventHandler.Mouvement));
            });
        }
        /// <summary>
        /// Démarage de l'accéléromètre. Aucun effet s'il a déjà été lancé.
        /// </summary>
        public void StartAccelerometer()
        {
            try
            {
                if (!Accelerometer.IsMonitoring)
                    Accelerometer.Start(speed);
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Featurenot supportedon device
            }
            catch (Exception ex)
            {
                // Othererrorhas occurred.
            }
        }
        /// <summary>
        /// Arrêt de l'accéléromètre. Aucun effet s'il a déjà été arrété.
        /// </summary>
        public void StopAccelerometer()
        {
            try
            {
                if (Accelerometer.IsMonitoring)
                    Accelerometer.Stop();
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Featurenot supportedon device
            }
            catch (Exception ex)
            {
                // Othererrorhas occurred.
            }
        }
    }
}