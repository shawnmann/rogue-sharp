using Godot;
using System;

public partial class FastNoise
{
    public FastNoiseLite elevation;
    public FastNoiseLite moisture;
    public FastNoiseLite temperature;
    public String[,] MapInfo;
    
    public FastNoise(int width, int height)
    {
        var random = new RandomNumberGenerator();
        
        // https://docs.godotengine.org/en/stable/classes/class_fastnoiselite.html

        // Basically generate height, temperature, and moisture
        // then decide, based on those maps, what each tile is
        
        random.Randomize();
        elevation = new FastNoiseLite();
        elevation.Seed = random.RandiRange(0, 500);
        elevation.NoiseType = FastNoiseLite.NoiseTypeEnum.Perlin;
        elevation.FractalOctaves = 6;
        elevation.Frequency = 0.01f;
        elevation.FractalLacunarity = 2f;
        elevation.FractalGain = 0.5f;
        elevation.FractalType = FastNoiseLite.FractalTypeEnum.Ridged;
        elevation.CellularJitter = 50;
        elevation.DomainWarpEnabled = true;
        
        random.Randomize();
        moisture = new FastNoiseLite();
        moisture.Seed = random.RandiRange(0, 500);
        moisture.NoiseType = FastNoiseLite.NoiseTypeEnum.Perlin;
        moisture.FractalOctaves = 6;
        moisture.Frequency = 0.01f;
        moisture.FractalLacunarity = 2f;
        moisture.FractalGain = 0.5f;
        moisture.FractalType = FastNoiseLite.FractalTypeEnum.Ridged;
        moisture.CellularJitter = 50;
        moisture.DomainWarpEnabled = true;
        
        random.Randomize();
        temperature = new FastNoiseLite();
        temperature.Seed = random.RandiRange(0, 500);
        temperature.NoiseType = FastNoiseLite.NoiseTypeEnum.Perlin;
        temperature.FractalOctaves = 6;
        temperature.Frequency = 0.01f;
        temperature.FractalLacunarity = 2f;
        temperature.FractalGain = 0.5f;
        temperature.FractalType = FastNoiseLite.FractalTypeEnum.Ridged;
        temperature.CellularJitter = 50;
        temperature.DomainWarpEnabled = true;

        MapInfo = new String[width, height];

        float maxE = 0;
        float minE = 0;
        float maxT = 0;
        float minT = 0;
        float maxM = 0;
        float minM = 0;
        float seaLevel = 0; //10000

        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                // Get the values for this cell
                var m = moisture.GetNoise2D(x, y);
                var e = elevation.GetNoise2D(x, y);
                var t = temperature.GetNoise2D(x, y);
                
                // Convert temp to between 0 and 40 (degrees celsius?)
                t = (int)Math.Floor(t * 40);

                // Convert elevation to between 0 and 19000 (meters?)
                // 0 is bottom of sea, sea is ~10,000m deep, so 10,000 is sea level
                e = (int)Math.Floor(e * 19000);
                
                // Convert moisture to between 0 and 400 (cm annual precipitation)
                m = (int)Math.Floor(m * 400);
                
                // You lose 10 degrees celsius per 1,000 meters of elevation

                if (x == 0 && y == 0)
                {
                    maxE = e;
                    minE = e;
                    maxM = m;
                    minM = m;
                    maxT = t;
                    minT = t;
                }
                else
                {
                    maxE = e > maxE ? e : maxE;
                    minE = e < minE ? e : minE;
                    maxT = t > maxT ? t : maxT;
                    minT = t < minT ? t : minT;
                    maxM = m > maxM ? m : maxM;
                    minM = m < minM ? m : minM;
                }

                var value = "X";

                if (e < seaLevel)
                {
                    // We are below sea level, so just make me water
                    value = "W";
                }

                if (e >= seaLevel)
                {
                    // We are above sea level
                    if (t > 32)
                    {
                        if (m > 250)
                        {
                            value = "TROPICAL_RAINFOREST";
                        } else if (m > 80)
                        {
                            value = "SAVANNA";
                        }
                        else
                        {
                            value = "DESERT";
                        }
                    } else if (t > 15)
                    {
                        if (m > 200)
                        {
                            value = "TEMPERATE_RAINFOREST";
                        } else if (m > 100)
                        {
                            value = "TEMPERATE_SEASONAL_RAINFOREST";
                        } else if (m > 50)
                        {
                            value = "SHRUBLAND";
                        }
                        else
                        {
                            value = "TEMPERATE_GRASSLAND";
                        }
                    } else if (t > 10)
                    {
                        value = "BOREAL_FOREST";
                    }
                    else
                    {
                        value = "TUNDRA";
                    }
                }

                MapInfo[x, y] = value;
            }
        }
        
        GD.Print($"ELEVATION - MAX: {maxE} MIN: {minE}");
        GD.Print($"MOISTURE - MAX: {maxM} MIN: {minM}");
        GD.Print($"TEMPERATURE - MAX {maxT} MIN: {minT}");
    }
}
