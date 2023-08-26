using NPlug;

namespace Warbler;
public class WarblerModel : AudioProcessorModel
{
    public WarblerModel() : base("Warbler") 
    {
        AddByPassParameter();
        Depth = AddParameter(new AudioRangeParameter("Depth", minValue:0.0, maxValue:1.0, defaultPlainValue:0.0));
        Rate = AddParameter(new AudioRangeParameter("Rate", units: "Hz", minValue:0.0, maxValue:10.0, defaultPlainValue:0.0));
        ManualDepth = AddParameter(new AudioRangeParameter("ManualDepth"));
    }
    public AudioParameter Depth { get; }
    public AudioParameter Rate { get; }
    public AudioRangeParameter ManualDepth { get; }
}
