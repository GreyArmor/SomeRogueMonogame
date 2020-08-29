 

using System;

namespace NamelessRogue.Engine.Engine.Generation.Noise
{
    public class SimplexNoise {

        SimplexNoise_octave[] octaves;
        double[] frequencys;
        double[] amplitudes;

        private int largestFeature;
        double persistence;

        public SimplexNoise(int largestFeature,double persistence, Random rnd){
            this.largestFeature=largestFeature;
            this.persistence=persistence;

            //recieves a number (eg 128) and calculates what power of 2 it is (eg 2^7)
            int numberOfOctaves=(int)Math.Ceiling(Math.Log10(largestFeature)/Math.Log10(2));

            octaves=new SimplexNoise_octave[numberOfOctaves];
            frequencys=new double[numberOfOctaves];
            amplitudes=new double[numberOfOctaves];


            for(int i=0;i<numberOfOctaves;i++){
                octaves[i]=new SimplexNoise_octave(rnd.Next());

                frequencys[i] = Math.Pow(2,i);
                amplitudes[i] = Math.Pow(persistence,octaves.Length-i);




            }

        }


        public double getNoise(double x, double y){

            double result=0;

            for(int i=0;i<octaves.Length;i++){
                //double frequency = Math.Pow(2, i);
                //double amplitude = Math.Pow(persistence, octaves.Length - i);

                result = result+octaves[i].noise(x/frequencys[i], y/frequencys[i])* amplitudes[i];
            }


            return result;

        }

        public double getNoise(double x,double y, double z){

            double result=0;

            for(int i=0;i<octaves.Length;i++){
                double frequency = Math.Pow(2,i);
                double amplitude = Math.Pow(persistence,octaves.Length-i);

                result=result+octaves[i].noise(x/frequency, y/frequency,z/frequency)* amplitude;
            }


            return result;

        }

        public int getLargestFeature() {
            return largestFeature;
        }
    }
}