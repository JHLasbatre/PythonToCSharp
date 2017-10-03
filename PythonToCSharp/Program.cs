using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PythonToCSharp
{
    class Program
    {

        //Récupération d'historiques de cotation

        static List<double> RecupDonnees()
        {
            //string folder = "C:\\Users\\J-H\\Desktop\\histo_cotation";
            //string fileName = "CotationsAF 22-09-15 - 21-09-17.txt";
            StreamReader MySR = new StreamReader(@"C:\Users\J-H\Desktop\histo_cotation\CotationsAF 22-09-15 - 21-09-17.txt");
            string line = MySR.ReadLine();
            //List<string> content = new List<string>();
            List<double> priceList = new List<double>();
            
            // parcours du fichier texte ligne par ligne, et extraction de line[5]

                
            while(line != null)
            {
                string[] tempString = line.Split(';');
                for(int i = 0; i < tempString.Count(); i++)
                {
                    tempString[i] = tempString[i].Replace('.', ',');
                }
                priceList.Add(Convert.ToDouble(tempString[5]));
                line = MySR.ReadLine();
            }
            return priceList;
        }
        
        // Tracé de graph simples - à terminer !!

        

        // Extraction des paramètres et estimation pour t>t0

            
        static double avg(List<double> Y)
        {
            int taille = Y.Count();
            double sum = 0;
            for(int i =0; i< taille; i++)
            {
                sum += Y[i];
            }
            return sum / taille;
        }
        static List<double> eraseNoise(List<double> Y, int p)
        {
            List<double> noNoise = new List<double>();
            for(int i = p; i < Y.Count(); i++)
            {
                List<double> temp = new List<double>();
                for (int j = i-p; j < i; j++)
                {
                    temp.Add(Y[j]);
                }
                noNoise.Add(avg(temp));
            }
            return noNoise;
        }
            

             
        static List<double> EvalMu(List<double> Y, int p)
        {
            List<double> mu = new List<double>();
            List<double> noNoise = eraseNoise(Y, p);
            for(int i = p; i < noNoise.Count(); i++)
            {
                mu.Add((noNoise[i-1] - noNoise[i]) / noNoise[i-1]);
            }
            return mu;
        }

        static List<double> EvalSigma(List<double> Y, List<double> mu, int p)
        {
            List<double> sigma = new List<double>();
            for(int i = p; i < mu.Count(); i++)
            {
                sigma.Add(Math.Abs((Y[p + i + 1] - Y[p + i]) / Y[p + i] - mu[i]));
            }
            return sigma;
        }
        
        static List<double> FromToList(List<double> Y, int f, int t)
        {
            List<double> reponse = new List<double>();
            for(int i = f; i < t+1; i++)
            {
                reponse.Add(Y[i]);
            }
            return reponse;
        }

    
        static double MaxList(List<double> Y)
        {
            double max = Y[0];
            for(int i = 0; i < Y.Count(); i++)
            {
                if(Y[i] > max)
                {
                    max = Y[i];
                }
            }
            return max;
        }
        
        static List<double> EvalSigmaMax(List<double> Y, int pSigma) //use this method only with a sigma list
        {
            int remaining;
            List<double> sigmaMax = new List<double>();
            for(int i = 0; i < Y.Count(); i = i + pSigma)
            {
                remaining = Y.Count() - i;
                if(remaining < pSigma)
                {
                    pSigma = remaining;
                }
                for (int j = 0; j < pSigma; j++)
                {
                    sigmaMax.Add(MaxList(FromToList(Y, i, i + pSigma)));
                }
            }
            return sigmaMax;
        }
        
        
       
        static void printList(List<string> L)
        {
            Console.Write("[");
            for(int i = 0; i<L.Count()-1;i++)
            {
                Console.Write(L[i] + ", ");
            }
            Console.WriteLine(L[L.Count()-1] + "]");
        }

        static void printList(List<double> L)
        {
            Console.Write("[");
            for(int i = 0; i < L.Count()-1; i++)
            {
                Console.WriteLine(L[i] + ", ");
            }
            Console.WriteLine(L[L.Count - 1] + "]");
        }
        
        static void printList(string[] L)
        {
            Console.Write("[" + L[0]);
            int i = 0;
            while(L[i+1] != null)
            {
                i++;
                Console.Write(", " + L[i]);
            }
        }

        static void init()
        {
            int p = 2;
            List<double> Y = RecupDonnees();
            List<double> mu = EvalMu(Y, p);
            List<double> sigma = EvalSigma(Y, mu, p);
            List<double> sigmaMax = EvalSigmaMax(sigma, 10);
            printList(Y);

            //printList(eraseNoise(Y, 2));
            //printList(mu);
            //printList(sigma);
            //printList(FromToList(Y, 0, 5));
            //printList(sigmaMax);
            //Console.WriteLine("taille mu: " + mu.Count() + "\n taille sigma: " + sigma.Count() + "\n taille sigmaMax: " + sigmaMax.Count());
        }

        static void Main(string[] args)
        {
            
            init();
            Console.ReadKey();

        }
    }
}
