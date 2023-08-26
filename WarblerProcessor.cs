using NPlug;

namespace Warbler;

public class WarblerProcessor : AudioProcessor<WarblerModel>
{
    private float[] bufferLeft;
    private float[] bufferRight;
    private int bufferLength;
    private int bufferPositionLeft;
    private int bufferPositionRight;
    private double leftSweep;
    private double rightSweep;
    private double prevManualDepth;

    public WarblerProcessor() : base(AudioSampleSizeSupport.Float32)
    {
        bufferLeft = Array.Empty<float>();
        bufferRight = Array.Empty<float>();
        bufferLength = 2000; 
        bufferPositionLeft = bufferLength - 1;
        bufferPositionRight = bufferLength - 1;
        leftSweep = 0;
        rightSweep = 0;
    }
    protected override void OnActivate(bool isActive)
    {
        if (isActive)
        {
            bufferLeft = GC.AllocateArray<float>(bufferLength, true);
            bufferRight = GC.AllocateArray<float>(bufferLength, true);
        }
        else
        {
            bufferLeft = Array.Empty<float>();
            bufferRight = Array.Empty<float>();
        }
    }
    protected override bool Initialize(AudioHostApplication host)
    {
        AddAudioInput("AudioInput", SpeakerArrangement.SpeakerStereo);
        AddAudioOutput("AudioOutput", SpeakerArrangement.SpeakerStereo);
        return true;
    }
    protected override void ProcessMain(in AudioProcessData data)
    {
        var depth = Model.Depth.ToPlain(Model.Depth.NormalizedValue);
        var sampleCount = data.SampleCount;
        var secondsPerSample = 1.0 / data.GetContext().SampleRate;
        var rateInHz = Model.Rate.ToPlain(Model.Rate.NormalizedValue);
        var cyclesPerSample = rateInHz * secondsPerSample;
        var radiansPerSample = cyclesPerSample * (Math.PI * 2.0);

        var manualDepth = Model.ManualDepth.NormalizedValue;
        var manualDepthDelta = manualDepth - prevManualDepth;
        var manualDepthPerSample = manualDepthDelta / ((double)sampleCount);

        for (int channel = 0; channel < 2; channel++)
        {
            var inputChannel = data.Input[0].GetChannelSpanAsFloat32(ProcessSetupData, data, channel);
            var outputChannel = data.Output[0].GetChannelSpanAsFloat32(ProcessSetupData, data, channel);
            var buffer = channel == 0 ? bufferLeft : bufferRight;
            var bufferPosition = channel == 0 ? bufferPositionLeft : bufferPositionRight;
            var sweep = channel == 0 ? leftSweep : rightSweep;
            var sampleManualDepth = prevManualDepth;

            for (int sample = 0; sample < sampleCount; sample++)
            {
                buffer[bufferPosition] = inputChannel[sample];

                var offset = (buffer.Length-1)*(depth*(Math.Sin(sweep)/2.0+1.0) + sampleManualDepth);
                var offsetFraction = offset - Math.Floor(offset);
                var offsetBufferPosition = bufferPosition + (int)offset;

                var newSample = buffer[offsetBufferPosition % buffer.Length] * (1.0 - offsetFraction);
                newSample += buffer[(offsetBufferPosition+1) % buffer.Length] * offsetFraction;
                sweep += radiansPerSample;
                if (sweep >= 2*Math.PI) 
                {
                    sweep -= Math.PI;
                }

                bufferPosition--;
                if (bufferPosition < 0) 
                {
                    bufferPosition = buffer.Length - 1;
                }

                sampleManualDepth += manualDepthPerSample;

                outputChannel[sample] = (float)newSample;
            }

            if(channel == 0)
            {
                bufferPositionLeft = bufferPosition;
                leftSweep = sweep;
            }
            else
            {
                bufferPositionRight = bufferPosition;
                rightSweep = sweep;
            }
        }

        prevManualDepth = manualDepth;
    }

    public static readonly Guid ClassId = new("d7383385-bff7-4cb3-8803-400e2a17053f");

    public override Guid ControllerClassId => WarblerController.ClassId;
}