using Godot;
using System;
using System.Linq;
using Rogue;
using Rogue.Generation.World;

public partial class FastNoise
{
    public FastNoiseLite elevation;
    public FastNoiseLite moisture;
    public FastNoiseLite temperature;
    public Biome[,] MapInfo;
    
    public FastNoise(int width, int height)
    {
        var random = new RandomNumberGenerator();
        
        // https://docs.godotengine.org/en/stable/classes/class_fastnoiselite.html

        // Basically generate height, temperature, and moisture
        // then decide, based on those maps, what each tile is
        
        // Generate an elevation map
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
        
        // Generate a moisture map
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
        
        // Generate a temperature map
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

        MapInfo = new Biome[width, height];

        // Hold the max and mins of each value
        float maxE = 0;
        float minE = 0;
        float maxT = 0;
        float minT = 0;
        float maxM = 0;
        float minM = 0;
        
        // If we want to use sea level
        float seaLevel = 0; //~10000 meters is the deepest part of Earth's ocean

        // Make arrays to hold the sorted values of each 
        float[] sortedMoisture = new float[width * height];
        float[] sortedTemperature = new float[width * height];
        float[] sortedElevation = new float[width * height];

        // Add results to a flat array so we can sort them
        int i = 0;
        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                sortedMoisture[i] = moisture.GetNoise2D(x, y);
                sortedTemperature[i] = temperature.GetNoise2D(x, y);
                sortedElevation[i] = elevation.GetNoise2D(x, y);
                i++;
            }
        }

        // Sort the flat versions of the arrays so we can use them to calculate stuff
        Array.Sort(sortedMoisture);
        Array.Sort(sortedTemperature);
        Array.Sort(sortedElevation);

        // Get thresholds so we can assign biomes
        var moistureLowLimit = sortedMoisture[(int)Math.Floor(sortedMoisture.Length * .3)];
        var moistureMedLimit = sortedMoisture[(int)Math.Floor(sortedMoisture.Length * .6)];
        var tempLowLimit = sortedTemperature[(int)Math.Floor(sortedTemperature.Length * .3)];
        var tempMedLimit = sortedTemperature[(int)Math.Floor(sortedTemperature.Length * .6)];

        // Go through and decide biomes
        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                // Get the values for this cell
                var m = moisture.GetNoise2D(x, y);
                var e = elevation.GetNoise2D(x, y);
                var t = temperature.GetNoise2D(x, y);
                
                BiomeMoistures biomeMoisture;
                BiomeTemps biomeTemp;

                if (m < moistureLowLimit)
                {
                    biomeMoisture = BiomeMoistures.Dry;
                } else if (m < moistureMedLimit)
                {
                    biomeMoisture = BiomeMoistures.Moderate;
                }
                else
                {
                    biomeMoisture = BiomeMoistures.Wet;
                }

                if (t < tempLowLimit)
                {
                    biomeTemp = BiomeTemps.Cold;
                } else if (t < tempMedLimit)
                {
                    biomeTemp = BiomeTemps.Temperate;
                }
                else
                {
                    biomeTemp = BiomeTemps.Hot;
                }
                
                // Load the biome into the results
                MapInfo[x, y] = GameConstants.GetBiome(biomeMoisture, biomeTemp);
            }
        }
        
        // Now go through and make sure all biome cells are orthogonal to the same biome
        //  so we don't have hanging/diagonal cells...
        for (var x = 0; x < width; x++)
        {
            for (var y = 0; y < height; y++)
            {
                var thisBiome = MapInfo[x, y].BiomeType;
                
                // Grab the neighboring biomes
                var topBiome = y > 0 ? MapInfo[x, y - 1].BiomeType : (BiomeTypes?)null;
                var bottomBiome = y < height - 1 ? MapInfo[x, y + 1].BiomeType : (BiomeTypes?)null;
                var leftBiome = x > 0 ? MapInfo[x - 1, y].BiomeType : (BiomeTypes?)null;
                var rightBiome = x < width - 1 ? MapInfo[x + 1, y].BiomeType : (BiomeTypes?)null;
                
                if ((topBiome != thisBiome && topBiome != null) &&
                    (bottomBiome != thisBiome && bottomBiome != null) &&
                    (leftBiome != thisBiome && leftBiome != null) &&
                    (rightBiome != thisBiome && rightBiome != null))
                {
                    // There is no equal biome orthogonal so we need to change this one
                    var switchedBiome = (BiomeTypes?)null;
                    while (switchedBiome == null)
                    {
                        // Randomly pick a neighbor to switch this to
                        random.Randomize();
                        var r = random.RandiRange(0, 3);
                        switchedBiome = r switch
                        {
                            0 => topBiome,
                            1 => bottomBiome,
                            2 => leftBiome,
                            3 => rightBiome,
                            _ => null
                        };
                    }
                    MapInfo[x, y] = GameConstants.GetBiome(switchedBiome);
                }
            }
        }
        
        // TODO: Rivers
        // Choose a random cell along the top row
        
        /*random.Randomize();
        var riverStartingX = random.RandiRange(20, width - 20);
        var river = new Vector2[height - 1];
        for (var y = 0; y < height; y++)
        {
            // Get the previous x (or if this is the first row, get the starting point)
            var x = y == 0 ? riverStartingX : river[y - 1].X;
            
            // TODO: Some algorithm that winds the river top to bottom...
            
            // Add the next cell to the river
            river[y] = new Vector2(x, y);
        }*/
        
        GD.Print($"ELEVATION - MAX: {maxE} MIN: {minE}");
        GD.Print($"MOISTURE - MAX: {maxM} MIN: {minM}");
        GD.Print($"TEMPERATURE - MAX {maxT} MIN: {minT}");
    }
}
