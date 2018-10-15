using RDotNet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Printing;
namespace RDotNet
{

    class Program
    {
        static void Main(string[] args)
        {

            SetupPath(); // current process, soon to be deprecated

            // There are several options to initialize the engine, but by default the following suffice:
            REngine engine = REngine.GetInstance();
            engine.Initialize(); // required since v1.5                            

            // some random weight samples 
            double[] weight = new double[] { 3.2, 3.6, 3.2, 1.7, 0.8, 2.9, 2, 1.4, 1.2, 2.1, 2.5, 3.9, 3.7, 2.4, 1.5, 0.9, 2.5, 1.7, 2.8, 2.1, 1.2 };
            double[] lenght = new double[] { 2, 3, 3.2, 4.7, 5.8, 3.9, 2, 8.4, 5.2, 4.1, 2.5, 3.9, 5, 2.4, 3.5, 0.9, 2.5, 2.7, 2.8, 2.1, 1.2 };

            // introduce the samples into R
            engine.SetSymbol("weight", engine.CreateNumericVector(weight));
            engine.SetSymbol("lenght", engine.CreateNumericVector(lenght));

            // set the weights and lenghts as a data frame (regular R syntax in string)
            engine.Evaluate("df <- data.frame(id=c(1:length(weight)), weight = weight,lenght = lenght )");

         
            // evaluate and retrieve mean 
            double avg = engine.Evaluate("mean(df$weight)").AsNumeric().ToArray()[0];
            // same for standard deviation
            double std = engine.Evaluate("sd(df$weight)").AsNumeric().ToArray()[0];
            // NumericVector coeff = engine.Evaluate("coefficients(lm(df$weight ~ df$lenght ))").AsNumeric();
            // print output in console
            System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-gb");

            //Show in console the weight and lenght data
            Console.WriteLine(string.Format("Weights: ({0})", string.Join(",",
                weight.Select(f => f.ToString(ci)) // LINQ expression
                )));
            Console.WriteLine(string.Format("Length: ({0})", string.Join(",",
            lenght.Select(f => f.ToString(ci)) // LINQ expression
            )));
            Console.WriteLine(string.Format("Sample size: {0}", weight.Length));
            Console.WriteLine(string.Format(ci, "Average: {0:0.00}", avg));
            Console.WriteLine(string.Format(ci, "Standard deviation: {0:0.00}", std));

            var result = engine.Evaluate("lm(df$weight ~ df$lenght)");
            engine.SetSymbol("result", result);
            var coefficients = result.AsList()["coefficients"].AsNumeric().ToList();
            double r2 = engine.Evaluate("summary(result)").AsList()["r.squared"].AsNumeric().ToList()[0];
            double intercept = coefficients[0];
            double slope = coefficients[1];
            Console.WriteLine("Intercept:" + intercept.ToString());
            Console.WriteLine("slope:" + slope);
            Console.WriteLine("r2:" + r2);
           
            string fileName = "myplot.png";

            CharacterVector fileNameVector = engine.CreateCharacterVector(new[] { fileName });
            engine.SetSymbol("fileName", fileNameVector);
            
            engine.Evaluate("png(filename=fileName, width=6, height=6, units='in', res=100)");
            engine.Evaluate("reg <- lm(df$weight ~ df$lenght)");
            engine.Evaluate("plot(df$weight ~ df$lenght)");
            engine.Evaluate("abline(reg)");
            engine.Evaluate("dev.off()");
            //The file will save in debug directory
      
            Application.Run(new Form1());
            // After disposing of the engine, you cannot reinitialize nor reuse it
            engine.Dispose();

        }
        public static void SetupPath(string Rversion = "R-3.4.4")
        {
            var oldPath = System.Environment.GetEnvironmentVariable("PATH");
            var rPath = System.Environment.Is64BitProcess ?
                                   string.Format(@"C:\Program Files\R\{0}\bin\x64", Rversion) :
                                   string.Format(@"C:\Program Files\R\{0}\bin\i386", Rversion);

            if (!Directory.Exists(rPath))
                throw new DirectoryNotFoundException(
                  string.Format(" R.dll not found in : {0}", rPath));
            var newPath = string.Format("{0}{1}{2}", rPath,
                                         System.IO.Path.PathSeparator, oldPath);
            System.Environment.SetEnvironmentVariable("PATH", newPath);
        }

    }
}