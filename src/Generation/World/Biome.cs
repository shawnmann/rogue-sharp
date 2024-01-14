using Rogue;

namespace Rogue.Generation.World;

public record Biome
{
    public BiomeTypes BiomeType { get; init; }
    public BiomeTemps Temp { get; init; }
    public BiomeMoistures Moisture { get; init; }
}
