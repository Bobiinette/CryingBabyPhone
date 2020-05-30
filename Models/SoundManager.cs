using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Crying_baby_phone.Models
{
    class SoundManager
    {
        //Quel est le son précédent joué
        private int encouragement;
        //Si un encouragement est en cours
        private bool estEncouragement;
        //Son start
        private int start;
        //Sons roll
        private int[] rolls;
        //Sons clash
        private int[] clashs;
        //Sons well
        private int[] wells;
        //Son end
        private int end;
        //Media Player
        private MediaPlayer player;


        public SoundManager ()
        {
            this.encouragement = 0;
            player = new MediaPlayer();
            this.SetSounds(1);
            player.Completion += OnCompletion;
        }

        /// <summary>
        /// Choisit le son à jouer en fonction du mouvement
        /// </summary>
        /// <param name="mouvement">Le mouvement enregistré</param>
        public void PlaySound(AccelerometerEventHandler.MouvementPossibles mouvement)
        {
            if (this.player.IsPlaying)
            {
                return;
            }
            Random rand = new Random();
            switch (mouvement)
            {
                case AccelerometerEventHandler.MouvementPossibles.CHOC :
                    this.StartNewSound(clashs[rand.Next(0, 3)]);
                    break;
                case AccelerometerEventHandler.MouvementPossibles.BASCULE :
                    this.StartNewSound(rolls[rand.Next(0, 3)]);
                    break;
                case AccelerometerEventHandler.MouvementPossibles.LINEAIRE :
                    if (this.encouragement == 0)
                    {
                        this.encouragement = wells[rand.Next(0, 2)];
                    }
                    this.StartNewSound(this.encouragement);
                    break;
                case AccelerometerEventHandler.MouvementPossibles.DEBUT:
                    this.StartNewSound(start);
                    break;
                case AccelerometerEventHandler.MouvementPossibles.FIN:
                    this.StartNewSound(end);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// On lance la lecture d'un nouveau son
        /// </summary>
        /// <param name="sound">L'id du son à jouer</param>
        private void StartNewSound(int sound)
        {
            if (this.player.IsPlaying)
            {
                return;
            }
            this.estEncouragement = wells.Contains<int>(sound);
            player.Reset();
            player.SetDataSource(Application.Context, Android.Net.Uri.Parse("android.resource://" + Application.Context.PackageName + "/raw/" + sound));
            player.Prepare();
            player.Start();
        }

        /// <summary>
        /// On met les sons correspondants aux paramêtres
        /// </summary>
        /// <param name="i">Le paramêtre choisi par l'utilisateur</param>
        public void SetSounds(int i)
        {
            if (i == 1)
            {
                start = Resource.Raw.Voice01_01;
                rolls = new int[3] { Resource.Raw.Voice01_02, Resource.Raw.Voice01_03, Resource.Raw.Voice01_04 };
                clashs = new int[3] { Resource.Raw.Voice01_05, Resource.Raw.Voice01_06, Resource.Raw.Voice01_07 };
                wells = new int[2] { Resource.Raw.Voice01_08, Resource.Raw.Voice01_09 };
                end = Resource.Raw.Voice01_10;
            }
            else if (i == 2)
            {
                start = Resource.Raw.Voice02_01;
                rolls = new int[3] { Resource.Raw.Voice02_02, Resource.Raw.Voice02_03, Resource.Raw.Voice02_04 };
                clashs = new int[3] { Resource.Raw.Voice02_05, Resource.Raw.Voice02_06, Resource.Raw.Voice02_07 };
                wells = new int[2] { Resource.Raw.Voice02_08, Resource.Raw.Voice02_09 };
                end = Resource.Raw.Voice02_10;
            }
            else if (i == 3)
            {
                //Hellooooo
                start = Resource.Raw.Voice03_01;
                //Owwwwwww
                rolls = new int[3] { Resource.Raw.Voice03_02, Resource.Raw.Voice03_03, Resource.Raw.Voice03_04 };
                //I don't hate you | Please stop | No hard feelings
                clashs = new int[3] { Resource.Raw.Voice03_05, Resource.Raw.Voice03_06, Resource.Raw.Voice03_07 };
                //Hooray!!!! | I'm flying
                wells = new int[2] { Resource.Raw.Voice03_08, Resource.Raw.Voice03_09 };
                //Goodbyes
                end = Resource.Raw.Voice03_10;
            }
        }

        /// <summary>
        /// A appeler lorsqu'un son est fini d'être joué
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCompletion(object sender, EventArgs e)
        {
            if (estEncouragement)
            {
                foreach (var item in wells)
                {
                    //On change le prochain encouragement à jouer
                    if (this.encouragement != item)
                    {
                        this.encouragement = item;
                    }
                }
            }
        }
    }
}