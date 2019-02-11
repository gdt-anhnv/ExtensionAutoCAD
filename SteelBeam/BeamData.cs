using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteelBeam
{
    public class BeamDauGoiData
    {
        public int number_steel_first_layer;
        public int first_layer_diameter;
        public int number_steel_second_layer;
        public int second_layer_diameter;
        public int type_beam;
        public int dai_diameter;
        public int spacing;
        public int dai_diameter2;
        public int spacing2;
    }

    public class BeamGiuaData
    {
        public int number_steel_first_layer;
        public int first_layer_diameter;
        public int number_steel_second_layer;
        public int second_layer_diameter;
        public int type_beam;
        public int dai_diameter;
        public int spacing;
        public int dai_diameter2;
        public int spacing2;
    }

    public class BeamData
    {
        private int width;
        public int Width
        {
            set { width = value; }
            get { return width; }
        }
        private int height;
        public int Height
        {
            set { height = value; }
            get { return height; }
        }
        private int length;
        public int Length
        {
            set { length = value; }
            get { return length; }
        }

        public BeamDauGoiData dau_goi_data;
        public BeamGiuaData giua_data;

        public BeamData()
        {
            width = 0;
            height = 0;
            length = 0;
            dau_goi_data = new BeamDauGoiData();
            giua_data = new BeamGiuaData();
        }
    }
}
