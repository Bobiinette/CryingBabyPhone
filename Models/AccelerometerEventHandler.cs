using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Timers;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Crying_baby_phone.Models
{
    class AccelerometerEventHandler
    {
        //Accélération au repos, quand le téléphone ne bouge pas
        private bool minSet;
        //Mouvements posibles
        public enum MouvementPossibles {NON, BASCULE, CHOC, LINEAIRE, DEBUT, FIN, AUTRE};
        //Mouvements actuel
        private MouvementPossibles mouvement;
        //Norme de la dernière mesure
        private double oldNorme;
        //Pour savoir si on doit jouer la fin
        private bool estEnMouvement;
        //Timer pour espacer les events
        private Stopwatch timerLineaire;
        //Timer pour espacer les events
        private Stopwatch timerSansMouvement;
        //Timer pour espacer les events
        private Stopwatch timerBascule;
        //Valeurs initiales pour les 3 axes
        private double xx;
        private double yy;
        private double zz;

        public MouvementPossibles Mouvement { get => mouvement; }
        public bool EstEnMouvement { get => estEnMouvement; }

        public AccelerometerEventHandler()
        {
            //On veut garder un historique des normes
            minSet = false;
            this.mouvement = MouvementPossibles.NON;
            timerLineaire = new Stopwatch();
            timerSansMouvement = new Stopwatch();
            timerBascule = new Stopwatch();
        }
        /// <summary>
        /// On ajoute la dernière valeur lue sur l'accéléromètre et on supprime la valeur la plus vielle si on a atteind la taille maximale
        /// </summary>
        public void AddAcceleration(double ddx, double ddy, double ddz)
        {
            double norme;
            if (!this.minSet)
            {
                this.xx = ddx * ddx;
                this.yy = ddy * ddy;
                this.zz = ddz * ddz;
                this.minSet = true;
            }
            double norme2 = ddx * ddx - this.xx + ddy * ddy - this.yy + ddz * ddz - this.zz;
            norme = (norme2 <= 0) ? 0 : Math.Sqrt(norme2);
            this.SetMouvement(norme);
        }

        /// <summary>
        /// On met à jour le mouvement actuel
        /// </summary>
        /// <param name="norme"></param>
        private void SetMouvement(double norme)
        {
            if (!this.minSet)
            {
                this.mouvement = MouvementPossibles.NON;
                this.estEnMouvement = false;
            }
            //Cas du début
            else if (!this.estEnMouvement && norme > 0.3)
            {
                this.mouvement = MouvementPossibles.DEBUT;
                this.estEnMouvement = true;
            }
            else if (!this.estEnMouvement)
            {
                this.mouvement = MouvementPossibles.NON;
            }
            else if (Math.Abs(oldNorme - norme) > 0.6 && norme < 0.2)
            {
                //Si un mouvement est trop sec et qu'il s'arrête d'un coup c'est un choc
                this.mouvement = MouvementPossibles.CHOC;
                this.estEnMouvement = true;
            }
            else if (Math.Abs(oldNorme - norme) > 0.6 && norme > 0.4)
            {
                //Pour jouer la bascule il faut qu'on reste en mouvement un certain temps
                if (timerBascule.ElapsedMilliseconds > 300)
                {
                    this.mouvement = MouvementPossibles.BASCULE;
                }
                else
                {
                    this.mouvement = MouvementPossibles.AUTRE;
                }
                if (!timerBascule.IsRunning)
                {
                    timerBascule.Start();
                }
                this.estEnMouvement = true;
            }
            else if (Math.Abs(oldNorme - norme) < 0.2 && norme > 0.2)
            {
                //Dans le cas d'un mouvement linéaire, on attend les 5 secondes avant de dire que c'est un mouvement linéaire
                if (timerLineaire.ElapsedMilliseconds > 5000)
                {
                    this.mouvement = MouvementPossibles.LINEAIRE;
                }
                else
                {
                    this.mouvement = MouvementPossibles.AUTRE;
                }
                
                if (!timerLineaire.IsRunning)
                {
                    timerLineaire.Start();
                }
                this.estEnMouvement = true;
            }
            else if (norme < 0.2)
            {
                //Si on ne bouge pas pendant 4 secondes d'affilé c'est que c'est la fin
                if (timerSansMouvement.ElapsedMilliseconds > 4000 && this.estEnMouvement)
                {
                    this.mouvement = MouvementPossibles.FIN;
                    this.estEnMouvement = false;
                }
                else
                {
                    this.mouvement = MouvementPossibles.NON;
                }
                if (!timerSansMouvement.IsRunning)
                {
                    timerSansMouvement.Start();
                }
            }
            else
            {
                this.mouvement = MouvementPossibles.AUTRE;
            }

            oldNorme = norme;
            this.gestionTimers();
            
        }

        /// <summary>
        /// Pour réinitialiser les timers au bon moment
        /// </summary>
        private void gestionTimers()
        {
            if (this.mouvement == MouvementPossibles.CHOC || this.mouvement == MouvementPossibles.BASCULE)
            {
                timerLineaire.Stop();
                timerLineaire.Reset();
            }
            if (timerLineaire.ElapsedMilliseconds > 6000)
            {
                timerLineaire.Stop();
                timerLineaire.Restart();
            }
            if (this.mouvement == MouvementPossibles.CHOC || this.mouvement == MouvementPossibles.LINEAIRE)
            {
                timerBascule.Stop();
                timerBascule.Reset();
            }
            if (timerBascule.ElapsedMilliseconds > 500)
            {
                timerBascule.Stop();
                timerBascule.Restart();
            }
            if (this.mouvement != MouvementPossibles.NON)
            {
                timerSansMouvement.Stop();
                timerSansMouvement.Reset();
            }
            if (!this.estEnMouvement)
            {
                timerSansMouvement.Stop();
                timerSansMouvement.Reset();
                timerLineaire.Stop();
                timerLineaire.Reset();
                timerBascule.Stop();
                timerBascule.Reset();
            }
        }
    }
}