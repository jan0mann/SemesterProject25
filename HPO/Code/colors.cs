using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using SkiaSharp;

namespace HPO.Models
{
    public class ColorsClass
    {
        private List<SKColor> colorList = new List<SKColor> {
            new SKColor(249, 202, 36),//dark pink
            new SKColor(240, 147, 43),//dark pink
            new SKColor(235, 77, 75),//dark pink
            new SKColor(106, 176, 76),//dark pink
            new SKColor(199, 236, 238),//dark pink
            new SKColor(34, 166, 179),//dark pink
            new SKColor(190, 46, 221),//dark pink
            new SKColor(72, 52, 212),//dark pink
            new SKColor(19, 15, 64),//dark pink
            new SKColor(83, 92, 104),//dark pink


        };
        private int counter = -1;
        public SKColor getColor(){
            if(colorList.Count == counter-1){
                counter = 0;
            }else{
                counter++;
            }
            return colorList[counter];
        }
        public SKColor getColor(int number){
            return colorList[number];
        }

        public ColorsClass(int counterArg){
            counter = counterArg;
        }
        public ColorsClass(){
        }
    }
}