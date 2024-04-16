using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.IO; // lecture de fichier
using System.Dynamic;

namespace MN41V2
{
    internal class Program
    {
        static void Main(string[] args)
        {

            const int dim = 2; // dimension du problème

            //--------------------- Recuperation des données a partir de Donnees.txt------------------------------------

            Console.Write("\n ...Recupération des données... \n ");

            //ouverture du fichier 'Donnee.txt'
            string CheminDonnee = @"Donnee.txt";
            StreamReader Donnee = new StreamReader(CheminDonnee);

            // Recupération d'informations sur les Elements 
            Donnee.ReadLine(); // saut de ligne

            int NbElement = Convert.ToInt32(Donnee.ReadLine()); // nombre d'élements

            Donnee.ReadLine();
            Donnee.ReadLine();

            double E = Convert.ToDouble(Donnee.ReadLine()); // module de young des barres (en pascal)

            Donnee.ReadLine();
            Donnee.ReadLine();

            double D = Convert.ToDouble(Donnee.ReadLine()); // diametre (en mètre)

            Donnee.ReadLine();
            Donnee.ReadLine();
            Donnee.ReadLine();

            int[,] TabConnexion = new int[NbElement, dim]; // tableau de connexion
                                                           // 1er colonne Noeud 1
                                                           // 2eme colonne Noeud 2

            // Recuperation du tableau de connexion
            for (int i = 0; i < NbElement; i++)
            {
                string texte = Donnee.ReadLine(); // défintion d'une chaine de caractère 'texte' qui contient toute la ligne
                string[] nums = texte.Split(' '); // découpage de 'texte' lorsqu'il y a un espace, les parties obtenues sont mises dans le tabelau 'nums'
                for (int j = 0; j < dim; j++)
                {
                    TabConnexion[i, j] = Convert.ToInt32(nums[j + 1]); // convertion de chaine de carctère en entier et remplissage de 'TabConnexion'
                }
            }

            // Affichage des informations récupérés
            Console.Write("\n Nombre d'elements : {0,8:f3}\n", NbElement);
            Console.Write("\n Module de Young : {0,8:f3}\n", E);
            Console.Write("\n Diamètre : {0,8:f3}\n", D);

            Donnee.ReadLine();
            Donnee.ReadLine();

            // Récupération d'informations sur les noeuds
            int NbNoeud = Convert.ToInt32(Donnee.ReadLine()); // nombre de noeuds

            Donnee.ReadLine();
            Donnee.ReadLine();
            Donnee.ReadLine();

            // Tableau ou sont répertoriés les coordonnées des noeuds
            // colonne 1 => coordonnées X
            // colonne 2 => coordonnées Y
            double[,] TabCoordonnee = new double[NbNoeud, dim];

            // Recuperation du tableau de coordonnées
            for (int i = 0; i < NbNoeud; i++)
            {
                string texte = Donnee.ReadLine();
                string[] nums = texte.Split(' ');

                for (int j = 0; j < dim; j++)
                {
                    TabCoordonnee[i, j] = Convert.ToDouble(nums[j + 1]);
                }
            }

            Donnee.ReadLine();
            Donnee.ReadLine();
            Donnee.ReadLine();

            // Contrainte sur les noeuds
            // colonne 1 contrainte sur X (1 = oui, 0 = non)
            // colonne 2 valeur de la contrainte sur X
            // colonne 3 contrainte sur Y (1 = oui, 0 = non)
            // colonne 4 valeur de la contrainte sur Y
            double[,] Contrainte = new double[NbNoeud, 4];

            for (int i = 0; i < NbNoeud; i++)
            {
                string texte = Donnee.ReadLine();
                string[] nums = texte.Split(' ');

                for (int j = 0; j < 4; j++)
                {
                    Contrainte[i, j] = Convert.ToDouble(nums[j + 1]); ;
                }
            }

            Donnee.ReadLine();
            Donnee.ReadLine();
            Donnee.ReadLine();

            // Forces appliqués au noeuds 
            // colonne 1 : valeur de la force
            // colonne 2 : angle de la force
            double[,] Force = new double[NbNoeud, 2];

            for (int i = 0; i < NbNoeud; i++)
            {
                string texte = Donnee.ReadLine();
                string[] nums = texte.Split(' ');

                for (int j = 0; j < 2; j++)
                {
                    Force[i, j] = Convert.ToDouble(nums[j + 1]); ;
                }
            }

            Donnee.ReadLine();
            Donnee.ReadLine();


            // méthode de résolution choisi
            // 1 => méthode LU
            // 2 => méthode de Gauss
            // 3 => méthode de Thomas 
            int methode = Convert.ToInt32(Donnee.ReadLine());

            Donnee.Close(); // fermeture du fichier

            // Affichage informations sur les noeuds
            Console.Write("\n Nombre de noeud : {0,8:f0}\n", NbNoeud);

            Console.Write("\n Tableau de coordonnées :\n");
            for (int i = 0; i < NbNoeud; i++)
            {
                for (int j = 0; j < dim; j++)
                {
                    Console.Write(" {0,8:f3} ", TabCoordonnee[i, j]);
                }
                Console.Write("\n");
            }

            Console.Write("\n Tableau de contrainte :\n");
            for (int i = 0; i < NbNoeud; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    Console.Write(" {0,8:f3} ", Contrainte[i, j]);
                }
                Console.Write("\n");
            }

            Console.Write("\n Tableau 'Force' :\n");
            for (int i = 0; i < NbNoeud; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    Console.Write(" {0,8:f3} ", Force[i, j]);
                }
                Console.Write("\n");
            }

            Console.Write("\n Méthode de résolution : {0,8:f0}\n", methode);


            //------------------------- Calcul des angles et des raideurs de chaque barre ------------------------

            Console.Write("\n...Calcul des angles et des raideurs de chaque barre...\n");
            // en C# les tableaux sont initilaisé a zéro a la creation
            double[] LongeurB = new double[NbElement]; // tableau qui repertorie les longeurs de chaques barres
            double[] Angle = new double[NbElement];  // tableau qui repertorie les angles de chaques barres
            double[] TabRaideur = new double[NbElement]; // tableau qui repertorie la raideur de chaques barres

            for (int i = 0; i < NbElement; i++)
            {
                // identification des noeuds réliés a l'élément
                int Ni = TabConnexion[i, 0]; // Noeud d'une extrémité de la barre
                int Nj = TabConnexion[i, 1]; // Noeud de l'autre extrémité de la barre

                // Longueur de la barre sur X et sur Y 
                double x = TabCoordonnee[Nj - 1, 0] - TabCoordonnee[Ni - 1, 0];
                double y = TabCoordonnee[Nj - 1, 1] - TabCoordonnee[Ni - 1, 1];

                // Calcul de l'angle de la barre par rapport à l'horizontale en radians
                if (x != 0)
                {
                    Angle[i] = Math.Atan(y / x);
                }
                else
                {
                    // Cas non present dans le systeme a analyser
                    Angle[i] = Math.PI / 2.0;
                }


                // Calcul de la longueur de la barre
                LongeurB[i] = Math.Sqrt(Math.Pow(x, 2.0) + Math.Pow(y, 2));

                // Calcul de le raideur de la barre
                TabRaideur[i] = (Math.PI * E * Math.Pow(D, 2.0)) / (4 * LongeurB[i]);
            }

            // Affichage des raideurs et des angles pour chaque élément
            for (int i = 0; i < NbElement; i++)
            {
                Console.Write("\n Element {0,2:f0} :", i + 1);
                Console.Write("\n Raideur = {0,4:f3} ", TabRaideur[i]);
                Console.Write("\n Angle = {0,4:f3}\n", (Angle[i] * 180) / Math.PI);

            }

            // -----------------Creation des Matrice elementaire et Assemblage dans la matrice 'MAssemble'------------------------------------------------
            Console.Write("\n...Creation des matrices élémentaires et assemblage...\n");
            double[,] MAssemble = new double[NbNoeud * dim, NbNoeud * dim]; // matrice assemblée

            for (int i = 0; i < NbElement; i++)
            {

                double C2 = Math.Pow(Math.Cos(Angle[i]), 2); // cos^2(angle)
                double S2 = Math.Pow(Math.Sin(Angle[i]), 2); // sin^2(angle)
                double CS = Math.Cos(Angle[i]) * Math.Sin(Angle[i]); // cos(angle)*sin(angle)

                // déclaration de la matrice élémentaire
                double[,] MElementaire = new double[4, 4] {{  C2,  CS, -C2, -CS},
                                                           {  CS,  S2, -CS, -S2},
                                                           { -C2, -CS,  C2,  CS},
                                                           { -CS, -S2,  CS,  S2}};

                // Multiplication de 'MElementaire' par la raideur de la barre 'i'
                for (int l = 0; l < 4; l++)
                {
                    for (int c = 0; c < 4; c++)
                    {
                        MElementaire[l, c] *= TabRaideur[i];
                    }
                }

                // Affichage de 'MElementaire' 
                Console.Write("\n  M{0,2:f0} :\n", i + 1);
                AffichageM(MElementaire, false);

                // Assemblage de la matrice

                int Ni = TabConnexion[i, 0]; // Noeud d'une extrémité de la barre
                int Nj = TabConnexion[i, 1]; // Noeud de l'autre extrémité de la barre

                // Tableau des emplacements des coefficients de 'MElementaire' dans 'MAssemble'
                int[] Tab = new int[4] { (Ni - 1) * 2, (Ni - 1) * 2 + 1, (Nj - 1) * 2, (Nj - 1) * 2 + 1 };

                // Positionnment des coefficient de la matrice élémentaire dans la matrice assemblée
                for (int l = 0; l < 4; l++)
                {
                    for (int c = 0; c < 4; c++)
                    {
                        MAssemble[Tab[l], Tab[c]] += MElementaire[l, c];
                    }
                }

                // Affichage des Matrices Assemblées
                AffichageM(MAssemble, true);
            }

            // ------------------------ Reduction de 'MAssemble' ------------------------------------------------------------

            // Passage du tableau 'Contrainte' (4x8) au tableau 'ContrainteXY' (2x16)
            // colonne 1 : 0 et 1 pour savoir si il y a une contrainte
            // colonne 2 : valeur de la contrainte
            double[,] ContrainteXY = new double[NbNoeud * dim, 2];

            for (int i = 0; i < 8; i++)
            {
                // passage des 0 et 1
                ContrainteXY[i * 2, 0] = Contrainte[i, 0];
                ContrainteXY[(i * 2) + 1, 0] = Contrainte[i, 2];

                // passage de la valeur de la contrainte
                ContrainteXY[i * 2, 1] = Contrainte[i, 1];
                ContrainteXY[(i * 2) + 1, 1] = Contrainte[i, 3];
            }

            // Comptage du nombre de déplacements possibles
            int NbDeplacement = 0;

            for (int i = 0; i < NbNoeud * dim; i++)
            {
                if (ContrainteXY[i, 0] == 0)
                {
                    // incremente le nombre de déplacements possibles
                    NbDeplacement++;
                }
            }

            Console.Write("\n...Reduction de la matrice assemblée...\n");
            // Affichage de La Matrice Assemblé avec les lignes barrés

            for (int l = 0; l < NbNoeud * dim; l++)
            {
                Console.Write("\n");
                for (int c = 0; c < NbNoeud * dim; c++)
                {

                    if (ContrainteXY[l, 0] == 1 || ContrainteXY[c, 0] == 1)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write(" {0,8:f2} ", MAssemble[l, c]);
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(" {0,8:f2} ", MAssemble[l, c]);

                    }

                }
                Console.Write("     {0,8:f2}  ", ContrainteXY[l, 0]);
                Console.Write("\n");
            }
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("\n\n\n");

            // Calcul des forces Fx et Fy appliqué a un point
            // Passage du tableau 'Force' (4x8) au tableau 'ForceXY' (1x16)
            double[] ForceXY = new double[NbNoeud * dim];

            for (int i = 0; i < NbNoeud; i++)
            {
                ForceXY[i * 2] = Force[i, 0] * Math.Cos(Force[i, 1] * Math.PI / 180); // Force sur l'axe x (conversion de l'angle en radians)
                ForceXY[(i * 2) + 1] = Force[i, 0] * Math.Sin(Force[i, 1] * Math.PI / 180); // Force sur l'axe y
            }

            // Extraction de la matrice reduite
            double[,] MReduite = new double[NbDeplacement, NbDeplacement]; // declaration matrice réduite

            int colonneMReduite = 0;
            for (int c = 0; c < NbNoeud * dim; c++) // déplacement sur les colonnes de la matrice reduite
            {
                if (ContrainteXY[c, 0] == 0) // si le Noeud 'c' n'est pas contraint
                {
                    int ligneMReduite = 0;
                    // Recopie les colonnes dans la matrice reduite
                    for (int l = 0; l < NbNoeud * dim; l++)
                    {
                        if (ContrainteXY[l, 0] == 0)
                        {
                            MReduite[ligneMReduite, colonneMReduite] = MAssemble[c, l];
                            ligneMReduite++;
                        }
                    }
                    colonneMReduite++;
                }
                else // si le Noeud 'c' est contraint on change le second membre 'ForceXY'
                {
                    for (int l = 0; l < NbNoeud * dim; l++)
                    {
                        if (ContrainteXY[l, 0] == 0)
                        {
                            ForceXY[l] = ForceXY[l] - (ContrainteXY[c, 1] * MAssemble[l, c]);
                        }
                    }
                }
            }

            // ------------------------------- Reduction de 'ForceXY' ---------------------------------
            // cette étape est nécessaire pour résoudre le système par la suite
            double[] ForceXYRed = new double[NbDeplacement];

            for (int i = 0, j = 0; i < NbNoeud * dim; i++)
            {
                if (ContrainteXY[i, 0] == 0)
                {
                    ForceXYRed[j] = ForceXY[i];
                    j++;
                }
            }

            //Affichage 'MReduite' et 'ForceXYRed'
            AffichageMV(MReduite, ForceXYRed, true);

            //-------------------------- Resolution de 'MReduite' * 'X' = 'ForceXYRed' ---------------------------------------------
            Console.Write("\n\n...Resolution par la méthode choisie...\n");
            double[] X = new double[NbDeplacement];

            if (methode == 1) // méthode LU
            {
                X = MethodeLU(MReduite, ForceXYRed);
            }
            else if (methode == 2) // méthode de Gauss
            {
                X = MethodeGauss(MReduite, ForceXYRed);
            }
            else if (methode == 3) // méthode de Thomas
            {
                X = MethodeThomas(MReduite, ForceXYRed);
            }

            // Sauvegarde de tout les déplacement dans le tableau 'Deplacement'
            double[] Deplacement = new double[NbNoeud * dim];

            for (int i = 0, j = 0; i < NbNoeud * dim; i++)
            {
                if (ContrainteXY[i, 0] == 0)
                {
                    Deplacement[i] = X[j];
                    j++;
                }
            }

            // Affichage du resultat 'X' en mm
            Console.Write("\n Deplacement des noeuds : \n");
            for (int i = 0, j = 0; i < NbNoeud; i++)
            {

                Console.Write("\n U{0,1:f0}  =", i + 1);
                Console.Write("{0,10:f6} mm ", Deplacement[j] * 1000);
                j++;
                Console.Write(" V{0,1:f0}  =", i + 1);
                Console.Write("{0,10:f6} mm", Deplacement[j] * 1000);
                j++;
            }
            Console.Write("\n");

            // -------------------- Calcul des Reactions des supports ----------------------- 
            Console.Write("\n...Calcul des reactions des supports...\n");

            for (int l = 0; l < NbNoeud * dim; l++)
            {
                if (ContrainteXY[l, 0] == 1)
                {
                    double Somme = 0;

                    for (int c = 0; c < NbNoeud * dim; c++)
                    {
                        if (ContrainteXY[c, 0] == 0)
                        {
                            Somme += MAssemble[l, c] * Deplacement[c];
                            
                        }
                    }

                    ForceXY[l] = Somme;
                }
            }
            // Affichage des forces appliqués aux noeuds
            Console.Write("\n Forces appliqués aux noeuds : \n");
            for (int i = 0, j = 0; i < NbNoeud; i++)
            {
                Console.Write("\n N{0,1:f0} : ", i + 1);
                Console.Write(" Fx = {0,10:f6} N ", ForceXY[j]);
                j++;
                Console.Write(" Fy = {0,10:f6} N ", ForceXY[j]);
                j++;
            }
            Console.Write("\n");

            // Verification Somme des forces est nulle
            double SForce = 0;
            for (int i = 0; i < NbNoeud * dim; i++)
            {
                SForce += ForceXY[i];
            }
            Console.Write("\n  Somme des Forces = {0,4:f2} \n", SForce);


            //------------------------------ Calcul des Tensions de chaque barre--------------------------------
            Console.Write("\n...Calcul des tensions de chaque barre...\n");
            double[] Tension = new double[NbElement];

            Console.Write("\n Tension de chaque barre : \n");
            for (int i = 0; i < NbElement; i++)
            {
                int Ni = TabConnexion[i, 0]; // Noeud d'une extrémité de la barre
                int Nj = TabConnexion[i, 1]; // Noeud de l'autre extrémité de la barre

                // Calcul des coordonnées des noeuds 'Ni' et 'Nj' après déplacement
                //Ni
                double Xi = TabCoordonnee[Ni - 1, 0] + Deplacement[(Ni - 1) * 2]; // coordonnée X de Ni
                double Yi = TabCoordonnee[Ni - 1, 1] + Deplacement[(Ni - 1) * 2 + 1]; // coordonnée Y de Ni
                //Nj
                double Xj = TabCoordonnee[Nj - 1, 0] + Deplacement[(Nj - 1) * 2]; // coordonnée X de Nj
                double Yj = TabCoordonnee[Nj - 1, 1] + Deplacement[(Nj - 1) * 2 + 1]; // coordonnée Y de Nj

                // Calcul de la nouvelle longueur de la barre
                double Longueur = Math.Sqrt(Math.Pow(Xi - Xj, 2.0) + Math.Pow(Yi - Yj, 2));

                // Calcul de l'allongement de la barre après deplacement
                double Allongement = Longueur - LongeurB[i];

                //Calcul de la tension de la barre avec la loi de Hook
                Tension[i] = Allongement * TabRaideur[i];

                // Affichage de la tension de la barre
                Console.Write("\n E{0,1:f0} : ", i + 1);
                if (Tension[i] > 0)
                {
                    Console.Write(" {0,4:f6} N (traction) ", Tension[i]);
                }
                else
                {
                    Console.Write(" {0,4:f6} N (compression) ", Tension[i]);
                }

            }

            //------------------------- Ecriture des résultats dans le fichier ---------------------------
            Console.Write("\n\n...Ecriture des résultats dans un fichier texte...\n");
            //ouverture du fichier 'Resultat.txt'
            string CheminResultat = @"Resultat.txt";
            StreamWriter Resultat = new StreamWriter(CheminResultat);

            // ecriture des déplacement
            Resultat.WriteLine("Deplacement de chaque noeuds (en mètre)");
            Resultat.WriteLine("   X           Y");

            for (int i = 0; i < NbNoeud; i++)
            {
                Resultat.Write("N{0,1:f0}  ", i + 1);
                Resultat.Write(" {0,10:f8} ", Deplacement[i * 2]);
                Resultat.Write(" {0,10:f8}\n", Deplacement[(i * 2) + 1]);

            }
            Resultat.WriteLine();
            // ecriture des forces appliqués aux noeuds
            Resultat.WriteLine("Force appliqués a chaque noeuds (en newton)");
            Resultat.WriteLine("   Fx         Fy");

            for (int i = 0; i < NbNoeud; i++)
            {
                Resultat.Write("N{0,1:f0}  ", i + 1);
                Resultat.Write(" {0,8:f6} ", ForceXY[i * 2]);
                Resultat.Write(" {0,8:f6}\n", ForceXY[(i * 2) + 1]);

            }
            Resultat.WriteLine();


            // ecriture des tensions de chaque barre
            Resultat.WriteLine("Tension de chaque barre (en newton)");

            for (int i = 0; i < NbElement; i++)
            {
                Resultat.Write("E{0,1:f0}", i + 1);
                if (Tension[i] > 0)
                {
                    Resultat.Write(" {0,8:f4} N  (traction)\n", Tension[i]);
                }
                else
                {
                    Resultat.Write(" {0,8:f4} N  (compression)\n", Tension[i]);
                }
            }

            Resultat.Close();

            Console.ReadLine();
        }

        //--------------------------------- Fonctions de Resolution -------------------------------------------------

        // Méthode de LU
        static double[] MethodeLU(double[,] A, double[] B)
        {

            Console.Write("\n-------------------------------------- Méthode LU ------------------------------------- :\n");

            int TAILLE = A.GetLength(0); // Obtient la taille d'une ligne de la matrice

            // Decomposition de 'A' en 'L' * 'U'
            double[,] L = new double[TAILLE, TAILLE];
            double[,] U = new double[TAILLE, TAILLE];

            // Mettre des 1 sur la digonale de L
            for (int i = 0; i < TAILLE; i++)
            {
                L[i, i] = 1;
            }

            // Calcul des coefficients de 'U' et de 'L'
            for (int r = 0; r < TAILLE; r++)
            {
                for (int j = r; j < TAILLE; j++)
                {
                    double Somme = 0;

                    for (int s = 0; s < r; s++)
                    {
                        Somme = Somme + L[r, s] * U[s, j];
                    }

                    U[r, j] = A[r, j] - Somme;
                }

                for (int i = r; i < TAILLE; i++)
                {
                    double Somme = 0;

                    for (int s = 0; s < r; s++)
                    {
                        Somme = Somme + L[i, s] * U[s, r];
                    }

                    L[i, r] = (A[i, r] - Somme) / U[r, r];
                }

            }

            // Affichage de L
            Console.Write("\n  L : \n");
            AffichageM(L, true);

            // Affichage U
            Console.Write("\n  U : \n");
            AffichageM(U, true);

            // Verification que L*U = MReduite
            double[,] LU = ProduitM(L, U);
            if (VerificationM(LU, A))
            {
                Console.ForegroundColor = ConsoleColor.Green; // mettre l'écriture en vert
                Console.Write("\n ========> L x U = MReduite \n");
                Console.ForegroundColor = ConsoleColor.White; //remettre l'écriture en blanc
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("\n ========> ERREUR : L x U pas égale à MReduite \n");
                Console.ForegroundColor = ConsoleColor.White;
            }

            // Calcul de 'Y' tel que 'L' * 'Y' = 'ForceXY'
            double[] Y = new double[TAILLE];

            for (int i = 0; i < TAILLE; i++)
            {

                double Somme = 0;

                for (int j = 0; j < i; j++)
                {
                    Somme = Somme + (L[i, j] * Y[j]);
                }
                Y[i] = B[i] - Somme;
            }

            // Affichage 'Y'
            Console.Write("\n Y :\n ");
            AffichageV(Y);

            // Verification que 'L' * 'Y' = 'B'
            double[] LY = ProduitV(L, Y); // Calcul de 'L' * 'Y'

            if (VerificationV(LY, B)) // Si 'L' * 'Y' = 'B'
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("\n ========> L x Y = B \n");
                Console.ForegroundColor = ConsoleColor.White;
            }
            else // Si 'L' * 'Y' different de 'B'
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("\n ========> ERREUR : L x U pas égale à B \n");
                Console.ForegroundColor = ConsoleColor.White;
            }

            // Calcul de 'X' tel que  'U' * 'X' = 'Y'

            //U[TAILLE - 1, TAILLE - 1] = 1; ca sert a rien ? 

            double[] X = new double[TAILLE];

            // Initialisation
            X[TAILLE - 1] = Y[TAILLE - 1] / U[TAILLE - 1, TAILLE - 1];

            // Calcul des coefficients de 'X'
            for (int k = TAILLE - 2; k > -1; k--)
            {
                double Somme = 0;

                for (int r = k + 1; r < TAILLE; r++)
                {
                    Somme = Somme + (U[k, r] * X[r]);
                }

                X[k] = (Y[k] - Somme) / U[k, k];

            }

            // Affichage 'X'
            Console.Write("\n  X :\n ");
            AffichageV(X);

            // Verification que 'U' * 'X' = 'Y'
            double[] UX = ProduitV(U, X);

            if (VerificationV(UX, Y))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("\n ========> U x X = Y \n");
                Console.ForegroundColor = ConsoleColor.White;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("\n ========> ERREUR : U x X pas égale à Y \n");
                Console.ForegroundColor = ConsoleColor.White;
            }

            // Verifiaction que 'A' * 'X' = 'B'
            double[] AX = ProduitV(A, X);
            if (VerificationV(AX, B))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("\n ========> A x X = B\n");
                Console.ForegroundColor = ConsoleColor.White;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("\n ========> ERREUR : A x X pas égale à B \n");
                Console.ForegroundColor = ConsoleColor.White;
            }

            return X;
        }

        // Méhode de Gauss
        static double[] MethodeGauss(double[,] A, double[] B)
        {

            Console.Write("\n------------------------------------- Méthode de Gauss ------------------------------------- :\n");

            int TAILLE = A.GetLength(0); // Obtient la taille d'une ligne de la matrice

            double[,] M = new double[TAILLE, TAILLE + 1];

            // Remplissage de la Matrice 'M'
            // partie gauche de 'M' => A
            // dernière colonne de 'M' => B
            for (int j = 0; j < TAILLE; j++)
            {
                for (int i = 0; i < TAILLE; i++)
                {
                    M[j, i] = A[j, i];
                }

                M[j, TAILLE] = B[j];

            }

            //Triangularisation de 'M'
            Console.Write("\n....Triangularisation... \n");

            for (int etape = 0; etape < TAILLE - 1; etape++)// chaque etape de 0 à TAILLE - 1
            {
                for (int ligne = 1 + etape; ligne < TAILLE; ligne++)
                {

                    double Coef = (M[ligne, etape] / M[etape, etape]);


                    for (int i = 0; i < TAILLE + 1; i++)
                    {
                        M[ligne, i] -= Coef * M[etape, i];
                    }
                }

                //Affichage de 'M' aux différentes étapes 
                Console.Write("\n\n Etape {0,4:d1} : \n ", etape + 1);
                for (int j = 0; j < TAILLE; j++)
                {
                    Console.Write("\n");
                    for (int i = 0; i < TAILLE; i++)
                    {
                        if (M[j, i] == 0)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write(" {0,8:f2} ", M[j, i]);
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write(" {0,8:f2} ", M[j, i]);
                        }

                    }
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("     {0,8:f3}  \n", M[j, TAILLE]);
                }
            }

            //Remontée/Resolution

            Console.Write("\n ...Resolution/Remontée... \n");
            double[] X = new double[TAILLE];
            double[] Y = new double[TAILLE];

            //Inilialisation de la partie triangulaire de 'M'
            //Initiliastion de 'Y' avec la dernière colonne de la 'M' 
            double[,] MT = new double[TAILLE, TAILLE]; // matrice Triangulaire avec Y sur la derniere colonne

            for (int l = 0; l < TAILLE; l++)
            {
                for (int c = 0; c < TAILLE; c++)
                {
                    MT[l, c] = M[l, c];
                }

                Y[l] = M[l, TAILLE];
            }

            for (int ligne = TAILLE - 1; ligne >= 0; ligne--)
            {
                double somme = 0;
                for (int colonne = ligne + 1; colonne < TAILLE; colonne++)
                {
                    somme += X[colonne] * MT[ligne, colonne];
                }

                X[ligne] = (Y[ligne] - somme) / MT[ligne, ligne];
            }

            double[] MX = ProduitV(M, X);
            if (VerificationV(MX, Y))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("\n ========> M x X = Y \n");
                Console.ForegroundColor = ConsoleColor.White;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("\n ========> M x X ≠ Y \n");
                Console.ForegroundColor = ConsoleColor.White;
            }

            // Verifiaction A*X = B

            double[] AX = ProduitV(A, X);
            if (VerificationV(AX, B))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("\n ========> A x X = B\n");
                Console.ForegroundColor = ConsoleColor.White;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("\n ========> A x X ≠ B \n");
                Console.ForegroundColor = ConsoleColor.White;
            }
            return X;
        }

        // méthode de Thomas
        static double[] MethodeThomas(double[,] A, double[] B)
        {
            int TAILLE = A.GetLength(0); // Obtient la taille d'une ligne de la matrice

            double[] X = new double[TAILLE]; // solution 

            if (VerifMTridiagonale(A)) // si la matrice est triadagonale
            {


                double[] CoeffA = new double[TAILLE]; // 1er diagonale
                double[] CoeffB = new double[TAILLE]; // 2eme diagonale
                double[] CoeffC = new double[TAILLE]; // 3eme diagonale

                // Extraction des trois diagonales de la matrice
                for (int i = 0; i < TAILLE; i++)
                {
                    for (int j = 0; j < TAILLE; j++)
                    {
                        if (A[i, j] != 0)
                        {
                            if (i - j == 1) // diagonale 1
                            {
                                CoeffA[i - 1] = A[i, j];
                            }

                            if (i == j) // diagonale 2
                            {
                                CoeffB[i] = A[i, j];
                            }

                            if (i - j == -1) // diagonale 3
                            {
                                CoeffC[j - 1] = A[i, j];
                            }
                        }
                    }
                }

                // Affichage des trois diagonale

                Console.Write("\n  Coefficient A : \n");
                AffichageV(CoeffA); // diagonale 1

                Console.Write("\n Coefficient B : \n");
                AffichageV(CoeffB); // diagonale 2

                Console.Write("\n Coefficient C : \n");
                AffichageV(CoeffC); // diagonale 3

                double[] Alpha = new double[TAILLE - 1];

                // Remplissage du tableau 'Alpha'
                Alpha[0] = CoeffC[0] / CoeffB[0];

                for (int i = 1; i < TAILLE - 1; i++)
                {
                    Alpha[i] = (CoeffC[i]) / (CoeffB[i] - (CoeffA[i - 1] * Alpha[i - 1])); //marche pas
                }

                // Remplissage tableau Beta
                double[] Beta = new double[TAILLE];

                Beta[0] = B[0] / CoeffB[0];

                for (int i = 1; i < TAILLE; i++)
                {
                    Beta[i] = (B[i] - (CoeffA[i - 1] * Beta[i - 1])) / (CoeffB[i] - (CoeffA[i - 1] * Alpha[i - 1]));

                }

                // Affichage tableaux
                Console.Write("\n  Alpha : \n");
                AffichageV(Alpha);

                Console.Write("\n  Beta : \n");
                AffichageV(Beta);

                // Calcul de la solution et Remplissage du tableau 'X'
                X[TAILLE - 1] = Beta[TAILLE - 1];

                for (int i = 3; i > -1; i--)
                {
                    X[i] = Beta[i] - (Alpha[i] * X[i + 1]);
                }

                // Affichage de la solution
                Console.Write("\n  X : \n");
                AffichageV(X);
            }
            else
            {
                Console.Write("\n ERREUR : La matrice reduite n'est pas tridiagonale, impossible d'éfféctuer la méthode de Thomas \n");
            }
            return X;
        }

        //---------------------------------Affichage--------------------------------------------------

        // Affichage Matrice 
        static void AffichageM(double[,] M, bool Couleur)
        {
            int TAILLE = M.GetLength(0); // Obtient la taille d'une ligne de la matrice

            if (Couleur)
            {
                for (int l = 0; l < TAILLE; l++)
                {
                    Console.Write("\n");
                    for (int c = 0; c < TAILLE; c++)
                    {
                        if (M[l, c] == 0)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write(" {0,8:f2} ", M[l, c]);
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write(" {0,8:f2} ", M[l, c]);
                        }

                    }
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("\n");
                }
                Console.Write("\n\n\n");
            }
            else
            {
                for (int l = 0; l < TAILLE; l++)
                {
                    Console.Write("\n");
                    for (int c = 0; c < TAILLE; c++)
                    {
                        Console.Write(" {0,8:f3} ", M[l, c]);
                    }
                    Console.Write("\n");
                }
                Console.Write("\n\n\n");
            }
        }

        // Affichage Vecteur
        static void AffichageV(double[] V)
        {

            int TAILLE = V.GetLength(0); // Obtient la taille du vecteur

            Console.Write("\n");

            Console.ForegroundColor = ConsoleColor.White;

            for (int i = 0; i < TAILLE; i++)
            {
                Console.Write("{0,9:f5}", V[i]);
            }

            Console.Write("\n\n");
        }

        // Affichage Matrice et Vecteur 

        static void AffichageMV(double[,] M, double[] V, bool Couleur)
        {
            int TAILLE = M.GetLength(0); // Obtient la taille d'une ligne de la matrice

            if (Couleur)
            {
                for (int l = 0; l < TAILLE; l++)
                {
                    Console.Write("\n");
                    for (int c = 0; c < TAILLE; c++)
                    {
                        if (M[l, c] == 0)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write(" {0,8:f2} ", M[l, c]);
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write(" {0,8:f2} ", M[l, c]);
                        }

                    }
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("     {0,8:f3}  \n", V[l]);
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.White;
                for (int l = 0; l < TAILLE; l++)
                {
                    Console.Write("\n");
                    for (int c = 0; c < TAILLE; c++)
                    {
                        if (M[l, c] == 0)
                        {
                            Console.Write(" {0,8:f2} ", M[l, c]);
                        }
                        else
                        {
                            Console.Write(" {0,8:f2} ", M[l, c]);
                        }
                    }
                    Console.Write("     {0,8:f3}  \n", V[l]);
                }
            }
        }


        //------------------------------ Fonctions de test ----------------------------------------------

        // Produit Matriciel M1 * M2
        static double[,] ProduitM(double[,] M1, double[,] M2)
        {
            int TAILLE = M1.GetLength(0); // Obtient la taille d'une ligne de la matrice

            double[,] Sol = new double[TAILLE, TAILLE];

            for (int l = 0; l < TAILLE; l++)
            {
                for (int c = 0; c < TAILLE; c++)
                {
                    double Somme = 0;
                    for (int i = 0; i < TAILLE; i++)
                    {

                        Somme = Somme + M2[i, c] * M1[l, i];

                    }

                    Sol[l, c] = Somme;
                }
            }

            return Sol;
        }

        // Produit Vectoriel M*V
        static double[] ProduitV(double[,] M, double[] V)
        {
            int TAILLE = M.GetLength(0); // Obtient la taille d'une ligne de la matrice

            double[] Sol = new double[TAILLE];

            for (int l = 0; l < TAILLE; l++)
            {
                double Somme = 0;
                for (int i = 0; i < TAILLE; i++)
                {

                    Somme = Somme + M[l, i] * V[i];

                }

                Sol[l] = Somme;

            }

            return Sol;
        }

        // Verification de l'egalite de matrice
        static bool VerificationM(double[,] M1, double[,] M2)
        {
            int TAILLE = M1.GetLength(0); // Obtient la taille d'une ligne de la matrice

            for (int l = 0; l < TAILLE; l++)
            {
                for (int c = 0; c < TAILLE; c++)
                {
                    if (Math.Abs(M1[l, c] - M2[l, c]) > 0.000001)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        // Verification de l'egalite de vecteurs
        static bool VerificationV(double[] V1, double[] V2)
        {
            int TAILLE = V1.GetLength(0); // Obtient la taille du vecteur

            for (int l = 0; l < TAILLE; l++)
            {
                if (Math.Abs(V1[l] - V2[l]) > 0.000001)
                {
                    return false;
                }
            }
            return true;
        }
        static bool VerifMTridiagonale(double[,] M)
        {
            int TAILLE = M.GetLength(0); // Obtient la taille d'une ligne de la matrice

            // verification qu'il n'y a que 2 coefficients sur la ligne 1 et la ligne n
            for (int l = 0; l < TAILLE; l += TAILLE - 1)
            {
                int compte = 0;
                for (int c = 0; c < TAILLE; c++)
                {
                    if (M[l, c] != 0)
                    {
                        compte++;
                    }
                }
                if (compte != 2)
                {
                    return false;
                }
            }

            // verification qu'il n'y a que 3 coefficients de la ligne 2 à la colonne n-1
            for (int l = 1; l < TAILLE - 1; l++)
            {
                int compte = 0;
                for (int c = 0; c < TAILLE; c++)
                {
                    if (M[l, c] != 0)
                    {
                        compte++;
                    }
                }
                if (compte != 3)
                {
                    return false;
                }
            }

            // verification qu'il n'y a que 2 coefficients sur la colonne 1 et la colonne n
            for (int c = 0; c < TAILLE; c += TAILLE - 1)
            {
                int compte = 0;
                for (int l = 0; l < TAILLE; l++)
                {
                    if (M[l, c] != 0)
                    {
                        compte++;
                    }
                }
                if (compte != 2)
                {
                    return false;
                }
            }

            // verification qu'il n'y a que 3 coefficients de la ligne 2 à la colonne n-1
            for (int c = 1; c < TAILLE - 1; c++)
            {
                int compte = 0;
                for (int l = 0; l < TAILLE; l++)
                {
                    if (M[l, c] != 0)
                    {
                        compte++;
                    }
                }
                if (compte != 3)
                {
                    return false;
                }
            }

            return true;
        }

    }
}
