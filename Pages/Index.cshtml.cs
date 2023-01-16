using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;

namespace Julia.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public double C1 { get; set; }

        [BindProperty]
        public double C2 { get; set; }

        [BindProperty]
        public string VyberPoctuBarev { get; set; }

        [BindProperty]
        public bool JednaBarvaCheck { get; set; }

        [BindProperty]
        public bool DveBarvyCheck { get; set; }

        [BindProperty]
        public string Info { get; set; }

        // base64 image:
        [BindProperty]
        public string Obrazek { get; set; }

        private Stopwatch stopwatch = new Stopwatch();

        private Random nahodneCislo = new Random();

        public IndexModel()
        {
            C1 = -0.4;
            C2 = 0.59;
            JednaBarvaCheck = true;
        }

        public void OnPostVygenerovat()
        {
            stopwatch.Restart();
            Bitmap bitmap = new Bitmap(700, 700);
            int pocetIteraci;
            int[] iterace = new int[700 * 700];
            int[] cetnostiIteraci = new int[256];
            int pocetOdstinu = 0;
            Color barva = new Color();

            for (int i = 0; i < 700; i++)
            {
                for (int j = 0; j < 700; j++)
                {
                    pocetIteraci = VyhodnoceniBodu(2 * (i / 700d - 0.5), 2 * (j / 700d - 0.5), C1, C2, 256);
                    if (VyberPoctuBarev == "JednaBarva")
                        barva = PrevodNa1Barvu(pocetIteraci);
                    if (VyberPoctuBarev == "DveBarvy")
                        barva = PrevodNa2Barvy(pocetIteraci);
                    bitmap.SetPixel(i, j, barva);
                    iterace[i * j] = pocetIteraci;
                }
            }

            MemoryStream memoryStream = new MemoryStream();
            bitmap.Save(memoryStream, ImageFormat.Png);
            byte[] byteImage = memoryStream.ToArray();
            Obrazek = "data:image/png;base64," + Convert.ToBase64String(byteImage);

            for (int i = 0; i < iterace.Length; i++)
            {
                if (iterace[i] <= 255)
                    cetnostiIteraci[iterace[i]]++;
            }

            for (int i = 0; i < cetnostiIteraci.Length; i++)
            {
                if (cetnostiIteraci[i] > 0)
                    pocetOdstinu++;
            }

            stopwatch.Stop();
            Info = "počet odstínů: " + pocetOdstinu.ToString() + ", čas: " + stopwatch.ElapsedMilliseconds.ToString() + " ms";
            if (VyberPoctuBarev == "JednaBarva")
            {
                JednaBarvaCheck = true;
                DveBarvyCheck = false;
            }
            if (VyberPoctuBarev == "DveBarvy")
            {
                JednaBarvaCheck = false;
                DveBarvyCheck = true;
            }
        }

        public void OnPostNahodne()
        {
            C1 = Math.Round((nahodneCislo.NextDouble() - 0.5) * 2, 3);
            C2 = Math.Round((nahodneCislo.NextDouble() - 0.5) * 2, 3);
            OnPostVygenerovat();
        }

        public void OnPostVyhledat()
        {
            double c1;
            double c2;
            int[] iterace = new int[100 * 100];
            int[] cetnostiIteraci = new int[256];
            int pocetOdstinu;

            do
            {
                c1 = Math.Round((nahodneCislo.NextDouble() - 0.5) * 2, 3);
                c2 = Math.Round((nahodneCislo.NextDouble() - 0.5) * 2, 3);
                pocetOdstinu = 0;

                for (int i = 0; i < 100; i++)
                {
                    for (int j = 0; j < 100; j++)
                    {
                        iterace[i * j] = VyhodnoceniBodu(2 * (i / 100d - 0.5), 2 * (j / 100d - 0.5), c1, c2, 256);
                    }
                }

                for (int i = 0; i < iterace.Length; i++)
                {
                    if (iterace[i] <= 255)
                        cetnostiIteraci[iterace[i]]++;
                }

                for (int i = 0; i < cetnostiIteraci.Length; i++)
                {
                    if (cetnostiIteraci[i] > 0)
                        pocetOdstinu++;
                }

            } while (pocetOdstinu <= 75);

            C1 = c1;
            C2 = c2;
            OnPostVygenerovat();
        }

        private int VyhodnoceniBodu(double polohaX, double polohaY, double c1, double c2, int maximalniPocetIteraci)
        {
            int pocetIteraci = 0;
            double z1 = polohaX;
            double z2 = polohaY;
            double z1Puvodni;
            double z2Puvodni;

            do
            {
                z1Puvodni = z1;
                z2Puvodni = z2;
                z1 = z1Puvodni * z1Puvodni - z2Puvodni * z2Puvodni + c1;
                z2 = 2 * z1Puvodni * z2Puvodni + c2;
                pocetIteraci++;
            }
            while ((z1 * z1 + z2 * z2) < 4 && pocetIteraci < maximalniPocetIteraci);

            return pocetIteraci;
        }

        private Color PrevodNa1Barvu(int pocetIteraci)
        {
            Color barva;
            Color barva0 = Color.FromArgb(255, 255, 255);
            Color barva1 = Color.FromArgb(0, 0, 0);
            double koeficient = 1 - pocetIteraci / 256d;

            barva = Color.FromArgb((byte)(koeficient * barva0.R + (1 - koeficient) * barva1.R), (byte)(koeficient * barva0.G + (1 - koeficient) * barva1.G), (byte)(koeficient * barva0.B + (1 - koeficient) * barva1.B));
            return barva;
        }

        private Color PrevodNa2Barvy(int pocetIteraci)
        {
            Color barva;
            Color barva0 = Color.FromArgb(255, 255, 255);
            Color barva1 = Color.FromArgb(0, 0, 255);
            Color barva2 = Color.FromArgb(255, 0, 0);
            double koeficient = 1 - pocetIteraci / 256d;

            if (pocetIteraci <= 128)
                barva = Color.FromArgb((byte)(koeficient * barva0.R + (1 - koeficient) * barva1.R), (byte)(koeficient * barva0.G + (1 - koeficient) * barva1.G), (byte)(koeficient * barva0.B + (1 - koeficient) * barva1.B));
            else if (pocetIteraci > 128 && pocetIteraci <= 256)
                barva = Color.FromArgb((byte)(koeficient * barva1.R + (1 - koeficient) * barva2.R), (byte)(koeficient * barva1.G + (1 - koeficient) * barva2.G), (byte)(koeficient * barva1.B + (1 - koeficient) * barva2.B));
            else
                barva = barva2;
            return barva;
        }
    }
}
